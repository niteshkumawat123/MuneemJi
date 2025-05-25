using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class ProductsController : Controller
    {
        [HttpPost]
        public IActionResult Add([FromForm] Product model)
        {
            ProductDataAccess.Add(model);
            return Ok();
        }

        [HttpPost]
        public IActionResult Update([FromForm] Product model)
        {
            ProductDataAccess.Update(model);
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            ProductDataAccess.Delete(id);
            return Ok();
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            var prod = ProductDataAccess.GetById(id);
            if (prod == null) return NotFound();
            return Json(prod);
        }
    }
}
