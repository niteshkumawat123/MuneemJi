using Microsoft.AspNetCore.Mvc;
using static MUNEEMJI.Models.ItemModel;
using System.Globalization;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;

        public ItemsController(IWebHostEnvironment webHostEnv)
        {
            _webHostEnv = webHostEnv;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Load categories, units, taxes for dropdowns
            var categories = CategoryRepository.GetAll();
            var units = UnitRepository.GetAll();
            var taxes = TaxRepository.GetAll();
            ViewBag.Categories = categories;
            ViewBag.Units = units;
            ViewBag.Taxes = taxes;
            return View(new Item { EntryDate = DateTime.Today });
        }

        [HttpPost]
        public IActionResult Create(Item model)
        {
            // Save uploaded image file to wwwroot/images
            if (model.ItemImage != null && model.ItemImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnv.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder); // ensure directory exists
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ItemImage.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ItemImage.CopyTo(fileStream);
                }
                // Store relative path in database (e.g., "/images/filename.jpg")
                model.ImagePath = "/images/" + fileName;
            }

            // Insert item into database
            int newItemId = ItemRepository.InsertItem(model);

            // Handle wholesale price fields (if provided)
            // Attempt to parse wholesale price and minimum quantity from form
            string wholesalePriceStr = Request.Form["WholesalePrice"];
            string minQtyStr = Request.Form["MinWholesaleQty"];
            if (!string.IsNullOrWhiteSpace(wholesalePriceStr) && !string.IsNullOrWhiteSpace(minQtyStr))
            {
                if (decimal.TryParse(wholesalePriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) &&
                    int.TryParse(minQtyStr, out var minQty))
                {
                    // Save wholesale price entry linked to this item
                    ItemRepository.AddWholesalePrice(newItemId, price, minQty);
                }
            }

            // After saving, redirect or show success (here we redirect back to Create form)
            return RedirectToAction("Create");
        }
    }
}

