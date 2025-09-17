using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace MUNEEMJI.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        [HttpGet]
        public IActionResult Login(bool otpSent = false, string email = null)
        {
            ViewBag.otpsent = otpSent;
            ViewBag.Email = email;
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email is required");
                return View("Login", model);
            }

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Check if user exists with this email
                var query = @"
                    SELECT id, business_name, phone, email, created_at, updated_at 
                    FROM businesses 
                    WHERE email = @Email";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", model.Email);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // User exists, generate and send OTP
                    var otp = new Random().Next(100000, 999999).ToString();

                    // Store OTP and email in session
                    HttpContext.Session.SetString("Otp", otp);
                    HttpContext.Session.SetString("Email", model.Email);
                    HttpContext.Session.SetString("OtpGeneratedAt", DateTime.Now.ToString());

                    // Send OTP to email
                    await SendEmailAsync(model.Email, "Your OTP Code", $"Your OTP is: {otp}");

                    // Redirect to login with OTP sent flag and email
                    return RedirectToAction("Login", new { otpSent = true, email = model.Email });
                }
                else
                {
                    TempData["EmailNotFound"] = "Email not found. Please register first.";
                    ModelState.AddModelError("Email", "Email not found. Please register first.");
                    return RedirectToAction("Login", new { otpSent = false, email = model.Email });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while sending OTP. Please try again.");
                return View("Login", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otp, string email)
        {
            var storedOtp = HttpContext.Session.GetString("Otp");
            var storedEmail = HttpContext.Session.GetString("Email");
            var otpGeneratedAt = HttpContext.Session.GetString("OtpGeneratedAt");

            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(storedEmail))
            {
                ViewBag.OtpError = "OTP has expired. Please request a new one.";
                ViewBag.otpsent = false;
                return View("Login");
            }

            // Check if OTP is expired (5 minutes)
            if (DateTime.TryParse(otpGeneratedAt, out DateTime generatedTime))
            {
                if (DateTime.Now.Subtract(generatedTime).TotalMinutes > 5)
                {
                    HttpContext.Session.Remove("Otp");
                    HttpContext.Session.Remove("Email");
                    HttpContext.Session.Remove("OtpGeneratedAt");
                    ViewBag.OtpError = "OTP has expired. Please request a new one.";
                    ViewBag.otpsent = false;
                    return View("Login");
                }
            }

            if (storedOtp == otp && storedEmail == email)
            {
                // OTP is valid, log in the user
                try
                {
                    using var connection = new NpgsqlConnection(_connectionString);
                    await connection.OpenAsync();
                    var query = @"
                SELECT id, business_name, phone, email, created_at, updated_at 
                FROM businesses 
                WHERE email = @Email";
                    using var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        var business = new Business
                        {
                            Id = reader.GetInt32("id"),
                            BusinessName = reader.GetString("business_name"),
                            Phone = reader.GetString("phone"),
                            Email = reader.GetString("email"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            UpdatedAt = reader.GetDateTime("updated_at")
                        };

                        // Create authentication cookie
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, business.Id.ToString()),
                    new Claim(ClaimTypes.Name, business.BusinessName),
                    new Claim(ClaimTypes.Email, business.Email),
                    new Claim("Phone", business.Phone)
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        // Store business info in session (optional, since you have claims now)
                        HttpContext.Session.SetString("BusinessId", business.Id.ToString());
                        HttpContext.Session.SetString("BusinessName", business.BusinessName);
                        HttpContext.Session.SetString("Phone", business.Phone);
                        HttpContext.Session.SetString("Email", business.Email);

                        // Clear OTP from session
                        HttpContext.Session.Remove("Otp");
                        HttpContext.Session.Remove("OtpGeneratedAt");

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.OtpError = "An error occurred during login. Please try again.";
                    ViewBag.Email = email;
                    ViewBag.otpsent = true;
                    return View("Login");
                }
            }

            // OTP is invalid
            ViewBag.OtpError = "Invalid OTP. Please try again.";
            ViewBag.Email = email;
            ViewBag.otpsent = true;
            return View("Login");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            // Clear TempData (optional but recommended)
            TempData.Clear();
            ViewData.Clear();

            // Sign out from cookie authentication (synchronous wait)
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            // Redirect to Login view or action
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            if (!model.AcceptTerms)
            {
                TempData["Error"] = "You must accept the terms and conditions";
                ViewData.ModelState.AddModelError("AcceptTerms", "You must accept the terms and conditions");
                ViewBag.ShowRegisterForm = true;
                ViewBag.RegisterError = "You must accept the terms and conditions.";
                ViewBag.BusinessName = model.BusinessName;
                ViewBag.Phone = model.Phone;
                ViewBag.Email = model.Email;
                //return View("Login", model);
                return RedirectToAction("Login", "Account");
            }

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Check if business already exists with the same phone or email
                var checkQuery = @"
                    SELECT COUNT(*) 
                    FROM businesses 
                    WHERE phone = @Phone OR email = @Email";

                using var checkCommand = new NpgsqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Phone", model.Phone);
                checkCommand.Parameters.AddWithValue("@Email", model.Email);

                var existingCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (existingCount > 0)
                {
                    // Check which field already exists for better error message
                    var detailCheckQuery = @"
                        SELECT phone, email 
                        FROM businesses 
                        WHERE phone = @Phone OR email = @Email";

                    using var detailCommand = new NpgsqlCommand(detailCheckQuery, connection);
                    detailCommand.Parameters.AddWithValue("@Phone", model.Phone);
                    detailCommand.Parameters.AddWithValue("@Email", model.Email);

                    using var reader = await detailCommand.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var existingPhone = reader.IsDBNull("phone") ? null : reader.GetString("phone");
                        var existingEmail = reader.IsDBNull("email") ? null : reader.GetString("email");

                        if (existingPhone == model.Phone)
                        {
                            TempData["Error"] = "This phone number is already registered";

                            ViewData.ModelState.AddModelError("Phone", "This phone number is already registered");
                            ViewBag.RegisterError = "This phone number is already registered.";
                            TempData["BusinessName"] = model.BusinessName;
                            TempData["Phone"] = model.Phone;
                            TempData["Email"] = model.Email;
                            TempData["ShowRegisterForm"] = true;

                        }
                        if (existingEmail == model.Email)
                        {
                            TempData["Error"] = "This email address is already registered";
                            ViewBag.RegisterError = "This email address is already registered.";
                            TempData["BusinessName"] = model.BusinessName;
                            TempData["Phone"] = model.Phone;
                            TempData["Email"] = model.Email;
                            TempData["ShowRegisterForm"] = true;

                            ViewData.ModelState.AddModelError("Email", "This email address is already registered");
                        }
                    }
                    //return View("Login", loginViewModel);

                    return RedirectToAction("Login", "Account");
                }

                // Insert new business
                var insertQuery = @"
                            INSERT INTO businesses 
                            (business_name, phone, email, created_at, updated_at, status, roleid, isactive, companyid, username, isowner) 
                            VALUES (@BusinessName, @Phone, @Email, @CreatedAt, @UpdatedAt, @Status, @RoleId, @IsActive, @CompanyId, @Username, @IsOwner)
                            RETURNING id";

                using var insertCommand = new NpgsqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@BusinessName", model.BusinessName);
                insertCommand.Parameters.AddWithValue("@Phone", model.Phone);
                insertCommand.Parameters.AddWithValue("@Email", model.Email);
                insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                insertCommand.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                insertCommand.Parameters.AddWithValue("@Status", 1);
                insertCommand.Parameters.AddWithValue("@RoleId", 1);
                insertCommand.Parameters.AddWithValue("@IsActive", true);
                insertCommand.Parameters.AddWithValue("@CompanyId", 0);
                insertCommand.Parameters.AddWithValue("@Username", "");
                insertCommand.Parameters.AddWithValue("@IsOwner", true);

                var newBusinessId = await insertCommand.ExecuteScalarAsync();

                if(newBusinessId!=null)
                {
                    string updateQuery = @"UPDATE businesses SET companyid = @CompanyId WHERE id = @Id";

                    using var updateCommand = new NpgsqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@CompanyId", (int)newBusinessId);
                    updateCommand.Parameters.AddWithValue("@Id", (int)newBusinessId);

                    await updateCommand.ExecuteNonQueryAsync();


                    string sql = @"INSERT INTO public.business_profiles
                          (business_name, phone_number, email, businessesid) 
                          VALUES (@businessName, @phoneNumber, @email, @businessesId)";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("businessName", model.BusinessName);
                        cmd.Parameters.AddWithValue("phoneNumber", model.Phone);
                        cmd.Parameters.AddWithValue("email", model.Email);
                        cmd.Parameters.AddWithValue("businessesId", newBusinessId);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (newBusinessId != null)
                {
                    ViewBag.SuccessMessage = "Registration successful! Please login with your email.";
                    return RedirectToAction("Login", "Account", new { registered = true });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create business account. Please try again.");
                    //return View("Login", model);
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                //return View("Login", model);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    Subject = subject,
                    Body = body,
                    From = new MailAddress("noreplymuneemjii@gmail.com", "MuneemJiApp"),
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                NetworkCredential networkCredential = new NetworkCredential("noreplymuneemjii@gmail.com", "bumd envm vjbn zqre");

                SmtpClient smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = networkCredential
                };

                mail.BodyEncoding = Encoding.Default;
                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Failed to send email", ex);
            }
        }
    }
}


public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    public bool AcceptTerms { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Business name is required")]
    [StringLength(100, ErrorMessage = "Business name cannot exceed 100 characters")]
    public string BusinessName { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "You must accept the terms and conditions")]
    public bool AcceptTerms { get; set; }
}

public class Business
{
    public int Id { get; set; }
    public string BusinessName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Status { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }
}

public class OtpViewModel
{
    [Required(ErrorMessage = "OTP is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
    public string Otp { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}
