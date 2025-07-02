using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using Npgsql;
using System.Transactions;
using Insight.Database;


namespace MUNEEMJI.Controllers
{
    public class BillItemController : Controller
    {
        private readonly IBillItemService _billItemService;
        private readonly ILogger<BillItemController> _logger;
        string _connectionString = string.Empty;


        public BillItemController(IBillItemService billItemService, ILogger<BillItemController> logger)
        {
            _billItemService = billItemService;
            _logger = logger;
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
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
        public async Task<IActionResult> Create([FromBody]BillItem model)
        {
            BillItemViewModel viewModel = new BillItemViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    // Set service-specific fields based on item type
                    if (model.ItemType == "Service")
                    {
                        model.ServiceName = model.ItemName;
                        model.ServiceHsn =  model.ItemHsn;
                        model.ServiceCode = model.ItemCode;
                    }

                    bool result = await _billItemService.SaveBillItemAsync(model);

                    if (result)
                    {
                        TempData["SuccessMessage"] = $"{model.ItemType} saved successfully!";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to save the item. Please try again.");
                    }
                }
                

                viewModel.BillItem = model;
                // Reload dropdown data if validation fails
                viewModel.Categories = await _billItemService.GetCategoriesAsync();
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bill item");
                ModelState.AddModelError("", "An error occurred while saving the item.");

                // Reload dropdown data
                viewModel.Categories = await _billItemService.GetCategoriesAsync();
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                return View(viewModel);
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

        public async Task<List<BillItem>> GetBillItemsAsync()
        {
            List<BillItem> items = new List<BillItem>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ✅ Query to get all bill items
            var billItemSql = @"
        SELECT 
            id AS ""Id"",
            item_type AS ""ItemType"",
            item_name AS ""ItemName"",
            item_hsn AS ""ItemHsn"",
            item_code AS ""ItemCode"",
            category AS ""Category"",
            unit AS ""Unit"",
            item_image_url AS ""ItemImageUrl"",
            sale_price AS ""SalePrice"",
            sale_price_tax_type AS ""SalePriceTaxType"",
            discount_on_sale_price AS ""DiscountOnSalePrice"",
            discount_type AS ""DiscountType"",
            purchase_price AS ""PurchasePrice"",
            purchase_price_tax_type AS ""PurchasePriceTaxType"",
            tax_rate AS ""TaxRate"",
            wholesale_price AS ""WholesalePrice"",
            opening_quantity AS ""OpeningQuantity"",
            at_price AS ""AtPrice"",
            as_of_date AS ""AsOfDate"",
            location AS ""Location"",
            min_stock_to_maintain AS ""MinStockToMaintain"",
            online_store_price AS ""OnlineStorePrice"",
            description AS ""Description"",
            raw_materials AS ""RawMaterials"",
            additional_costs AS ""AdditionalCosts"",
            total_estimated_cost AS ""TotalEstimatedCost"",
            service_name AS ""ServiceName"",
            service_hsn AS ""ServiceHsn"",
            service_code AS ""ServiceCode"",
            created_at AS ""CreatedAt"",
            updated_at AS ""UpdatedAt""
        FROM billitem
        ORDER BY id;
    ";

            // ✅ Fetch bill items
            var billItems = connection.QuerySql<BillItem>(billItemSql).ToList();

            if (billItems != null && billItems.Count > 0)
            {
                foreach (var billItem in billItems)
                {
                    // ✅ Query to fetch manufacturing data for each bill item
                    var manufacturingSql = @"
                SELECT 
                    id AS ""Id"",
                    itembillingid AS ""ItemBillingId"",
                    name AS ""Name"",
                    quantity AS ""Quantity"",
                    unit AS ""Unit"",
                    purchasepriceperunit AS ""PurchasePricePerUnit"",
                    estimatedcost AS ""EstimatedCost""
                FROM manufacturing
                WHERE itembillingid = @_itembillingid;
            ";

                    var manufacturing = connection
                        .QuerySql<RawMaterial>(manufacturingSql, new { _itembillingid = billItem.Id })
                        .ToList();

                    billItem.Manufacturing = manufacturing;
                    items.Add(billItem);
                }
            }

            return items;
        }


        public async Task<IActionResult> Index(int? id)
        {
            string godown = "All Godowns"; string search = ""; string sortBy = "date"; string sortDirection = "desc";
            try
            {


                var viewModel = await GetBillItemsAsync();

                ItemViewModel itemViewModel = new ItemViewModel();
                itemViewModel.ItemView = viewModel;
                if (id > 0)
                {
                    itemViewModel.SelectedItem = new BillItem();
                    itemViewModel.SelectedItem = viewModel.Where(x => x.Id == id).FirstOrDefault();
                }
                else {

                    itemViewModel.SelectedItem = new BillItem();
                    itemViewModel.SelectedItem = viewModel.FirstOrDefault();

                }
                //MockInventoryService mock = new MockInventoryService();
                //var viewModel = new InventoryViewModel
                //{
                //    Product = GetProductById("demo"),
                //    Transactions = mock.GetTransactions("demo", search, sortBy, sortDirection),
                //    Godowns = mock.GetGodowns(),
                //    SelectedGodown = godown
                //};

                ////if (Request.IsAjaxRequest())
                ////{
                ////    return PartialView("_TransactionsPartial", viewModel);
                ////}

                //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                //{
                //    return PartialView("_TransactionsPartial", viewModel);
                //}

                return View(itemViewModel);
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
