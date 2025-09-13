using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        [HttpPost]
        public IActionResult Add(string name)
        {
            CategoryDataAccess.Add(name);
            return Ok();
        }

        [HttpPost]
        public IActionResult MoveProduct(int productId, int categoryId)
        {
            int? newCatId = (categoryId == 0 ? (int?)null : categoryId);
            ProductDataAccess.UpdateCategory(productId, newCatId);
            return Ok();
        }

        public List<CategoryDropdownModel> GetCategoriesDropdown()
        {
            var categories = new List<CategoryDropdownModel>();

            using (var conn = new NpgsqlConnection("Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser"))
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

       

    }
}
