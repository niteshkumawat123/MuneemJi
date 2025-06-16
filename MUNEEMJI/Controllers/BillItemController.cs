using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using System.Transactions;

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
        //public List<TransactionRecord> GetTransactions(string productId, string search = "", string sortBy = "date", string sortDirection = "desc")
        //{
        //    var transactions = _transactions.AsQueryable();

        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        transactions = transactions.Where(t =>
        //            t.Type.Contains(search) ||
        //            t.Name.Contains(search) ||
        //            t.InvoiceRef.Contains(search));
        //    }

        //    // Apply sorting
        //    switch (sortBy.ToLower())
        //    {
        //        case "type":
        //            transactions = sortDirection == "asc" ? transactions.OrderBy(t => t.Type) : transactions.OrderByDescending(t => t.Type);
        //            break;
        //        case "name":
        //            transactions = sortDirection == "asc" ? transactions.OrderBy(t => t.Name) : transactions.OrderByDescending(t => t.Name);
        //            break;
        //        case "date":
        //        default:
        //            transactions = sortDirection == "asc" ? transactions.OrderBy(t => t.Date) : transactions.OrderByDescending(t => t.Date);
        //            break;
        //    }

        //    return transactions.ToList();
        //}


        private static List<ProductInfo> _products = new List<ProductInfo>
        {
            new ProductInfo
            {
                Name = "demo",
                SalePrice = 0.00m,
                PurchasePrice = 0.00m,
                StockQuantity = 1,
                StockValue = 1233.00m,
                IsExcludingTax = true
            }
        };
        public ProductInfo GetProductById(string productId)
        {
            return _products.FirstOrDefault(p => p.Name.Equals(productId, StringComparison.OrdinalIgnoreCase));
        }
        public class MockInventoryService
        {
            private static List<ProductInfo> _products = new List<ProductInfo>
        {
            new ProductInfo
            {
                Name = "demo",
                SalePrice = 0.00m,
                PurchasePrice = 0.00m,
                StockQuantity = 1,
                StockValue = 1233.00m,
                IsExcludingTax = true
            }
        };

            private static List<TransactionRecord> _transactions = new List<TransactionRecord>
        {
            new TransactionRecord
            {
                Id = 1,
                Type = "Purchase (Main)",
                InvoiceRef = "-",
                Name = "test",
                Date = new DateTime(2025, 6, 14),
                Quantity = "1 Btl",
                PricePerUnit = 1233.00m,
                Status = "Partial",
                StatusColor = "status-partial"
            }
        };

            public ProductInfo GetProductById(string productId)
            {
                return _products.FirstOrDefault(p => p.Name.Equals(productId, StringComparison.OrdinalIgnoreCase));
            }

            public List<TransactionRecord> GetTransactions(string productId, string search = "", string sortBy = "date", string sortDirection = "desc")
            {
                var transactions = _transactions.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    transactions = transactions.Where(t =>
                        t.Type.Contains(search) ||
                        t.Name.Contains(search) ||
                        t.InvoiceRef.Contains(search));
                }

                // Apply sorting
                switch (sortBy.ToLower())
                {
                    case "type":
                        transactions = sortDirection == "asc" ? transactions.OrderBy(t => t.Type) : transactions.OrderByDescending(t => t.Type);
                        break;
                    case "name":
                        transactions = sortDirection == "asc" ? transactions.OrderBy(t => t.Name) : transactions.OrderByDescending(t => t.Name);
                        break;
                    case "date":
                    default:
                        transactions = sortDirection == "asc" ? transactions.OrderBy(t => t.Date) : transactions.OrderByDescending(t => t.Date);
                        break;
                }

                return transactions.ToList();
            }

            public List<string> GetGodowns()
            {
                return new List<string> { "All Godowns", "Main Warehouse", "Secondary Storage", "Retail Store" };
            }

            public List<ProductInfo> GetAllProducts(string search = "", string godown = "All Godowns")
            {
                var products = _products.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.Name.Contains(search));
                }

                return products.ToList();
            }
            public class ServiceResult
            {
                public bool Success { get; set; }
                public string ErrorMessage { get; set; }
                public string ProductId { get; set; }
                public object Data { get; set; }
            }

            public ServiceResult AddProduct(ProductInfo product)
            {
                try
                {
                    product.Name = product.Name ?? $"Product_{Guid.NewGuid().ToString("N")[..8]}";
                    _products.Add(product);

                    return new ServiceResult { Success = true, ProductId = product.Name };
                }
                catch (Exception ex)
                {
                    return new ServiceResult { Success = false, ErrorMessage = "Failed to add product" };
                }
            }

            public ServiceResult UpdateProduct(ProductInfo product)
            {
                try
                {
                    var existingProduct = _products.FirstOrDefault(p => p.Name == product.Name);
                    if (existingProduct != null)
                    {
                        existingProduct.SalePrice = product.SalePrice;
                        existingProduct.PurchasePrice = product.PurchasePrice;
                        existingProduct.StockQuantity = product.StockQuantity;
                        existingProduct.StockValue = product.StockValue;
                        existingProduct.IsExcludingTax = product.IsExcludingTax;

                        return new ServiceResult { Success = true };
                    }

                    return new ServiceResult { Success = false, ErrorMessage = "Product not found" };
                }
                catch (Exception ex)
                {
                    return new ServiceResult { Success = false, ErrorMessage = "Failed to update product" };
                }
            }

            public ServiceResult DeleteProduct(string productId)
            {
                try
                {
                    var product = _products.FirstOrDefault(p => p.Name.Equals(productId, StringComparison.OrdinalIgnoreCase));
                    if (product != null)
                    {
                        _products.Remove(product);
                        return new ServiceResult { Success = true };
                    }

                    return new ServiceResult { Success = false, ErrorMessage = "Product not found" };
                }
                catch (Exception ex)
                {
                    return new ServiceResult { Success = false, ErrorMessage = "Failed to delete product" };
                }
            }

            public ServiceResult AdjustStock(string productId, int quantityAdjustment, string reason)
            {
                try
                {
                    var product = _products.FirstOrDefault(p => p.Name.Equals(productId, StringComparison.OrdinalIgnoreCase));
                    if (product != null)
                    {
                        product.StockQuantity += quantityAdjustment;

                        // Add adjustment transaction
                        _transactions.Add(new TransactionRecord
                        {
                            Id = _transactions.Count + 1,
                            Type = "Adjustment",
                            InvoiceRef = $"ADJ-{DateTime.Now:yyyyMMddHHmmss}",
                            Name = reason ?? "Stock Adjustment",
                            Date = DateTime.Now,
                            Quantity = $"{quantityAdjustment} Units",
                            PricePerUnit = 0,
                            Status = "Complete",
                            StatusColor = "status-complete"
                        });

                        return new ServiceResult { Success = true };
                    }

                    return new ServiceResult { Success = false, ErrorMessage = "Product not found" };
                }
                catch (Exception ex)
                {
                    return new ServiceResult { Success = false, ErrorMessage = "Failed to adjust stock" };
                }
            }

            public string ExportTransactionsToCSV(List<TransactionRecord> transactions)
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Type,Invoice/Ref,Name,Date,Quantity,Price/Unit,Status");

                foreach (var transaction in transactions)
                {
                    csv.AppendLine($"{transaction.Type},{transaction.InvoiceRef},{transaction.Name},{transaction.Date:yyyy-MM-dd},{transaction.Quantity},{transaction.PricePerUnit},{transaction.Status}");
                }

                return csv.ToString();
            }

            public byte[] ExportTransactionsToExcel(List<TransactionRecord> transactions)
            {
                // In a real implementation, you would use a library like EPPlus or ClosedXML
                // For this mock, we'll return CSV as bytes
                var csv = ExportTransactionsToCSV(transactions);
                return System.Text.Encoding.UTF8.GetBytes(csv);
            }
        }
        public ActionResult Index()
        {
            string godown = "All Godowns"; string search = ""; string sortBy = "date"; string sortDirection = "desc";
            try
            {
                MockInventoryService mock = new MockInventoryService();
                var viewModel = new InventoryViewModel
                {
                    Product = GetProductById("demo"),
                    Transactions = mock.GetTransactions("demo", search, sortBy, sortDirection),
                    Godowns = mock.GetGodowns(),
                    SelectedGodown = godown
                };

                //if (Request.IsAjaxRequest())
                //{
                //    return PartialView("_TransactionsPartial", viewModel);
                //}

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_TransactionsPartial", viewModel);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while loading the inventory data.";
                return View(new InventoryViewModel());
            }
        }
    }
}
