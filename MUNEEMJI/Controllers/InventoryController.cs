using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Returns the Products tab partial view
        public IActionResult Products()
        {
            var products = ProductDataAccess.GetAll();
            return PartialView("_ProductsPartial", products);
        }

        // Returns the Services tab partial view
        public IActionResult Services()
        {
            var services = ServiceDataAccess.GetAll();
            return PartialView("_ServicesPartial", services);
        }

        // Returns the Category tab partial view
        // Optional id parameter indicates selected category (0 for uncategorized)
        public IActionResult Category(int id = 0)
        {
            var categories = CategoryDataAccess.GetAllWithCounts();
            // If no specific id given, default to first category in list
            if (id == 0 && categories.Any())
            {
                id = categories.First().Id;
            }
            int? catId = (id == 0 ? (int?)null : id);
            var items = ProductDataAccess.GetByCategory(catId);
            var model = new CategoryViewModel
            {
                Categories = categories,
                SelectedCategoryId = id,
                Items = items
            };
            return PartialView("_CategoryPartial", model);
        }

        // Returns the Units tab partial view
        public IActionResult Units(int id = 0)
        {
            var units = UnitDataAccess.GetAll();
            if (id == 0 && units.Any())
            {
                id = units.First().Id; // Select first unit by default
            }
            var conversions = UnitDataAccess.GetConversions(id);
            var model = new UnitsViewModel
            {
                Units = units,
                SelectedUnitId = id,
                Conversions = conversions
            };
            return PartialView("_UnitsPartial", model);
        }
    }
}
