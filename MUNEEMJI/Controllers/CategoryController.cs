using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
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
    }
}
