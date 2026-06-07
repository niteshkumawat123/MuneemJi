using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using MUNEEMJI.Services;
using Insight.Database;
using Microsoft.Data.SqlClient;
using System.IO;
using MUNEEMJI.Repositories;
using MimeKit;

namespace MUNEEMJI.Controllers
{
    public class UserController: Controller
    {
        private readonly string _connectionString;
        private ICompanyTenancy _companyTenancy;
        public IUser _iuser;
        private readonly IErrorLogService _errorLogService;

        public UserController(IConfiguration configuration, ICompanyTenancy companyTenancy, IUser user, IErrorLogService errorLogService)
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;
            _companyTenancy = companyTenancy;
            _iuser = user;
            _errorLogService = errorLogService;
        }

        public async Task<IActionResult> Index()
        {
            var users = new List<MUNEEMJI.Models.Business>();
            var CompanyId = _companyTenancy.GetCurrentCompanyId();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id, business_name, phone, email, status, roleid, created_at, updated_at ,username
                FROM businesses where companyid = @p_companyid
                ORDER BY created_at DESC";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("p_companyid", CompanyId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new MUNEEMJI.Models.Business
                {
                    Id = reader["id"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("id")) : 0,
                    BusinessName = reader["business_name"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("business_name")) : null,
                    Phone = reader["phone"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("phone")) : null,
                    Email = reader["email"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("email")) : null,
                    Status = reader["status"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("status")) : 0,
                    RoleId = reader["roleid"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("roleid")) : 0,
                    CreatedAt = reader["created_at"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("created_at")) : DateTime.MinValue,
                    UpdatedAt = reader["updated_at"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("updated_at")) : DateTime.MinValue,
                    Username  = reader["username"]!=DBNull.Value?reader.GetString(reader.GetOrdinal("username")): reader["business_name"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("business_name")) : null
                });
            }

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            int Roleid = 0;
            var viewModel = new AddUserViewModel
            {
                AvailableRoles = await GetAvailableRoles(),
                ModulePermissions = await GetRolePermissions(Roleid)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            var CompanyId = _companyTenancy.GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await GetAvailableRoles();
                return View(model);
            }

            var businessIdString = HttpContext.Session.GetString("BusinessId");
            var BusinessName = HttpContext.Session.GetString("BusinessName");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ?? Step 1: Check if user already exists
            var checkQuery = @"
                                SELECT COUNT(1) 
                                FROM businesses 
                                WHERE email = @Email AND companyid = @CompanyId";

            using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("Email", model.PhoneOrEmail);
                checkCommand.Parameters.AddWithValue("CompanyId", Convert.ToInt32(businessIdString));

                var exists = (long)await checkCommand.ExecuteScalarAsync();

                if (exists > 0)
                {
                    // User already exists ? return with error message
                    ModelState.AddModelError("", "A user with this email already exists for this business.");
                    model.AvailableRoles = await GetAvailableRoles();
                    return View(model);
                }
            }

            // ?? Step 2: Insert new user if not exists
            var insertQuery = @"
                                INSERT INTO businesses 
                                ( business_name, phone, email, created_at, updated_at, status, roleid, isactive, companyid, username, isowner)
                                VALUES ( @businessName, @phone, @email, @createdAt, @updatedAt, @status, @roleid, @isactive, @companyid, @username, @isowner)";


            using (var command = new NpgsqlCommand(insertQuery, connection))
            {

                command.Parameters.AddWithValue("businessName", BusinessName);
                command.Parameters.AddWithValue("phone", model.PhoneOrEmail);
                command.Parameters.AddWithValue("email", model.PhoneOrEmail);
                command.Parameters.AddWithValue("createdAt", DateTime.Now);
                command.Parameters.AddWithValue("updatedAt", DateTime.Now);
                command.Parameters.AddWithValue("status", 0); // Pending
                command.Parameters.AddWithValue("roleid", model.SelectedRoleId);
                command.Parameters.AddWithValue("isactive", true);
                command.Parameters.AddWithValue("companyid", CompanyId);
                command.Parameters.AddWithValue("username", model.FullName);
                command.Parameters.AddWithValue("isowner", false); // or true depending on logic
                var rows = await command.ExecuteNonQueryAsync();


                if (rows > 0)
                {
                    // Load email template
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email-templates", "InviteTemplate.html");
                    var emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    // Replace placeholders
                    emailBody = emailBody.Replace("{UserName}", model.FullName)
                                         .Replace("{BusinessName}", businessIdString);

                    // Send email with retry
                    int maxRetries = 3;
                    for (int attempt = 1; attempt <= maxRetries; attempt++)
                    {
                        try
                        {
                            var message = new MimeMessage();
                            message.From.Add(new MailboxAddress("MuneemJiApp", "noreplymuneemjii@gmail.com"));
                            message.To.Add(new MailboxAddress("", model.PhoneOrEmail));
                            message.Subject = "You have been invited to join MunnemJi";
                            message.Body = new TextPart("html") { Text = emailBody };

                            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                            smtpClient.Timeout = 30000;

                            // Resolve DNS and force IPv4 to avoid IPv6 timeout on Linux
                            var addresses = await Dns.GetHostAddressesAsync("smtp.gmail.com");
                            var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                            if (ipv4 != null)
                            {
                                // Create TCP socket to IPv4 address, then hand to MailKit with hostname for SSL validation
                                var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                                await socket.ConnectAsync(ipv4, 465);
                                await smtpClient.ConnectAsync(socket, "smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                            }
                            else
                            {
                                await smtpClient.ConnectAsync("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                            }

                            await smtpClient.AuthenticateAsync("noreplymuneemjii@gmail.com", "bumd envm vjbn zqre");
                            await smtpClient.SendAsync(message);
                            await smtpClient.DisconnectAsync(true);
                            break; // success
                        }
                        catch (Exception emailEx)
                        {
                            await _errorLogService.LogErrorAsync(
                                $"User invite email attempt {attempt}/{maxRetries} failed for: {model.PhoneOrEmail} - {emailEx.Message}", emailEx.StackTrace);
                            if (attempt < maxRetries)
                                await Task.Delay(2000 * attempt);
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

      
        public async Task<List<ModulePermissionViewModel>> GetRolePermissions(int roleId)
        {
            var modulePermissions = new List<ModulePermissionViewModel>();


            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var QueryString = "SELECT roleid, moduleid, modulename," +
                    " BOOL_OR(CASE WHEN permissionid = 1 THEN allowed ELSE false END) AS CanView ,   " +
                    " BOOL_OR(CASE WHEN permissionid = 2 THEN allowed ELSE false END) AS CanCreate  ,   " +
                    " BOOL_OR(CASE WHEN permissionid = 3 THEN allowed ELSE false END) AS CanEdit ,    " +
                    " BOOL_OR(CASE WHEN permissionid = 4 THEN allowed ELSE false END) AS CanShare ,  " +
                    "  BOOL_OR(CASE WHEN permissionid = 5 THEN allowed ELSE false END) AS CanDelete " +
                    " FROM     public.rolepermissions  ";
                   


                if (roleId>0)
                {
                    QueryString += $" where roleid = {roleId}";
                }
                QueryString += "  GROUP BY     roleid, moduleid, modulename  ORDER BY     moduleid ;";

                modulePermissions = connection.QuerySql<ModulePermissionViewModel>(QueryString).ToList();
            }

            return modulePermissions;

        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(int userId, int newRoleId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updateQuery = @"
                UPDATE businesses 
                SET roleid = @roleId, updated_at = @updatedAt 
                WHERE id = @userId";

            using var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("roleId", newRoleId);
            command.Parameters.AddWithValue("updatedAt", DateTime.Now);
            command.Parameters.AddWithValue("userId", userId);

            await command.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var updateQuery = @"
                    UPDATE businesses 
                    SET roleid = @roleId, updated_at = @updatedAt 
                    WHERE id = @userId";

                using var command = new NpgsqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("roleId", request.RoleId);
                command.Parameters.AddWithValue("updatedAt", DateTime.Now);
                command.Parameters.AddWithValue("userId", request.UserId);

                await command.ExecuteNonQueryAsync();

                return Json(new { success = true, message = "Role updated successfully!" });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"User UpdateRole Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = false, message = $"Error updating role: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreUser(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updateQuery = @"
                UPDATE businesses 
                SET status = 1, updated_at = @updatedAt 
                WHERE id = @userId";

            using var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("updatedAt", DateTime.Now);
            command.Parameters.AddWithValue("userId", userId);

            await command.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }

        private async Task<List<RoleOption>> GetAvailableRoles()
            {
            var roles = new List<RoleOption>();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = "select * from roles ";

                    roles = connection.QuerySql<RoleOption>(QueryString).ToList();
                }
            }
            catch(Exception ex)
            {
                await _errorLogService.LogErrorAsync($"User GetAvailableRoles Error: {ex.Message}", ex.StackTrace);
            }
                return roles;
        }

        public async Task<IActionResult> DeleteBusiness(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new
                    {
                        Id = id,
                        Status = -1,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var rowsAffected =  connection.ExecuteSql(@"
                UPDATE businesses 
                SET status = @Status, 
                    updated_at = @UpdatedAt
                WHERE id = @Id 
               
            ", parameters);

                  
                    return Json(new { success = true, message = "User has been Delete Sucessfully!" });

                }
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"User DeleteBusiness Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = true, message = $"Error deleting business: {ex.Message}" });

            }
        }
        public async Task<IActionResult> RestoreBusiness(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new
                    {
                        Id = id,
                        Status = 1,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var rowsAffected = connection.ExecuteSql(@"
                UPDATE businesses 
                SET status = @Status, 
                    updated_at = @UpdatedAt
                WHERE id = @Id 
               
            ", parameters);


                    return Json(new { success = true, message = "User has been Restore Sucessfully!" });

                }
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"User RestoreBusiness Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = true, message = $"Error deleting business: {ex.Message}" });

            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var model = new AddUserViewModel();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string userQuery = @"SELECT id, phone , email, roleid, username ,
                                        created_at, updated_at, Status 
                                        FROM businesses WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(userQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                model.Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]);
                                model.FullName = reader["username"] == DBNull.Value ? string.Empty : reader["username"].ToString();
                                model.PhoneOrEmail = reader["phone"] == DBNull.Value ? string.Empty : reader["phone"].ToString();
                                model.PhoneOrEmail = reader["email"] == DBNull.Value ? string.Empty : reader["email"].ToString();
                                model.SelectedRoleId = reader["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoleId"]);

                            }
                            else
                            {
                                TempData["ErrorMessage"] = "User not found.";
                                return RedirectToAction("Index");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"User EditUser GET Error: {ex.Message}", ex.StackTrace);
                return RedirectToAction("Index");
            }

            model.AvailableRoles = await GetAvailableRoles();
            model.ModulePermissions = await GetRolePermissions(model.SelectedRoleId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await GetAvailableRoles();
                model.ModulePermissions = await GetRolePermissions(model.SelectedRoleId);
                return View(model);
            }

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updateQuery = @"
                UPDATE businesses 
                SET username = @username, 
                    roleid = @roleid, 
                    updated_at = @updatedAt 
                WHERE id = @id";

            using var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("id", model.Id);
            command.Parameters.AddWithValue("username", model.FullName ?? "");
            command.Parameters.AddWithValue("roleid", model.SelectedRoleId);
            command.Parameters.AddWithValue("updatedAt", DateTime.Now);

            await command.ExecuteNonQueryAsync();

            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("Index");
        }

        public async Task<List<MUNEEMJI.Models.Business>> GetUserDropdown()
        {
            var users = new List<MUNEEMJI.Models.Business>();
            var CompanyId = _companyTenancy.GetCurrentCompanyId();

            users =  await _iuser.GetUserDropdown(CompanyId);
            
            return users;
        }
    }
}

