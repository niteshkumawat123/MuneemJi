using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;
using System.Data;
using System.Net.Mail;
using System.Net;
using MUNEEMJI.Services;

namespace MUNEEMJI.Controllers
{
    public class UserController: Controller
    {
        private readonly string _connectionString;
        private ICompanyTenancy _companyTenancy;

        public UserController(IConfiguration configuration, ICompanyTenancy companyTenancy)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            _companyTenancy = companyTenancy;
        }

        public async Task<IActionResult> Index()
        {
            var users = new List<MUNEEMJI.Models.Business>();
            var CompanyId = _companyTenancy.GetCurrentCompanyId();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id, business_name, phone, email, status, roleid, created_at, updated_at
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
                    UpdatedAt = reader["updated_at"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("updated_at")) : DateTime.MinValue
                });
            }

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var viewModel = new AddUserViewModel
            {
                AvailableRoles = await GetAvailableRoles(),
                ModulePermissions = new List<ModulePermissionViewModel>()
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

            // 🔹 Step 1: Check if user already exists
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
                    // User already exists → return with error message
                    ModelState.AddModelError("", "A user with this email already exists for this business.");
                    model.AvailableRoles = await GetAvailableRoles();
                    return View(model);
                }
            }

            // 🔹 Step 2: Insert new user if not exists
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

                    // Send email
                    using var smtp = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("noreplymuneemjii@gmail.com", "bumd envm vjbn zqre"),
                        EnableSsl = true,
                    };

                    var mail = new MailMessage("noreplymuneemjii@gmail.com", model.PhoneOrEmail)
                    {
                        Subject = "You have been invited to join MunnemJi",
                        Body = emailBody,
                        IsBodyHtml = true
                    };

                    await smtp.SendMailAsync(mail);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var modulePermissions = new List<ModulePermissionViewModel>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                        m.moduleid,
                        m.modulename,
                        (MAX(CASE WHEN p.permissionname = 'VIEW'  THEN (rp.allowed::int) ELSE 0 END) = 1) as can_view,
                        (MAX(CASE WHEN p.permissionname = 'CREATE' THEN (rp.allowed::int) ELSE 0 END) = 1) as can_create,
                        (MAX(CASE WHEN p.permissionname = 'EDIT'   THEN (rp.allowed::int) ELSE 0 END) = 1) as can_edit,
                        (MAX(CASE WHEN p.permissionname = 'SHARE'  THEN (rp.allowed::int) ELSE 0 END) = 1) as can_share,
                        (MAX(CASE WHEN p.permissionname = 'DELETE' THEN (rp.allowed::int) ELSE 0 END) = 1) as can_delete
                    FROM modules m
                    LEFT JOIN rolepermissions rp ON m.moduleid = rp.moduleid AND rp.roleid = @roleId
                    LEFT JOIN permissions p ON rp.permissionid = p.permissionid
                    GROUP BY m.moduleid, m.modulename
                    ORDER BY m.modulename;";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("roleId", roleId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                modulePermissions.Add(new ModulePermissionViewModel
                {
                    ModuleId = reader.GetInt32("moduleid"),
                    ModuleName = reader.GetString("modulename"),
                    CanView = reader.GetBoolean("can_view"),
                    CanCreate = reader.GetBoolean("can_create"),
                    CanEdit = reader.GetBoolean("can_edit"),
                    CanShare = reader.GetBoolean("can_share"),
                    CanDelete = reader.GetBoolean("can_delete")
                });
            }

            return Json(modulePermissions);
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

            // Since you don't have a roles table, I'm creating predefined roles
            // You can modify this to fetch from a roles table if you create one
            roles.Add(new RoleOption { RoleId = 1, RoleName = "Secondary Admin" });
            roles.Add(new RoleOption { RoleId = 2, RoleName = "Salesman" });
            roles.Add(new RoleOption { RoleId = 3, RoleName = "Biller" });
            roles.Add(new RoleOption { RoleId = 4, RoleName = "Biller and Salesman" });
            roles.Add(new RoleOption { RoleId = 5, RoleName = "CA/Accountant" });
            roles.Add(new RoleOption { RoleId = 6, RoleName = "Stock Keeper" });

            return roles;
        }
    }
}

