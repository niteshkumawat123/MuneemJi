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


                }
                if (billItem != null && billItem.Id > 0)
                {
                    model = new BillItemViewModel
                    {

                        BillItem = billItem,
                        Categories = await _billItemService.GetCategoriesAsync(),
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
                            ItemType = "service",
                            AsOfDate = DateTime.Today,
                            SalePriceTaxType = "Without Tax",
                            PurchasePriceTaxType = "Without Tax",
                            DiscountType = "Percentage",
                            TaxRate = "None",
                            ItemCode = ""
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
                viewModel.Categories = await _billItemService.GetCategoriesAsync();
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                return View(viewModel);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "An error occurred while saving the item.");

                // Reload dropdown data
                viewModel.Categories = await _billItemService.GetCategoriesAsync();
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
                                        colour AS ""Colour"",
                                        material AS ""Material"",
                                        mfg_date AS ""MfgDate"",
                                        exp_date AS ""ExpDate"",
                                        size AS ""Size"",
                                        brand AS ""Brand"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where  item_type = @p_itemtype and companyid = @p_companyid
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
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {
                await Task.Delay(1);

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
                        Categories = await _billItemService.GetCategoriesAsync(),
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
                        Categories = await _billItemService.GetCategoriesAsync(),
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
            var viewModel = GetServiceAsync();

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

        public List<BillItem> GetServiceAsync()
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
                                        colour AS ""Colour"",
                                        material AS ""Material"",
                                        mfg_date AS ""MfgDate"",
                                        exp_date AS ""ExpDate"",
                                        size AS ""Size"",
                                        brand AS ""Brand"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where item_type = @p_itemtye
                                    ORDER BY id;
                                    ";

                    // ? Fetch bill items
                    var billItems = connection.QuerySql<BillItem>(billItemSql, new { p_itemtye = "service" }).ToList();

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
    }
}

