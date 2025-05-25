using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Repositories;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Controllers
{
    public class UnitsController : Controller
    {
        [HttpPost]
        public IActionResult AddUnit([FromForm] Unit model)
        {
            UnitDataAccess.AddUnit(model);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteUnit(int id)
        {
            UnitDataAccess.DeleteUnit(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult AddConversion(int fromUnitId, int toUnitId, decimal factor)
        {
            UnitDataAccess.AddConversion(fromUnitId, toUnitId, factor);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteConversion(int id)
        {
            UnitDataAccess.DeleteConversion(id);
            return Ok();
        }
    }
}
