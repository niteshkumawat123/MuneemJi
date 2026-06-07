using Insight.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MUNEEMJI.Services;
using Npgsql;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

namespace MUNEEMJI.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IErrorLogService _errorLogService;

        public CompanyController(IErrorLogService errorLogService)
        {
            _errorLogService = errorLogService;
        }
        public async Task<IActionResult> Index()
        {
            var email = HttpContext.Session.GetString("Email");
            var businesses = new MUNEEMJI.Models.CompanySharedModel
            {
                CompanySharedWithMe = new List<MUNEEMJI.Models.Business>(),
                MyCompany = new List<MUNEEMJI.Models.Business>()
            };

            if (string.IsNullOrEmpty(email))
            {
                return View(businesses); // no email in session
            }

            using var connection = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString);
            await connection.OpenAsync();

            // ---------------- Query 1: Businesses shared with me (by email) ----------------
            var query = @"SELECT id, business_name, phone, email, created_at, updated_at, status, roleid, isactive, companyid, username 
                  FROM businesses 
                  WHERE email = @Email and isowner = false and isactive = true";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("Email", email);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    businesses.CompanySharedWithMe.Add(new MUNEEMJI.Models.Business
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                        BusinessName = reader.IsDBNull(reader.GetOrdinal("business_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("business_name")),
                        Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? string.Empty : reader.GetString(reader.GetOrdinal("phone")),
                        Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("created_at")),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("updated_at")),
                        Status = reader.IsDBNull(reader.GetOrdinal("status")) ? 0 : reader.GetInt32(reader.GetOrdinal("status")),
                        RoleId = reader.IsDBNull(reader.GetOrdinal("roleid")) ? 0 : reader.GetInt32(reader.GetOrdinal("roleid")),
                        IsActive = reader.IsDBNull(reader.GetOrdinal("isactive")) ? false : reader.GetBoolean(reader.GetOrdinal("isactive")),
                        CompanyId = reader.IsDBNull(reader.GetOrdinal("companyid")) ? 0 : reader.GetInt32(reader.GetOrdinal("companyid")),
                        UserName = reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username"))
                    });
                }
            }

            // ---------------- Query 2: My company businesses (by companyid) ----------------
            var query1 = @"SELECT id, business_name, phone, email, created_at, updated_at, status, roleid, isactive, companyid, username 
                   FROM businesses 
                    WHERE email = @Email and isowner = true  and isactive = true";

            var businessIdString = HttpContext.Session.GetString("BusinessId");
            if (!string.IsNullOrEmpty(businessIdString) && int.TryParse(businessIdString, out int companyId))
            {
                using var command1 = new NpgsqlCommand(query1, connection);
                command1.Parameters.AddWithValue("Email", email);

                using var reader1 = await command1.ExecuteReaderAsync();
                while (await reader1.ReadAsync())
                {
                    businesses.MyCompany.Add(new MUNEEMJI.Models.Business
                    {
                        Id = reader1.IsDBNull(reader1.GetOrdinal("id")) ? 0 : reader1.GetInt32(reader1.GetOrdinal("id")),
                        BusinessName = reader1.IsDBNull(reader1.GetOrdinal("business_name")) ? string.Empty : reader1.GetString(reader1.GetOrdinal("business_name")),
                        Phone = reader1.IsDBNull(reader1.GetOrdinal("phone")) ? string.Empty : reader1.GetString(reader1.GetOrdinal("phone")),
                        Email = reader1.IsDBNull(reader1.GetOrdinal("email")) ? string.Empty : reader1.GetString(reader1.GetOrdinal("email")),
                        CreatedAt = reader1.IsDBNull(reader1.GetOrdinal("created_at")) ? DateTime.MinValue : reader1.GetDateTime(reader1.GetOrdinal("created_at")),
                        UpdatedAt = reader1.IsDBNull(reader1.GetOrdinal("updated_at")) ? DateTime.MinValue : reader1.GetDateTime(reader1.GetOrdinal("updated_at")),
                        Status = reader1.IsDBNull(reader1.GetOrdinal("status")) ? 0 : reader1.GetInt32(reader1.GetOrdinal("status")),
                        RoleId = reader1.IsDBNull(reader1.GetOrdinal("roleid")) ? 0 : reader1.GetInt32(reader1.GetOrdinal("roleid")),
                        IsActive = reader1.IsDBNull(reader1.GetOrdinal("isactive")) ? false : reader1.GetBoolean(reader1.GetOrdinal("isactive")),
                        CompanyId = reader1.IsDBNull(reader1.GetOrdinal("companyid")) ? 0 : reader1.GetInt32(reader1.GetOrdinal("companyid")),
                        UserName = reader1.IsDBNull(reader1.GetOrdinal("username")) ? string.Empty : reader1.GetString(reader1.GetOrdinal("username"))
                    });
                }
            }

            return View(businesses);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int Id)
        {
            try
            {
                var connection = MUNEEMJI.DbConfig.ConnectionString;
                using (var conn = new NpgsqlConnection(connection))
                {
                    if (Id > 0)
                    {
                        conn.ExecuteSql("update businesses set isactive = false where id = @p_id", new { p_id = Id });
                        return Ok(new { success = true });
                    }
                    return BadRequest(new { success = false, message = "Invalid company ID" });
                }
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Company DeleteCompany Error: {ex.Message}", ex.StackTrace);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class DeleteCompanyRequest
        {
            public int Id { get; set; }
        }

        // ?? Create Company (under logged-in email) ??
        [HttpGet]
        public IActionResult CreateCompany()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(string businessName, string phone, string email)
        {
            var sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login", "Account");

            // Use provided email or fall back to session email
            var companyEmail = string.IsNullOrWhiteSpace(email) ? sessionEmail : email.Trim();

            if (string.IsNullOrWhiteSpace(businessName))
            {
                ViewBag.Email = sessionEmail;
                ViewBag.Error = "Business name is required.";
                return View();
            }

            // Verify OTP was completed for this email
            var verifiedEmail = HttpContext.Session.GetString("CompanyOtpVerifiedEmail");
            if (string.IsNullOrEmpty(verifiedEmail) || !string.Equals(verifiedEmail, companyEmail, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Email = sessionEmail;
                ViewBag.Error = "Please verify your email with OTP before creating the company.";
                return View();
            }

            try
            {
                using var connection = new NpgsqlConnection(DbConfig.ConnectionString);
                await connection.OpenAsync();

                var insertQuery = @"
                    INSERT INTO businesses 
                    (business_name, phone, email, created_at, updated_at, status, roleid, isactive, companyid, username, isowner) 
                    VALUES (@BusinessName, @Phone, @Email, @CreatedAt, @UpdatedAt, 1, 1, true, 0, '', true)
                    RETURNING id";

                using var insertCmd = new NpgsqlCommand(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@BusinessName", businessName);
                insertCmd.Parameters.AddWithValue("@Phone", phone ?? "");
                insertCmd.Parameters.AddWithValue("@Email", companyEmail);
                insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                insertCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                var newId = await insertCmd.ExecuteScalarAsync();
                if (newId != null)
                {
                    int id = (int)newId;

                    // Set companyid = own id for owner businesses
                    using var updateCmd = new NpgsqlCommand("UPDATE businesses SET companyid = @cid WHERE id = @id", connection);
                    updateCmd.Parameters.AddWithValue("@cid", id);
                    updateCmd.Parameters.AddWithValue("@id", id);
                    await updateCmd.ExecuteNonQueryAsync();

                    // Also create business_profile row
                    using var profCmd = new NpgsqlCommand(
                        @"INSERT INTO public.business_profiles (business_name, phone_number, email, businessesid) 
                          VALUES (@biz, @ph, @em, @bid)", connection);
                    profCmd.Parameters.AddWithValue("biz", businessName);
                    profCmd.Parameters.AddWithValue("ph", phone ?? "");
                    profCmd.Parameters.AddWithValue("em", companyEmail);
                    profCmd.Parameters.AddWithValue("bid", id);
                    await profCmd.ExecuteNonQueryAsync();
                }

                // Clear OTP verification session
                HttpContext.Session.Remove("CompanyOtp");
                HttpContext.Session.Remove("CompanyOtpEmail");
                HttpContext.Session.Remove("CompanyOtpGeneratedAt");
                HttpContext.Session.Remove("CompanyOtpVerifiedEmail");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Company CreateCompany Error for email: {companyEmail} - {ex.Message}", ex.StackTrace);
                ViewBag.Email = sessionEmail;
                ViewBag.Error = "Failed to create company: " + ex.Message;
                return View();
            }
        }

        // ?? Send OTP for company creation ??
        [HttpPost]
        public async Task<IActionResult> SendCompanyOtp([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
                return Json(new { success = false, message = "Email is required." });

            try
            {
                var otp = new Random().Next(100000, 999999).ToString();

                HttpContext.Session.SetString("CompanyOtp", otp);
                HttpContext.Session.SetString("CompanyOtpEmail", request.Email.Trim());
                HttpContext.Session.SetString("CompanyOtpGeneratedAt", DateTime.Now.ToString());
                HttpContext.Session.Remove("CompanyOtpVerifiedEmail");

                await SendEmailAsync(request.Email.Trim(), "MuneemJi - Company Verification OTP",
                    $@"<div style='font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:0 auto;padding:32px;'>
                        <h2 style='color:#1a202c;margin-bottom:8px;'>Verify Your Email</h2>
                        <p style='color:#718096;font-size:14px;'>Use the following OTP to verify your email for creating a new company on MuneemJi:</p>
                        <div style='background:#f7fafc;border:2px solid #e2e8f0;border-radius:12px;padding:24px;text-align:center;margin:24px 0;'>
                            <span style='font-size:32px;font-weight:700;letter-spacing:8px;color:#e53e3e;'>{otp}</span>
                        </div>
                        <p style='color:#a0aec0;font-size:12px;'>This OTP is valid for 5 minutes. Do not share it with anyone.</p>
                    </div>");

                return Json(new { success = true, message = "OTP sent to " + request.Email.Trim() });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Company SendCompanyOtp Error for email: {request.Email} - {ex.Message}", ex.StackTrace);
                return Json(new { success = false, message = "Failed to send OTP: " + ex.Message });
            }
        }

        // ?? Verify OTP for company creation ??
        [HttpPost]
        public IActionResult VerifyCompanyOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Otp) || string.IsNullOrWhiteSpace(request?.Email))
                return Json(new { success = false, message = "OTP and email are required." });

            var storedOtp = HttpContext.Session.GetString("CompanyOtp");
            var storedEmail = HttpContext.Session.GetString("CompanyOtpEmail");
            var otpTime = HttpContext.Session.GetString("CompanyOtpGeneratedAt");

            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(storedEmail))
                return Json(new { success = false, message = "OTP expired. Please request a new one." });

            // Check expiry (5 minutes)
            if (DateTime.TryParse(otpTime, out DateTime generated) && DateTime.Now.Subtract(generated).TotalMinutes > 5)
            {
                HttpContext.Session.Remove("CompanyOtp");
                HttpContext.Session.Remove("CompanyOtpEmail");
                HttpContext.Session.Remove("CompanyOtpGeneratedAt");
                return Json(new { success = false, message = "OTP has expired. Please request a new one." });
            }

            if (!string.Equals(storedEmail, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                return Json(new { success = false, message = "Email mismatch. Please re-send OTP." });

            if (storedOtp != request.Otp.Trim())
                return Json(new { success = false, message = "Invalid OTP. Please try again." });

            // Mark email as verified
            HttpContext.Session.SetString("CompanyOtpVerifiedEmail", request.Email.Trim());
            HttpContext.Session.Remove("CompanyOtp");

            return Json(new { success = true, message = "Email verified successfully!" });
        }

        public class SendOtpRequest
        {
            public string Email { get; set; }
        }

        public class VerifyOtpRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            int maxRetries = 3;
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("MuneemJiApp", "noreplymuneemjii@gmail.com"));
                    message.To.Add(new MailboxAddress("", to));
                    message.Subject = subject;
                    message.Body = new TextPart("html") { Text = body };

                    using var client = new MailKit.Net.Smtp.SmtpClient();
                    client.Timeout = 30000; // 30 seconds

                    // Resolve DNS and force IPv4 to avoid IPv6 timeout on Linux
                    var addresses = await Dns.GetHostAddressesAsync("smtp.gmail.com");
                    var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                    if (ipv4 != null)
                    {
                        // Create a TCP socket connected to the IPv4 address
                        var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                        await socket.ConnectAsync(ipv4, 465);

                        // Pass the connected socket to MailKit with the hostname for SSL certificate validation
                        await client.ConnectAsync(socket, "smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    }
                    else
                    {
                        await client.ConnectAsync("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    }

                    await client.AuthenticateAsync("noreplymuneemjii@gmail.com", "bumd envm vjbn zqre");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return; // success
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    await _errorLogService.LogErrorAsync(
                        $"Company SendEmailAsync attempt {attempt}/{maxRetries} failed for: {to} - {ex.Message}", ex.StackTrace);

                    if (attempt < maxRetries)
                    {
                        await Task.Delay(2000 * attempt);
                    }
                }
            }

            throw new Exception($"Failed to send email after {maxRetries} attempts: {lastException?.Message}", lastException);
        }

        // ?? Switch to a different company ??
        [HttpPost]
        public async Task<IActionResult> SwitchCompany(int companyId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            try
            {
                using var connection = new NpgsqlConnection(DbConfig.ConnectionString);
                await connection.OpenAsync();

                var query = @"SELECT id, business_name, phone, email, isowner, companyid, roleid
                              FROM businesses 
                              WHERE id = @Id AND email = @Email AND isactive = true
                              LIMIT 1";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", companyId);
                cmd.Parameters.AddWithValue("@Email", email);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var bizName = reader.GetString(reader.GetOrdinal("business_name"));
                    var phone = reader.GetString(reader.GetOrdinal("phone"));
                    var isOwner = reader.GetBoolean(reader.GetOrdinal("isowner"));
                    var cid = reader.GetInt32(reader.GetOrdinal("companyid"));
                    var roleId = reader.IsDBNull(reader.GetOrdinal("roleid")) ? 0 : reader.GetInt32(reader.GetOrdinal("roleid"));

                    int resolvedCompanyId = isOwner ? companyId : cid;

                    await reader.CloseAsync();

                    // Re-create auth cookie with new company context
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, resolvedCompanyId.ToString()),
                        new Claim(ClaimTypes.Name, bizName),
                        new Claim(ClaimTypes.Email, email),
                        new Claim("Phone", phone),
                        new Claim("CompanyId", resolvedCompanyId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddYears(10) });

                    HttpContext.Session.SetString("BusinessId", resolvedCompanyId.ToString());
                    HttpContext.Session.SetString("BusinessName", bizName);
                    HttpContext.Session.SetString("Phone", phone);
                    HttpContext.Session.SetString("Email", email);
                    HttpContext.Session.SetString("RoleId", roleId.ToString());
                    HttpContext.Session.SetString("IsOwner", isOwner.ToString());

                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Company SwitchCompany Error for companyId: {companyId} - {ex.Message}", ex.StackTrace);
                return RedirectToAction("Index");
            }
        }
    }
}
