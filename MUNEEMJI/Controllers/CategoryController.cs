using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {

       

        public List<CategoryDropdownModel> GetCategoriesDropdown()
        {
            var categories = new List<CategoryDropdownModel>();

            using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
            {
                conn.Open();
                string query = @"SELECT id, name 
                             FROM categorieses 
                             ";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategoryDropdownModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return categories;
        }

        public async Task<IActionResult> DeleteCategory(int id )
        {
            try
            {
                await Task.Delay(1);
                using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                {

                    string query = " delete from categorieses where  id =@p_id ";

                    conn.ExecuteSql(query, new { p_id = id });


                }
                return Json(new { success = true, message = "Category has been deleted successfully!" });

            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message});

            }

        }



    }
}
