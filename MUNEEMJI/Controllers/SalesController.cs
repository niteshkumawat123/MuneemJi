using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class SalesController : Controller
    {
        

        // Display create form
        [HttpGet]
        public IActionResult SalesEntry()
        {
            var sale = new Sale();
            // Initialize with one empty item for the form
            sale.Items.Add(new SaleItem());

            // Load dropdown lists into ViewBag
            ViewBag.Items = SalesRepository.GetItems();
            ViewBag.Units = SalesRepository.GetUnits();
            ViewBag.Taxes = SalesRepository.GetTaxes();
            // (Optionally load State list similarly if implemented)

            return View(sale);
        }

        // Handle form post
        [HttpPost]
        public IActionResult SalesEntry(Sale model)
        {
            if (ModelState.IsValid)
            {
                // Save sale and its items
                SalesRepository.CreateSale(model);
                // Redirect to a "Get" action or another confirmation page
                return RedirectToAction("Get", new { id = model.SaleId });
            }
            // If validation fails, reload dropdowns and return view
            ViewBag.Items = SalesRepository.GetItems();
            ViewBag.Units = SalesRepository.GetUnits();
            ViewBag.Taxes = SalesRepository.GetTaxes();
            return View(model);
        }

        // Display saved sale details
        [HttpGet]
        public IActionResult Get(int id)
        {
            var sale = SalesRepository.GetSale(id);
            // Could use a different view or the same, here using a "Get" view for simplicity
            return View(sale);
        }

    }
}
