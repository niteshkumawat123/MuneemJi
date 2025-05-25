using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class ServicesController : Controller
    {
        [HttpPost]
        public IActionResult Add([FromForm] Service model)
        {
            ServiceDataAccess.Add(model);
            return Ok();
        }

        [HttpPost]
        public IActionResult Update([FromForm] Service model)
        {
            ServiceDataAccess.Update(model);
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            ServiceDataAccess.Delete(id);
            return Ok();
        }
    }
}
