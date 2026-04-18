using Insight.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class CompanyController : Controller
    {
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
        public IActionResult DeleteCompany(int Id)
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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class DeleteCompanyRequest
        {
            public int Id { get; set; }
        }
    }
}
