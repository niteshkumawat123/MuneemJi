using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MUNEEMJI.Models;
using Npgsql;
using System;

namespace MUNEEMJI.Controllers
{
    public class BusinessProfileController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private static string connString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

        public BusinessProfileController( IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            BusinessProfileModel model = new BusinessProfileModel();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Get profile (assuming id=1 for demo)
                using (var cmd = new NpgsqlCommand("SELECT * FROM business_profiles WHERE id = 1", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.Id = Convert.ToInt32(reader["id"]);
                        model.BusinessName = reader["business_name"].ToString();
                        model.PhoneNumber = reader["phone_number"].ToString();
                        model.Gstin = reader["gstin"].ToString();
                        model.Email = reader["email"].ToString();
                        model.BusinessTypeId = Convert.ToInt32(reader["business_type_id"]);
                        model.BusinessCategoryId = Convert.ToInt32(reader["business_category_id"]);
                        model.StateId = Convert.ToInt32(reader["state_id"]);
                        model.Pincode = reader["pincode"].ToString();
                        model.Address = reader["address"].ToString();
                        model.LogoPath = reader["logo_path"].ToString();
                        model.SignaturePath = reader["signature_path"].ToString();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BusinessProfileModel model, IFormFile LogoFile, IFormFile SignatureFile)
        {
            // Handle file upload
            if (LogoFile != null)
            {
                var path = Path.Combine("wwwroot/uploads/logos", LogoFile.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }
                model.LogoPath = $"uploads/logos/{LogoFile.FileName}";
            }

            if (SignatureFile != null)
            {
                var path = Path.Combine("wwwroot/uploads/signatures", SignatureFile.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    SignatureFile.CopyTo(stream);
                }
                model.SignaturePath = $"uploads/signatures/{SignatureFile.FileName}";
            }

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
            }

            return RedirectToAction("Edit");
        }

    }
}
