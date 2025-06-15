using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class BillItemController : Controller
    {
        private readonly IBillItemService _billItemService;
        private readonly ILogger<BillItemController> _logger;

        public BillItemController(IBillItemService billItemService, ILogger<BillItemController> logger)
        {
            _billItemService = billItemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = new BillItemViewModel
                {
                    BillItem = new BillItem
                    {
                        ItemType = "Product",
                        AsOfDate = DateTime.Today,
                        SalePriceTaxType = "Without Tax",
                        PurchasePriceTaxType = "Without Tax",
                        DiscountType = "Percentage",
                        TaxRate = "None"
                    },
                    Categories = await _billItemService.GetCategoriesAsync(),
                    Units = await _billItemService.GetUnitsAsync(),
                    TaxRates = await _billItemService.GetTaxRatesAsync(),
                    RawMaterials = new List<RawMaterial>
                    {
                        new RawMaterial { Id = 1 },
                        new RawMaterial { Id = 2 }
                    },
                    AdditionalCosts = new List<AdditionalCost>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create bill item page");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillItemViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set service-specific fields based on item type
                    if (model.BillItem.ItemType == "Service")
                    {
                        model.BillItem.ServiceName = model.BillItem.ItemName;
                        model.BillItem.ServiceHsn = model.BillItem.ItemHsn;
                        model.BillItem.ServiceCode = model.BillItem.ItemCode;
                    }

                    bool result = await _billItemService.SaveBillItemAsync(model);

                    if (result)
                    {
                        TempData["SuccessMessage"] = $"{model.BillItem.ItemType} saved successfully!";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to save the item. Please try again.");
                    }
                }

                // Reload dropdown data if validation fails
                model.Categories = await _billItemService.GetCategoriesAsync();
                model.Units = await _billItemService.GetUnitsAsync();
                model.TaxRates = await _billItemService.GetTaxRatesAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bill item");
                ModelState.AddModelError("", "An error occurred while saving the item.");

                // Reload dropdown data
                model.Categories = await _billItemService.GetCategoriesAsync();
                model.Units = await _billItemService.GetUnitsAsync();
                model.TaxRates = await _billItemService.GetTaxRatesAsync();

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRawMaterial([FromBody] RawMaterial rawMaterial)
        {
            try
            {
                // This would be used for AJAX calls to add raw materials dynamically
                return Json(new { success = true, data = rawMaterial });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding raw material");
                return Json(new { success = false, message = "Error adding raw material" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAdditionalCost([FromBody] AdditionalCost additionalCost)
        {
            try
            {
                // This would be used for AJAX calls to add additional costs dynamically
                return Json(new { success = true, data = additionalCost });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding additional cost");
                return Json(new { success = false, message = "Error adding additional cost" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _billItemService.GetCategoriesAsync();
                return Json(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnits()
        {
            try
            {
                var units = await _billItemService.GetUnitsAsync();
                return Json(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting units");
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxRates()
        {
            try
            {
                var taxRates = await _billItemService.GetTaxRatesAsync();
                return Json(taxRates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tax rates");
                return Json(new List<string>());
            }
        }
        // In your Controller (e.g., ProductController.cs)
        [HttpGet]
        public IActionResult StockAdjustment()
        {
            string itemName = "demo";
            var model = new StockAdjustmentViewModel
            {
                ItemName = itemName,
                AdjustmentDate = DateTime.Now,
                Godown = "Main Godown",
                Unit = "Btl"
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult SaveStockAdjustment(StockAdjustmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the stock adjustment to database
                // Your save logic here

                // Redirect back to products page or show success message
                return RedirectToAction("Products", "Home");
            }

            return View("Index", model);
        }
    }
}
