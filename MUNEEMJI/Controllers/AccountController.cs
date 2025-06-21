using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Controllers
{
    public class AccountController: Controller
    {
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.AcceptTerms)
            {
                ModelState.AddModelError("AcceptTerms", "You must accept the terms and conditions");
                return View(model);
            }

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Inline query to check if business exists with provided phone and email
                var query = @"
                    SELECT id, business_name, phone, email, created_at, updated_at 
                    FROM businesses 
                    WHERE phone = @Phone ";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Phone", model.Phone);

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

                    // Store business info in session or claims
                    HttpContext.Session.SetString("BusinessId", business.Id.ToString());
                    HttpContext.Session.SetString("BusinessName", business.BusinessName);
                    HttpContext.Session.SetString("Phone", business.Phone);
                    HttpContext.Session.SetString("Email", business.Email);

                    // Redirect to Home Index
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid phone number or email. Business not found.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                // Log the exception here
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.AcceptTerms)
            {
                ModelState.AddModelError("AcceptTerms", "You must accept the terms and conditions");
                return View(model);
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
                            ModelState.AddModelError("Phone", "This phone number is already registered");
                        }
                        if (existingEmail == model.Email)
                        {
                            ModelState.AddModelError("Email", "This email address is already registered");
                        }
                    }
                    return View(model);
                }

                // Insert new business
                var insertQuery = @"
            INSERT INTO businesses (business_name, phone, email, created_at, updated_at) 
            VALUES (@BusinessName, @Phone, @Email, @CreatedAt, @UpdatedAt)
            RETURNING id";

                using var insertCommand = new NpgsqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@BusinessName", model.BusinessName);
                insertCommand.Parameters.AddWithValue("@Phone", model.Phone);
                insertCommand.Parameters.AddWithValue("@Email", model.Email);
                insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                insertCommand.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                var newBusinessId = await insertCommand.ExecuteScalarAsync();

                if (newBusinessId != null)
                {
                    // Store business info in session
                    HttpContext.Session.SetString("BusinessId", newBusinessId.ToString());
                    HttpContext.Session.SetString("BusinessName", model.BusinessName);
                    HttpContext.Session.SetString("Phone", model.Phone);
                    HttpContext.Session.SetString("Email", model.Email);

                    // Redirect to Home Index after successful registration
                    ViewBag.SuccessMessage = "Registration successful! Please login with your phone number.";
                    return RedirectToAction("Login", "Account", new { registered = true });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create business account. Please try again.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                // Log the exception here - consider using ILogger
                // _logger.LogError(ex, "Error during business registration for phone: {Phone}", model.Phone);
                return View(model);
            }
        }
    }
}

