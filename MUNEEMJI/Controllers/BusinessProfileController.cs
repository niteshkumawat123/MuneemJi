using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MUNEEMJI.Models;
using Npgsql;
using NuGet.Protocol.Plugins;
using System;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class BusinessProfileController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private static string connString = MUNEEMJI.DbConfig.ConnectionString;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BusinessProfileController( IWebHostEnvironment env, IWebHostEnvironment webHostEnvironment)
        {
            _env = env;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            BusinessProfileModel model = new BusinessProfileModel();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var Id = Convert.ToInt32(HttpContext.Session.GetString("BusinessId"));

                // Get profile
                using (var cmd = new NpgsqlCommand($"SELECT * FROM business_profiles WHERE businessesid = {Id}", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0;
                        model.BusinessName = reader["business_name"] != DBNull.Value ? reader["business_name"].ToString() : string.Empty;
                        model.PhoneNumber = reader["phone_number"] != DBNull.Value ? reader["phone_number"].ToString() : string.Empty;
                        model.Gstin = reader["gstin"] != DBNull.Value ? reader["gstin"].ToString() : string.Empty;
                        model.Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : string.Empty;
                        model.BusinessTypeId = reader["business_type_id"] != DBNull.Value ? Convert.ToInt32(reader["business_type_id"]) : 0;
                        model.BusinessCategoryId = reader["business_category_id"] != DBNull.Value ? Convert.ToInt32(reader["business_category_id"]) : 0;
                        model.StateId = reader["state_id"] != DBNull.Value ? Convert.ToInt32(reader["state_id"]) : 0;
                        model.Pincode = reader["pincode"] != DBNull.Value ? reader["pincode"].ToString() : string.Empty;
                        model.Address = reader["address"] != DBNull.Value ? reader["address"].ToString() : string.Empty;
                        model.LogoPath = reader["logo_path"] != DBNull.Value ? reader["logo_path"].ToString() : string.Empty;
                        model.SignaturePath = reader["signature_path"] != DBNull.Value ? reader["signature_path"].ToString() : string.Empty;
                    }
                }

                ViewBag.Types = GetDropdownList(conn, "business_types");
                ViewBag.Categories = GetDropdownList(conn, "business_categories");
                ViewBag.States = GetDropdownList(conn, "states");
            }

            return View(model);
        }

        private List<SelectListItem> GetDropdownList(NpgsqlConnection conn, string table)
        {
            var list = new List<SelectListItem>();
            using (var cmd = new NpgsqlCommand($"SELECT id, name FROM {table}", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["id"].ToString(),
                        Text = reader["name"].ToString()
                    });
                }
            }
            return list;
        }

        private string GetUploadPath(string subfolder)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", subfolder);

            // Create directory first, then log to database
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Log to database after ensuring directory exists
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO system_paths (content_root, web_root, upload_path, directory_exists) 
                       VALUES (@contentRoot, @webRoot, @uploadPath, @dirExists)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("contentRoot", _webHostEnvironment.ContentRootPath);
                        cmd.Parameters.AddWithValue("webRoot", _webHostEnvironment.WebRootPath);
                        cmd.Parameters.AddWithValue("uploadPath", uploadPath);
                        cmd.Parameters.AddWithValue("dirExists", Directory.Exists(uploadPath));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception but don't fail the upload
                // You can use your logging framework here
                Console.WriteLine($"Database logging failed: {ex.Message}");
            }

            return uploadPath;
        }
        private async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            var uploadsFolder = GetUploadPath(subfolder);

            // Generate unique filename to avoid conflicts
            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Properly await the async operation
                }

                return $"Web/uploads/{subfolder}/{fileName}";
            }
            catch (Exception)
            {
                // Clean up the file if it was partially created
               
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BusinessProfileModel model, IFormFile LogoFile, IFormFile SignatureFile, bool DeleteLogo = false, bool DeleteSignature = false)
        {
            try
            {
                // Get existing data from database to preserve current image paths
                BusinessProfileModel existingProfile = null;
                if (model.Id > 0)
                {
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        string selectSql = "SELECT logo_path, signature_path ,businessesid FROM business_profiles WHERE id = @Id";
                        using (var cmd = new NpgsqlCommand(selectSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", model.Id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    existingProfile = new BusinessProfileModel
                                    {
                                        LogoPath = reader["logo_path"] as string,
                                        SignaturePath = reader["signature_path"] as string,
                                        businessesid = reader.IsDBNull(reader.GetOrdinal("businessesid")) ? 0 : reader.GetInt32(reader.GetOrdinal("businessesid"))

                                    };
                                }
                            }
                        }
                    }
                }

                // Handle logo file upload or deletion
                if (DeleteLogo)
                {
                    // User wants to delete the logo
                    if (existingProfile?.LogoPath != null && !string.IsNullOrEmpty(existingProfile.LogoPath))
                    {
                        var oldLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, existingProfile.LogoPath.TrimStart('/'));
                        //if (File.Exists(oldLogoPath))
                        //{
                        //    File.Delete(oldLogoPath);
                        //}
                    }
                    model.LogoPath = null; // Set to null in database
                }
                else if (LogoFile != null && LogoFile.Length > 0)
                {
                    // User uploaded a new logo
                    if (existingProfile?.LogoPath != null && !string.IsNullOrEmpty(existingProfile.LogoPath))
                    {
                        var oldLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, existingProfile.LogoPath.TrimStart('/'));
                        //if (File.Exists(oldLogoPath))
                        //{
                        //    File.Delete(oldLogoPath);
                        //}
                    }
                    model.LogoPath = await SaveFileAsync(LogoFile, "logos");
                }
                else
                {
                    // Keep existing logo path if no new file is uploaded and not deleted
                    model.LogoPath = existingProfile?.LogoPath;
                }

                // Handle signature file upload or deletion
                if (DeleteSignature)
                {
                    // User wants to delete the signature
                    if (existingProfile?.SignaturePath != null && !string.IsNullOrEmpty(existingProfile.SignaturePath))
                    {
                        var oldSignaturePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProfile.SignaturePath.TrimStart('/'));
                        //if (File.Exists(oldSignaturePath))
                        //{
                        //    File.Delete(oldSignaturePath);
                        //}
                    }
                    model.SignaturePath = null; // Set to null in database
                }
                else if (SignatureFile != null && SignatureFile.Length > 0)
                {
                    // User uploaded a new signature
                    if (existingProfile?.SignaturePath != null && !string.IsNullOrEmpty(existingProfile.SignaturePath))
                    {
                        var oldSignaturePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProfile.SignaturePath.TrimStart('/'));
                        //if (File.Exists(oldSignaturePath))
                        //{
                        //    File.Delete(oldSignaturePath);
                        //}
                    }
                    model.SignaturePath = await SaveFileAsync(SignatureFile, "signatures");
                }
                else
                {
                    // Keep existing signature path if no new file is uploaded and not deleted
                    model.SignaturePath = existingProfile?.SignaturePath;
                }

                // Database operations
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string sql;
                    if (model.Id > 0)
                    {
                        // UPDATE
                        sql = @"UPDATE business_profiles 
                SET business_name=@BusinessName, phone_number=@Phone, gstin=@Gstin, email=@Email, 
                    business_type_id=@TypeId, business_category_id=@CatId, state_id=@StateId, 
                    pincode=@Pincode, address=@Address, logo_path=@LogoPath, signature_path=@SignaturePath
                WHERE id=@Id";
                    }
                    else
                    {
                        // INSERT
                        sql = @"INSERT INTO business_profiles 
                (business_name, phone_number, gstin, email, business_type_id, business_category_id, 
                 state_id, pincode, address, logo_path, signature_path) 
                VALUES 
                (@BusinessName, @Phone, @Gstin, @Email, @TypeId, @CatId, @StateId, @Pincode, 
                 @Address, @LogoPath, @SignaturePath)";
                    }

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BusinessName", model.BusinessName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", model.PhoneNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gstin", model.Gstin ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TypeId", model.BusinessTypeId);
                        cmd.Parameters.AddWithValue("@CatId", model.BusinessCategoryId);
                        cmd.Parameters.AddWithValue("@StateId", model.StateId);
                        cmd.Parameters.AddWithValue("@Pincode", model.Pincode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LogoPath", model.LogoPath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SignaturePath", model.SignaturePath ?? (object)DBNull.Value);

                        if (model.Id > 0)
                            cmd.Parameters.AddWithValue("@Id", model.Id);

                        cmd.ExecuteNonQuery();
                    }

                    string updateQuery = @"UPDATE businesses SET business_name = @business_name ,phone = @phone,email = @email  WHERE id = @Id";

                    using var updateCommand = new NpgsqlCommand(updateQuery, conn);
                    updateCommand.Parameters.AddWithValue("@business_name", model.BusinessName);
                    updateCommand.Parameters.AddWithValue("@phone", model.PhoneNumber);
                    updateCommand.Parameters.AddWithValue("@email", model.Email);
                    updateCommand.Parameters.AddWithValue("@Id", existingProfile.businessesid);
                    await updateCommand.ExecuteNonQueryAsync();

                    HttpContext.Session.Remove("BusinessName");
                    HttpContext.Session.SetString("BusinessName", model.BusinessName);

                }

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Edit");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the profile. Please try again.");
                // Log the actual error for debugging
                Console.WriteLine($"Error in Edit action: {ex.Message}");
                return View(model);
            }
        }

    }
}
