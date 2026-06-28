using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using NuGet.Protocol.Plugins;
using SkiaSharp;
using System.ComponentModel.Design;
using System.Globalization;
using ClosedXML.Excel;
using static MUNEEMJI.Models.ItemModel;
using Category = MUNEEMJI.Models.Category;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        string _connectionString = string.Empty;
        private readonly IBillItemService _billItemService;
        private readonly ICompanyTenancy _CompayTenancy;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ItemsController(IWebHostEnvironment webHostEnv, IBillItemService billItemService, ICompanyTenancy CompayTenancy, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnv = webHostEnv;
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;
            _billItemService = billItemService;
            _CompayTenancy = CompayTenancy;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public IActionResult MainIndex(int? id)
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            var viewModel = GetItemsAsync(companyId);

            ItemViewModel itemViewModel = new ItemViewModel();
            itemViewModel.ItemView = viewModel;
            if (id > 0)
            {
                itemViewModel.SelectedItem = new BillItem();
                itemViewModel.SelectedItem = viewModel.Where(x => x.Id == id).FirstOrDefault();
            }
            else
            {

                itemViewModel.SelectedItem = new BillItem();
                itemViewModel.SelectedItem = viewModel.FirstOrDefault();

            }


            return View(itemViewModel);
        }


        [HttpGet]
        public IActionResult Products(int? id)
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            var viewModel = GetItemsAsync(companyId);

            ItemViewModel itemViewModel = new ItemViewModel();
            itemViewModel.ItemView = viewModel;
            if (id > 0)
            {
                itemViewModel.SelectedItem = new BillItem();
                itemViewModel.SelectedItem = viewModel.Where(x => x.Id == id).FirstOrDefault();
            }
            else
            {

                itemViewModel.SelectedItem = new BillItem();
                itemViewModel.SelectedItem = viewModel.FirstOrDefault();

            }


            return View(itemViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id = 0, bool isview = false)
        {

            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();


                BillItem billItem = new BillItem();
                var model = new BillItemViewModel();
                if (id > 0)
                {
                    var data = GetItemsAsync(companyId);
                    if (data != null && data.Count() > 0)
                    {
                        billItem = data.Where(x => x.Id == id).FirstOrDefault();
                    }

                    // If not found in products, search in services
                    if (billItem == null || billItem.Id == 0)
                    {
                        var serviceData = GetServiceAsync(companyId);
                        if (serviceData != null && serviceData.Count() > 0)
                        {
                            billItem = serviceData.Where(x => x.Id == id).FirstOrDefault();
                        }
                    }

                }
                if (billItem != null && billItem.Id > 0)
                {
                    model = new BillItemViewModel
                    {

                        BillItem = billItem,
                        Categories = await _billItemService.GetCategoriesAsync(companyId),
                        Units = await _billItemService.GetUnitsAsync(),
                        TaxRates = await _billItemService.GetTaxRatesAsync(),
                        RawMaterials = billItem.Manufacturing,
                        AdditionalCosts = new List<AdditionalCost>()
                    };
                }
                else
                {
                    model = new BillItemViewModel
                    {

                        BillItem = new BillItem
                        {
                            ItemType = "product",
                            AsOfDate = DateTime.Today,
                            SalePriceTaxType = "Without Tax",
                            PurchasePriceTaxType = "Without Tax",
                            DiscountType = "Percentage",
                            TaxRate = "None",
                            ItemCode = ""
                        },
                        Categories = await _billItemService.GetCategoriesAsync(companyId),
                        Units = await _billItemService.GetUnitsAsync(),
                        TaxRates = await _billItemService.GetTaxRatesAsync(),
                        RawMaterials = new List<RawMaterial>
                    {
                        new RawMaterial { Id = 1 },
                        new RawMaterial { Id = 2 }
                    },
                        AdditionalCosts = new List<AdditionalCost>()
                    };
                }
                model.IsView = isview;
                return View(model);
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([FromBody] BillItem model)
        {
            BillItemViewModel viewModel = new BillItemViewModel();
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate item/service name
                    string duplicateCheckQuery = model.Id > 0
                        ? "SELECT COUNT(*) FROM billitem WHERE LOWER(TRIM(item_name)) = LOWER(TRIM(@p_name)) AND item_type = @p_itemtype AND companyid = @p_companyid AND id != @p_id"
                        : "SELECT COUNT(*) FROM billitem WHERE LOWER(TRIM(item_name)) = LOWER(TRIM(@p_name)) AND item_type = @p_itemtype AND companyid = @p_companyid";

                    using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                    {
                        conn.Open();
                        using (var checkCmd = new NpgsqlCommand(duplicateCheckQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("p_name", model.ItemName ?? "");
                            checkCmd.Parameters.AddWithValue("p_itemtype", model.ItemType ?? "product");
                            checkCmd.Parameters.AddWithValue("p_companyid", companyId);
                            if (model.Id > 0)
                                checkCmd.Parameters.AddWithValue("p_id", model.Id);

                            var count = (long)(checkCmd.ExecuteScalar() ?? 0);
                            if (count > 0)
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = $"A {model.ItemType} with this name already exists. Please use a different name.",
                                    id = model.Id
                                });
                            }
                        }
                    }

                    // Set service-specific fields based on item type
                    if (string.Equals(model.ItemType, "service", StringComparison.OrdinalIgnoreCase))
                    {
                        model.ServiceName = model.ItemName;
                        model.ServiceHsn = model.ItemHsn;
                        model.ServiceCode = model.ItemCode;
                    }
                    if (!string.IsNullOrEmpty(model.ImageBase64) && !string.IsNullOrEmpty(model.ImageFileName))
                    {
                        model.ImageUrl = await SaveImageToServer(model.ImageBase64, model.ImageFileName, "Item");
                    }
                    else if (model.Id > 0 && string.IsNullOrEmpty(model.ImageUrl))
                    {
                        // Preserve existing image when editing without uploading a new one
                        var existingItems = GetItemsAsync(companyId);
                        var existingItem = existingItems.FirstOrDefault(x => x.Id == model.Id);
                        if (existingItem == null)
                        {
                            // Also check service items
                            var serviceItems = GetServiceAsync(companyId);
                            existingItem = serviceItems.FirstOrDefault(x => x.Id == model.Id);
                        }
                        if (existingItem != null)
                        {
                            model.ImageUrl = existingItem.ImageUrl;
                        }
                    }

                    bool result = await _billItemService.SaveBillItemAsync(model, companyId);

                    if (result)
                    {
                        //TempData["SuccessMessage"] = $"{model.ItemType} saved successfully!";
                        return Json(new
                        {
                            success = true,
                            message = model.Id > 0 ? "Item updated successfully!" : "Item has been saved successfully!",
                            id = model.Id
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = model.Id > 0 ? "Item updated successfully!" : "Item has been saved successfully!",
                            id = model.Id
                        });
                    }
                }


                viewModel.BillItem = model;
                // Reload dropdown data if validation fails
                viewModel.Categories = await _billItemService.GetCategoriesAsync(companyId);
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new
                {
                    success = false,
                    message = errors.Any() ? string.Join(", ", errors) : "Validation failed. Please check your input.",
                    id = model.Id
                });
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "An error occurred while saving the item.");

                // Reload dropdown data
                viewModel.Categories = await _billItemService.GetCategoriesAsync(companyId);
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                //return View(viewModel);
                return Json(new
                {
                    success = true,
                    message = ex.Message,
                    id = model.Id
                });
            }
        }
        public List<BillItem> GetItemsAsync(int CompanyId)
        {
            var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

            List<BillItem> items = new List<BillItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {

                // ? Query to get all bill items
                var billItemSql = @"
                                    SELECT 
                                        id AS ""Id"",
                                        item_type AS ""ItemType"",
                                        item_name AS ""ItemName"",
                                        item_hsn AS ""ItemHsn"",
                                        item_code AS ""ItemCode"",
                                        category AS ""Category"",
                                        unit AS ""Unit"",
                                        item_image_url AS ""ImageUrl"",
                                        sale_price AS ""SalePrice"",
                                        sale_price_tax_type AS ""SalePriceTaxType"",
                                        discount_on_sale_price AS ""DiscountOnSalePrice"",
                                        discount_type AS ""DiscountType"",
                                        purchase_price AS ""PurchasePrice"",
                                        purchase_price_tax_type AS ""PurchasePriceTaxType"",
                                        tax_rate AS ""TaxRate"",
                                        wholesale_price AS ""WholesalePrice"",
                                        wholesale_price_tax_type AS ""WholesalePriceTaxType"",
                                        min_wholesale_qty AS ""MinWholesaleQty"",
                                        disc_on_mrp_wholesale AS ""DiscOnMrpWholesale"",
                                        additional_cess AS ""AdditionalCess"",
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
                                        colour AS ""Colour"",
                                        material AS ""Material"",
                                        mfg_date AS ""MfgDate"",
                                        exp_date AS ""ExpDate"",
                                        size AS ""Size"",
                                        brand AS ""Brand"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where  item_type = @p_itemtype and companyid = @p_companyid and (is_active = true OR is_active IS NULL)
                                    ORDER BY id;
                                    ";

                // ? Fetch bill items
                var billItems = connection.QuerySql<BillItem>(billItemSql, new { p_itemtype = "product", p_companyid = CompanyId }).ToList();

                if (billItems != null && billItems.Count > 0)
                {
                    foreach (var billItem in billItems)
                    {
                        // ? Query to fetch manufacturing data for each bill item
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
            }

            return items;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriespartialView()
        {
            ItemViewModel ViewModel = new ItemViewModel();
            ViewModel.Categories = new List<Category>();
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {


                using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"SELECT id, name 
                     FROM categorieses 
                     WHERE companyid = @p_companyid";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Add parameter value here
                        cmd.Parameters.AddWithValue("p_companyid", companyId); // ?? replace companyId with your variable

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ViewModel.Categories.Add(new Category
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }



            }
            catch (Exception ex)
            {

            }
            return PartialView("_CategoryPartial", ViewModel);
        }

        [HttpGet]
        public async Task<IActionResult>Category()
        {
            ItemViewModel ViewModel = new ItemViewModel();
            ViewModel.Categories = new List<Category>();
            ViewModel.ItemView = new List<BillItem>();
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {
                await Task.Delay(1);

                using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Load categories with item counts
                    string query = @"SELECT c.id, c.name, 
                        COALESCE((SELECT COUNT(*) FROM billitem b WHERE b.companyid = @p_companyid AND b.category = CAST(c.id AS TEXT)), 0) as item_count
                     FROM categorieses c
                     WHERE c.companyid = @p_companyid";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ViewModel.Categories.Add(new Category
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    itemCount = reader.GetInt64(2)
                                });
                            }
                        }
                    }

                    // Load uncategorized items
                    string uncatQuery = @"SELECT id, item_name, opening_quantity, online_store_price 
                        FROM billitem 
                        WHERE companyid = @p_companyid 
                          AND (category IS NULL OR TRIM(category) = '')
                        ORDER BY item_name";

                    using (var cmd2 = new NpgsqlCommand(uncatQuery, conn))
                    {
                        cmd2.Parameters.AddWithValue("p_companyid", companyId);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                ViewModel.ItemView.Add(new BillItem
                                {
                                    Id = reader2.GetInt32(0),
                                    ItemName = reader2.IsDBNull(1) ? "" : reader2.GetString(1),
                                    OpeningQuantity = reader2.IsDBNull(2) ? 0 : reader2.GetInt32(2),
                                    OnlineStorePrice = reader2.IsDBNull(3) ? 0m : reader2.GetDecimal(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(ViewModel);

        }



        public IActionResult CategoryCreate([FromBody] CategoryDropdownModel model)
        {
            var _dbconnectionstrig = MUNEEMJI.DbConfig.ConnectionString;
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {
                using (var Conn = new NpgsqlConnection(_dbconnectionstrig))
                {
                    Conn.Open();

                    // Check for duplicate category name
                    string duplicateCheckQuery = model.Id > 0
                        ? "SELECT COUNT(*) FROM categorieses WHERE LOWER(TRIM(name)) = LOWER(TRIM(@p_name)) AND companyid = @p_companyid AND id != @p_id"
                        : "SELECT COUNT(*) FROM categorieses WHERE LOWER(TRIM(name)) = LOWER(TRIM(@p_name)) AND companyid = @p_companyid";

                    using (var checkCmd = new NpgsqlCommand(duplicateCheckQuery, Conn))
                    {
                        checkCmd.Parameters.AddWithValue("p_name", model.Name ?? "");
                        checkCmd.Parameters.AddWithValue("p_companyid", companyId);
                        if (model.Id > 0)
                            checkCmd.Parameters.AddWithValue("p_id", model.Id);

                        var count = (long)(checkCmd.ExecuteScalar() ?? 0);
                        if (count > 0)
                        {
                            return Json(new { success = false, message = "A category with this name already exists. Please use a different name." });
                        }
                    }

                    var insertquery = string.Empty;
                    if (model.Id > 0)
                    {
                        insertquery = "update categorieses set name = @p_name where id = @p_id ";

                    }
                    else
                    {
                        insertquery = "insert into categorieses(name,companyid)values(@p_name,@p_companyid) ";
                    }

                    Conn.ExecuteSql(insertquery, new { p_name = model.Name, p_companyid = companyId , p_id = model.Id});
                }

                return Json(new { success = true, message = "Category created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult DeleteItem(int id)
        {
            try
            {
                if (id > 0)
                {
                    string Query = "delete from billitem where id = @p_id";
                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        conn.QuerySql(Query, new { p_id = id });
                    }
                }
                return Json(new { success = true, message = "Item deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewItem(int id = 0)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                BillItem billItem = new BillItem();
                var model = new BillItemViewModel();
                model.IsView = true;
                if (id > 0)
                {
                    var data = GetItemsAsync(companyId);
                    if (data != null && data.Count() > 0)
                    {
                        billItem = data.Where(x => x.Id == id).FirstOrDefault();
                    }


                }
                if (billItem != null && billItem.Id > 0)
                {
                    model = new BillItemViewModel
                    {

                        BillItem = billItem,
                        Categories = await _billItemService.GetCategoriesAsync(companyId),
                        Units = await _billItemService.GetUnitsAsync(),
                        TaxRates = await _billItemService.GetTaxRatesAsync(),
                        RawMaterials = billItem.Manufacturing,
                        AdditionalCosts = new List<AdditionalCost>(),
                        IsView = true
                    };
                }
                else
                {
                    model = new BillItemViewModel
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
                        Categories = await _billItemService.GetCategoriesAsync(companyId),
                        Units = await _billItemService.GetUnitsAsync(),
                        TaxRates = await _billItemService.GetTaxRatesAsync(),
                        RawMaterials = new List<RawMaterial>
                    {
                        new RawMaterial { Id = 1 },
                        new RawMaterial { Id = 2 }
                    },
                        AdditionalCosts = new List<AdditionalCost>(),
                        IsView = true

                    };
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServicePartialView(int? id)
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            var viewModel = GetServiceAsync(companyId);

            ItemViewModel itemViewModel = new ItemViewModel();
            itemViewModel.ItemView = viewModel;
            if (id > 0)
            {
                itemViewModel.SelectedItem = new BillItem();
                itemViewModel.SelectedItem = viewModel.Where(x => x.Id == id).FirstOrDefault();
            }
            else
            {

                itemViewModel.SelectedItem = new BillItem();
                itemViewModel.SelectedItem = viewModel.FirstOrDefault();

            }


            return PartialView("_ServicePartial", itemViewModel);

        }

        public List<BillItem> GetServiceAsync(int CompanyId = 0)
        {
            var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

            List<BillItem> items = new List<BillItem>();
            try
            {

                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    // ? Query to get all bill items
                    var billItemSql = @"
                                    SELECT 
                                        id AS ""Id"",
                                        item_type AS ""ItemType"",
                                        item_name AS ""ItemName"",
                                        item_hsn AS ""ItemHsn"",
                                        item_code AS ""ItemCode"",
                                        category AS ""Category"",
                                        unit AS ""Unit"",
                                        item_image_url AS ""ImageUrl"",
                                        sale_price AS ""SalePrice"",
                                        sale_price_tax_type AS ""SalePriceTaxType"",
                                        discount_on_sale_price AS ""DiscountOnSalePrice"",
                                        discount_type AS ""DiscountType"",
                                        purchase_price AS ""PurchasePrice"",
                                        purchase_price_tax_type AS ""PurchasePriceTaxType"",
                                        tax_rate AS ""TaxRate"",
                                        wholesale_price AS ""WholesalePrice"",
                                        wholesale_price_tax_type AS ""WholesalePriceTaxType"",
                                        min_wholesale_qty AS ""MinWholesaleQty"",
                                        disc_on_mrp_wholesale AS ""DiscOnMrpWholesale"",
                                        additional_cess AS ""AdditionalCess"",
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
                                        colour AS ""Colour"",
                                        material AS ""Material"",
                                        mfg_date AS ""MfgDate"",
                                        exp_date AS ""ExpDate"",
                                        size AS ""Size"",
                                        brand AS ""Brand"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where item_type = @p_itemtype and (@p_companyid = 0 OR companyid = @p_companyid)
                                    ORDER BY id;
                                    ";

                    // ? Fetch bill items
                    var billItems = connection.QuerySql<BillItem>(billItemSql, new { p_itemtype = "service", p_companyid = CompanyId }).ToList();

                    if (billItems != null && billItems.Count > 0)
                    {
                        foreach (var billItem in billItems)
                        {
                            // ? Query to fetch manufacturing data for each bill item
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
                }
            }
            catch (Exception ex)
            {

            }

            return items;
        }
        private async Task<string> SaveImageToServer(string base64String, string fileName, string subfolder)
        {
            try
            {
                // Convert base64 to byte array
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Create unique filename to avoid conflicts
                string fileExtension = Path.GetExtension(fileName);
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                // Create directory path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", subfolder);

                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Full file path
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Return relative path for storing in database
                return $"/uploads/{subfolder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                // Log error and return null or empty string
                // You might want to use your logging framework here
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUnitsPartialView()
        {
            var _connectionString = MUNEEMJI.DbConfig.ConnectionString;
            ItemViewModel model = new ItemViewModel();
            try
            {
                await Task.Delay(1);
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    model.Units = new List<UnitViewModel>();
                    model.Units = connection.QuerySql<UnitViewModel>("select id , fullname , shortname from units").ToList();

                }

            }

            catch (Exception ex)
            {
            }
            return PartialView("_UnitsPartial", model);
        }

        [HttpGet]
        public IActionResult ViewItemForPartial(int id)
        {
            BillItem billItem = new BillItem();

            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                var item = GetItemsAsync(companyId);
                if (item != null && item.Count() > 0)
                {
                    billItem = item.Where(x => x.Id == id).FirstOrDefault();
                }

                if (item == null)
                {
                    return NotFound(new { success = false, message = "Item not found" });
                }

                var response = new
                {
                    id = billItem.Id,
                    itemName = billItem.ItemName,
                    salePrice = billItem.SalePrice,
                    purchasePrice = billItem.PurchasePrice,
                    openingQuantity = billItem.OpeningQuantity,
                    onlineStorePrice = billItem.OnlineStorePrice,
                    transactions = new List<object>
                    {
                         new
                {
                    type = "Purchase",
                    location = "Main",
                    invoiceRef = "123",
                    name = "Supplier Name",
                    date = DateTime.UtcNow.ToString("dd/MM/yyyy"),
                    quantity = 1,
                    pricePerUnit = 456,
                    status = "Pending"
                }
                    }
                };

                return Json(response);
            }
            catch (Exception ex)
            {


                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        public  int GetItemCode(int CompanyId)
        {
            int itemcode = 96105600;
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var SelectQuery = " select count(*)  FROM billitem where  item_type = @p_itemtype and companyid = @p_companyid ";

                    var billItems =  Conn.ExecuteScalarSql<long>(SelectQuery, new { p_itemtype = "product", p_companyid = CompanyId });

                    if (billItems > 0)
                    {
                        itemcode = itemcode + Convert.ToInt32(billItems);
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return itemcode;
        }

        // ?? Check if item code already exists ??
        [HttpGet]
        public async Task<IActionResult> CheckItemCode(string code, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Json(new { isAvailable = true, message = "" });

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                string query;
                NpgsqlCommand cmd;

                if (excludeId > 0)
                {
                    query = "SELECT COUNT(*) FROM billitem WHERE LOWER(TRIM(item_code)) = LOWER(TRIM(@code)) AND id != @excludeId";
                    cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("code", code.Trim());
                    cmd.Parameters.AddWithValue("excludeId", excludeId);
                }
                else
                {
                    query = "SELECT COUNT(*) FROM billitem WHERE LOWER(TRIM(item_code)) = LOWER(TRIM(@code))";
                    cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("code", code.Trim());
                }

                var count = (long)(await cmd.ExecuteScalarAsync() ?? 0);

                if (count > 0)
                    return Json(new { isAvailable = false, message = "Item code already exists. Please use a different code." });

                return Json(new { isAvailable = true, message = "Item code is available." });
            }
            catch (Exception ex)
            {
                return Json(new { isAvailable = true, message = "" });
            }
        }

        // ?? Generate a unique 11-digit item code ??
        [HttpGet]
        public async Task<IActionResult> GenerateItemCode()
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var rand = new Random();
                const long min = 10000000000L;
                const long max = 99999999999L;

                for (int attempt = 0; attempt < 10; attempt++)
                {
                    long code = (long)(rand.NextDouble() * (max - min)) + min;
                    string codeStr = code.ToString();

                    var query = "SELECT COUNT(*) FROM billitem WHERE item_code = @code";
                    using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("code", codeStr);

                    var count = (long)(await cmd.ExecuteScalarAsync() ?? 0);
                    if (count == 0)
                        return Json(new { success = true, code = codeStr });
                }

                return Json(new { success = false, message = "Could not generate a unique code after 10 attempts. Please try again." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating code: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetItemsByCategory(string categoryId)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;
                    if (categoryId == "0" || categoryId == "uncategorized")
                    {
                        query = @"SELECT id, item_name, opening_quantity, online_store_price 
                                  FROM billitem 
                                  WHERE companyid = @p_companyid 
                                    AND (category IS NULL OR TRIM(category) = '')
                                  ORDER BY item_name";
                    }
                    else
                    {
                        query = @"SELECT id, item_name, opening_quantity, online_store_price 
                                  FROM billitem 
                                  WHERE companyid = @p_companyid 
                                    AND category = @p_category
                                  ORDER BY item_name";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        if (categoryId != "0" && categoryId != "uncategorized")
                        {
                            cmd.Parameters.AddWithValue("p_category", categoryId);
                        }

                        var items = new List<object>();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    openingQuantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                    onlineStorePrice = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3)
                                });
                            }
                        }

                        return Json(new { success = true, items });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetItemsForCategoryModal(string categoryId)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    // Get items that are NOT already in the selected category
                    string query;
                    if (categoryId == "0" || categoryId == "uncategorized")
                    {
                        // For uncategorized, show all items that ARE in some category
                        query = @"SELECT id, item_name, opening_quantity 
                                  FROM billitem 
                                  WHERE companyid = @p_companyid 
                                    AND category IS NOT NULL AND TRIM(category) != ''
                                  ORDER BY item_name";
                    }
                    else
                    {
                        // For a specific category, show items NOT in that category
                        query = @"SELECT id, item_name, opening_quantity 
                                  FROM billitem 
                                  WHERE companyid = @p_companyid 
                                    AND (category IS NULL OR TRIM(category) = '' OR category != @p_category)
                                  ORDER BY item_name";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        if (categoryId != "0" && categoryId != "uncategorized")
                        {
                            cmd.Parameters.AddWithValue("p_category", categoryId);
                        }

                        var items = new List<object>();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    openingQuantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                                });
                            }
                        }

                        return Json(new { success = true, items });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult MoveItemsToCategory([FromBody] MoveItemsToCategoryRequest request)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                if (request == null || request.ItemIds == null || !request.ItemIds.Any())
                    return Json(new { success = false, message = "No items selected." });

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    // If RemoveFromExisting is true, set category to the new one
                    // Otherwise just assign to the new category
                    string categoryValue = request.CategoryId;

                    foreach (var itemId in request.ItemIds)
                    {
                        string updateQuery = @"UPDATE billitem SET category = @p_category 
                                              WHERE id = @p_id AND companyid = @p_companyid";
                        using (var cmd = new NpgsqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("p_category", categoryValue);
                            cmd.Parameters.AddWithValue("p_id", itemId);
                            cmd.Parameters.AddWithValue("p_companyid", companyId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return Json(new { success = true, message = "Items moved to category successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetActiveItems()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, opening_quantity 
                                  FROM billitem 
                                  WHERE item_type = 'product' 
                                    AND companyid = @p_companyid 
                                    AND (is_active = true OR is_active IS NULL)
                                  ORDER BY item_name";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        var items = new List<object>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    itemName = reader.GetString(1),
                                    quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                    quantitySoldIn90Days = 0
                                });
                            }
                        }

                        return Json(new { success = true, items });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult BulkMarkInactive([FromBody] BulkInactiveRequest request)
        {
            try
            {
                if (request == null || request.ItemIds == null || !request.ItemIds.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"UPDATE billitem 
                                  SET is_active = false, updated_at = @p_updated_at 
                                  WHERE id = ANY(@p_ids) 
                                    AND companyid = @p_companyid";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_ids", request.ItemIds.ToArray());
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        cmd.Parameters.AddWithValue("p_updated_at", DateTime.UtcNow);

                        var rowsAffected = cmd.ExecuteNonQuery();
                        return Json(new { success = true, message = $"{rowsAffected} item(s) marked as inactive." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetInactiveItems()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, opening_quantity 
                                  FROM billitem 
                                  WHERE item_type = 'product' 
                                    AND companyid = @p_companyid 
                                    AND is_active = false
                                  ORDER BY item_name";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        var items = new List<object>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    itemName = reader.GetString(1),
                                    quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                                });
                            }
                        }

                        return Json(new { success = true, items });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetItemsWithoutCode()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, opening_quantity 
                                  FROM billitem 
                                  WHERE item_type = 'product' 
                                    AND companyid = @p_companyid 
                                    AND (is_active = true OR is_active IS NULL)
                                    AND (item_code IS NULL OR TRIM(item_code) = '')
                                  ORDER BY item_name";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        var items = new List<object>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    itemName = reader.GetString(1),
                                    quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                                });
                            }
                        }

                        return Json(new { success = true, items });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> BulkAssignCode([FromBody] BulkAssignCodeRequest request)
        {
            try
            {
                if (request == null || request.ItemIds == null || !request.ItemIds.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var rand = new Random();
                const long min = 10000000000L;
                const long max = 99999999999L;
                int assignedCount = 0;

                foreach (var itemId in request.ItemIds)
                {
                    string generatedCode = null;
                    for (int attempt = 0; attempt < 10; attempt++)
                    {
                        long code = (long)(rand.NextDouble() * (max - min)) + min;
                        string codeStr = code.ToString();

                        var checkQuery = "SELECT COUNT(*) FROM billitem WHERE item_code = @code";
                        using var checkCmd = new NpgsqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("code", codeStr);
                        var count = (long)(await checkCmd.ExecuteScalarAsync() ?? 0);
                        if (count == 0)
                        {
                            generatedCode = codeStr;
                            break;
                        }
                    }

                    if (generatedCode != null)
                    {
                        var updateQuery = @"UPDATE billitem 
                                            SET item_code = @p_code, updated_at = @p_updated_at 
                                            WHERE id = @p_id AND companyid = @p_companyid";
                        using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("p_code", generatedCode);
                        updateCmd.Parameters.AddWithValue("p_updated_at", DateTime.UtcNow);
                        updateCmd.Parameters.AddWithValue("p_id", itemId);
                        updateCmd.Parameters.AddWithValue("p_companyid", companyId);
                        await updateCmd.ExecuteNonQueryAsync();
                        assignedCount++;
                    }
                }

                return Json(new { success = true, message = $"Code assigned to {assignedCount} item(s) successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetItemsWithoutUnit()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, opening_quantity 
                                  FROM billitem 
                                  WHERE item_type = 'product' 
                                    AND companyid = @p_companyid 
                                    AND (is_active = true OR is_active IS NULL)
                                    AND (unit IS NULL OR TRIM(unit) = '')
                                  ORDER BY item_name";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        var items = new List<object>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    itemName = reader.GetString(1),
                                    quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                                });
                            }
                        }

                        return Json(new { success = true, items });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetUnitsFromMaster()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, fullname, shortname FROM units ORDER BY fullname";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var units = new List<object>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                units.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    fullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    shortName = reader.IsDBNull(2) ? "" : reader.GetString(2)
                                });
                            }
                        }

                        return Json(new { success = true, units });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult BulkAssignUnit([FromBody] BulkAssignUnitRequest request)
        {
            try
            {
                if (request == null || request.ItemIds == null || !request.ItemIds.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                if (string.IsNullOrWhiteSpace(request.BaseUnit))
                {
                    return Json(new { success = false, message = "Please select a base unit." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"UPDATE billitem 
                                  SET unit = @p_unit, updated_at = @p_updated_at 
                                  WHERE id = ANY(@p_ids) 
                                    AND companyid = @p_companyid";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_unit", request.BaseUnit);
                        cmd.Parameters.AddWithValue("p_ids", request.ItemIds.ToArray());
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        cmd.Parameters.AddWithValue("p_updated_at", DateTime.UtcNow);

                        var rowsAffected = cmd.ExecuteNonQuery();
                        return Json(new { success = true, message = $"Unit assigned to {rowsAffected} item(s) successfully." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AdjustStock([FromBody] StockAdjustmentRequest request)
        {
            try
            {
                if (request == null || request.ItemId <= 0)
                {
                    return Json(new { success = false, message = "Invalid item." });
                }

                if (request.TotalQty <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than 0." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    // Get current quantity
                    var getQtyQuery = "SELECT opening_quantity FROM billitem WHERE id = @p_id AND companyid = @p_companyid";
                    int currentQty = 0;
                    using (var getCmd = new NpgsqlCommand(getQtyQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("p_id", request.ItemId);
                        getCmd.Parameters.AddWithValue("p_companyid", companyId);
                        var result = getCmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            currentQty = Convert.ToInt32(result);
                    }

                    int newQty;
                    if (request.AdjustmentType == "add")
                        newQty = currentQty + request.TotalQty;
                    else
                        newQty = currentQty - request.TotalQty;

                    if (newQty < 0)
                        newQty = 0;

                    var updateQuery = @"UPDATE billitem 
                                        SET opening_quantity = @p_qty, at_price = @p_price, updated_at = @p_updated_at 
                                        WHERE id = @p_id AND companyid = @p_companyid";

                    using (var cmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("p_qty", newQty);
                        cmd.Parameters.AddWithValue("p_price", request.AtPrice);
                        cmd.Parameters.AddWithValue("p_updated_at", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("p_id", request.ItemId);
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        cmd.ExecuteNonQuery();
                    }

                    var action = request.AdjustmentType == "add" ? "added to" : "reduced from";
                    return Json(new { success = true, message = $"Stock {action} successfully. New quantity: {newQty}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult BulkMarkActive([FromBody] BulkActiveRequest request)
        {
            try
            {
                if (request == null || request.ItemIds == null || !request.ItemIds.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"UPDATE billitem 
                                  SET is_active = true, updated_at = @p_updated_at 
                                  WHERE id = ANY(@p_ids) 
                                    AND companyid = @p_companyid";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_ids", request.ItemIds.ToArray());
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        cmd.Parameters.AddWithValue("p_updated_at", DateTime.UtcNow);

                        var rowsAffected = cmd.ExecuteNonQuery();
                        return Json(new { success = true, message = $"{rowsAffected} item(s) marked as active." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ImportItems()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ImportItemsFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".xls" && ext != ".xlsx")
                    return Json(new { success = false, message = "Only .xls and .xlsx files are allowed." });

                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var errors = new List<string>();
                int importedCount = 0;

                using (var stream = file.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First();
                    var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                    if (lastRow < 2)
                        return Json(new { success = false, message = "The Excel file is empty or has no data rows." });

                    using var connection = new NpgsqlConnection(_connectionString);
                    connection.Open();

                    for (int row = 2; row <= lastRow; row++)
                    {
                        try
                        {
                            var itemName = worksheet.Cell(row, 1).GetString()?.Trim();
                            if (string.IsNullOrWhiteSpace(itemName))
                            {
                                errors.Add($"Row {row}: Item Name is required, skipped.");
                                continue;
                            }

                            var itemCode = worksheet.Cell(row, 2).GetString()?.Trim();
                            var hsn = worksheet.Cell(row, 3).GetString()?.Trim();
                            decimal salePrice = 0;
                            decimal.TryParse(worksheet.Cell(row, 4).GetString()?.Trim(), out salePrice);
                            decimal purchasePrice = 0;
                            decimal.TryParse(worksheet.Cell(row, 5).GetString()?.Trim(), out purchasePrice);
                            int openingQty = 0;
                            int.TryParse(worksheet.Cell(row, 6).GetString()?.Trim(), out openingQty);
                            int minStock = 0;
                            int.TryParse(worksheet.Cell(row, 7).GetString()?.Trim(), out minStock);
                            var location = worksheet.Cell(row, 8).GetString()?.Trim();
                            var taxRate = worksheet.Cell(row, 9).GetString()?.Trim();
                            var taxInclusive = worksheet.Cell(row, 10).GetString()?.Trim();

                            if (string.IsNullOrWhiteSpace(taxRate)) taxRate = "None";

                            var salePriceTaxType = "Without Tax";
                            if (!string.IsNullOrWhiteSpace(taxInclusive) && (taxInclusive.Equals("Y", StringComparison.OrdinalIgnoreCase) || taxInclusive.Equals("Yes", StringComparison.OrdinalIgnoreCase)))
                            {
                                salePriceTaxType = "With Tax";
                            }

                            string insertSql = @"
                                INSERT INTO billitem (
                                    item_type, item_name, item_hsn, item_code,
                                    sale_price, sale_price_tax_type,
                                    purchase_price, purchase_price_tax_type, tax_rate,
                                    opening_quantity, min_stock_to_maintain, location,
                                    created_at, updated_at, companyid
                                ) VALUES (
                                    @item_type, @item_name, @item_hsn, @item_code,
                                    @sale_price, @sale_price_tax_type,
                                    @purchase_price, @purchase_price_tax_type, @tax_rate,
                                    @opening_quantity, @min_stock_to_maintain, @location,
                                    @created_at, @updated_at, @companyid
                                )";

                            using var cmd = new NpgsqlCommand(insertSql, connection);
                            cmd.Parameters.AddWithValue("@item_type", "product");
                            cmd.Parameters.AddWithValue("@item_name", itemName);
                            cmd.Parameters.AddWithValue("@item_hsn", (object)hsn ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@item_code", (object)itemCode ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sale_price", salePrice);
                            cmd.Parameters.AddWithValue("@sale_price_tax_type", salePriceTaxType);
                            cmd.Parameters.AddWithValue("@purchase_price", purchasePrice);
                            cmd.Parameters.AddWithValue("@purchase_price_tax_type", salePriceTaxType);
                            cmd.Parameters.AddWithValue("@tax_rate", taxRate);
                            cmd.Parameters.AddWithValue("@opening_quantity", openingQty);
                            cmd.Parameters.AddWithValue("@min_stock_to_maintain", minStock);
                            cmd.Parameters.AddWithValue("@location", (object)location ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@companyid", companyId);

                            cmd.ExecuteNonQuery();
                            importedCount++;
                        }
                        catch (Exception rowEx)
                        {
                            errors.Add($"Row {row}: {rowEx.Message}");
                        }
                    }
                }

                if (importedCount > 0 && errors.Count == 0)
                    return Json(new { success = true, message = $"{importedCount} item(s) imported successfully." });
                else if (importedCount > 0 && errors.Count > 0)
                    return Json(new { success = true, message = $"{importedCount} item(s) imported. {errors.Count} row(s) had errors.", errors });
                else
                    return Json(new { success = false, message = "No items were imported.", errors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Import failed: {ex.Message}" });
            }
        }
    }

    public class BulkInactiveRequest
    {
        public List<int> ItemIds { get; set; }
    }

    public class BulkActiveRequest
    {
        public List<int> ItemIds { get; set; }
    }

    public class BulkAssignCodeRequest
    {
        public List<int> ItemIds { get; set; }
    }

    public class BulkAssignUnitRequest
    {
        public List<int> ItemIds { get; set; }
        public string BaseUnit { get; set; }
        public string SecondaryUnit { get; set; }
    }

    public class StockAdjustmentRequest
    {
        public int ItemId { get; set; }
        public string AdjustmentType { get; set; } // "add" or "reduce"
        public int TotalQty { get; set; }
        public decimal AtPrice { get; set; }
        public string AdjustmentDate { get; set; }
        public string Details { get; set; }
    }
}

