using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly IBillItemService _billItemService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICompanyTenancy _CompayTenancy;
        private readonly IGstSettingsService _gstSettingsService;


        public ServicesController(IBillItemService billItem, IWebHostEnvironment webHostEnvironment, ICompanyTenancy companyTenancy, IGstSettingsService gstSettingsService) 
        {
            _billItemService = billItem;
            _webHostEnvironment = webHostEnvironment;
            _CompayTenancy = companyTenancy;
            _gstSettingsService = gstSettingsService;

        }

        [HttpGet]
        public async Task<IActionResult> Create(int id = 0)
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
                        Categories = await _billItemService.GetCategoriesAsync(companyId),
                        Units = await _billItemService.GetUnitsAsync(),
                        TaxRates = await _billItemService.GetTaxRatesAsync(),
                        RawMaterials = billItem.Manufacturing,
                        AdditionalCosts = new List<AdditionalCost>()
                    };
                }
                else
                {
                    var ServiceCode = GetServiceCode(companyId);
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
                            ItemCode =Convert.ToString(ServiceCode)
                            
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
                    // Check for duplicate service name
                    string duplicateCheckQuery = model.Id > 0
                        ? "SELECT COUNT(*) FROM billitem WHERE LOWER(TRIM(item_name)) = LOWER(TRIM(@p_name)) AND item_type = 'service' AND companyid = @p_companyid AND id != @p_id"
                        : "SELECT COUNT(*) FROM billitem WHERE LOWER(TRIM(item_name)) = LOWER(TRIM(@p_name)) AND item_type = 'service' AND companyid = @p_companyid";

                    using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                    {
                        conn.Open();
                        using (var checkCmd = new NpgsqlCommand(duplicateCheckQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("p_name", model.ItemName ?? "");
                            checkCmd.Parameters.AddWithValue("p_companyid", companyId);
                            if (model.Id > 0)
                                checkCmd.Parameters.AddWithValue("p_id", model.Id);

                            var count = (long)(checkCmd.ExecuteScalar() ?? 0);
                            if (count > 0)
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = "A service with this name already exists. Please use a different name.",
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
                        if (existingItem != null)
                        {
                            model.ImageUrl = existingItem.ImageUrl;
                        }
                    }

                    bool result = await _billItemService.SaveBillItemAsync(model,companyId);

                    if (result)
                    {
                        //TempData["SuccessMessage"] = $"{model.ItemType} saved successfully!";
                        return Json(new
                        {
                            success = true,
                            message = model.Id > 0
                                       ? "Service updated successfully!"
                                       : "Service has been saved successfully!",
                            id = model.Id,
                            itemName = model.ItemName
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

                return View(viewModel);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "An error occurred while saving the item.");

                // Reload dropdown data
                viewModel.Categories = await _billItemService.GetCategoriesAsync(companyId);
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                return Json(new
                {
                    success = true,
                    message = ex.Message
                });
            }
        }
        public List<BillItem> GetItemsAsync(int CompanyId)
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();

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
                                    FROM billitem where Companyid = @p_companyid
                                    ORDER BY item_name;
                                    ";

                // ? Fetch bill items
                var billItems = connection.QuerySql<BillItem>(billItemSql,new { p_companyid  = companyId }).ToList();

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


        [HttpGet]
        public async Task<IActionResult> Service(int? id)
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

            return View(itemViewModel);

        }

        public List<BillItem> GetServiceAsync(int companyId)
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
                                    FROM billitem where item_type = @p_itemtye and companyid = @p_companyId and (is_active = true OR is_active IS NULL)
                                                    ORDER BY item_name;
                                    ";

                    // ? Fetch bill items
                    var billItems = connection.QuerySql<BillItem>(billItemSql, new { p_itemtye = "service", p_companyId = companyId }).ToList();

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
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult DeleteItem(int id)
        {
            var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

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
        public int GetServiceCode(int CompanyId)
        {
            int itemcode = 96105600;
            var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var SelectQuery = " select count(*)  FROM billitem where  item_type = @p_itemtype and companyid = @p_companyid ";

                    var billItems = Conn.ExecuteScalarSql<long>(SelectQuery, new { p_itemtype = "service", p_companyid = CompanyId });

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

        [HttpGet]
        public IActionResult GetActiveServices()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, sale_price 
                                  FROM billitem 
                                  WHERE item_type = 'service' 
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
                                    itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    salePrice = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2)
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
        public IActionResult GetInactiveServices()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, sale_price 
                                  FROM billitem 
                                  WHERE item_type = 'service' 
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
                                    itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    salePrice = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2)
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
                    return Json(new { success = false, message = "No services selected." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

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
                        return Json(new { success = true, message = $"{rowsAffected} service(s) marked as inactive." });
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
        public IActionResult BulkMarkActive([FromBody] BulkActiveRequest request)
        {
            try
            {
                if (request == null || request.ItemIds == null || !request.ItemIds.Any())
                {
                    return Json(new { success = false, message = "No services selected." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

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
                        return Json(new { success = true, message = $"{rowsAffected} service(s) marked as active." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetServicesWithoutCode()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, sale_price 
                                  FROM billitem 
                                  WHERE item_type = 'service' 
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
                                    itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    salePrice = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2)
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
                    return Json(new { success = false, message = "No services selected." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

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

                return Json(new { success = true, message = $"Code assigned to {assignedCount} service(s) successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetServicesWithoutUnit()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = @"SELECT id, item_name, sale_price 
                                  FROM billitem 
                                  WHERE item_type = 'service' 
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
                                    itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    salePrice = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2)
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
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

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
                    return Json(new { success = false, message = "No services selected." });
                }

                if (string.IsNullOrWhiteSpace(request.BaseUnit))
                {
                    return Json(new { success = false, message = "Please select a base unit." });
                }

                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;

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
                        return Json(new { success = true, message = $"Unit assigned to {rowsAffected} service(s) successfully." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult BulkUpdateServices()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetTaxRatesForBulkUpdate()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            var taxRates = _gstSettingsService.GetTaxRates(companyId);
            var result = taxRates.Select(t => t.Name).ToList();
            result.Insert(0, "None");
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetBulkUpdateServicesData()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            var _connectionString = MUNEEMJI.DbConfig.ConnectionString;
            var items = new List<object>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var sql = @"SELECT id, item_name, item_hsn, item_code, category, 
                            sale_price, sale_price_tax_type, 
                            discount_on_sale_price, discount_type, tax_rate, description
                            FROM billitem 
                            WHERE item_type = 'service' AND companyid = @p_cid AND (is_active = true OR is_active IS NULL)
                            ORDER BY item_name";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("p_cid", companyId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new
                    {
                        id = reader.GetInt32(0),
                        itemName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        itemHsn = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        itemCode = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        category = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        salePrice = reader.IsDBNull(5) ? 0m : reader.GetDecimal(5),
                        salePriceTaxType = reader.IsDBNull(6) ? "Excluded" : reader.GetString(6),
                        discountOnSalePrice = reader.IsDBNull(7) ? 0m : reader.GetDecimal(7),
                        discountType = reader.IsDBNull(8) ? "Percentage" : reader.GetString(8),
                        taxRate = reader.IsDBNull(9) ? "None" : reader.GetString(9),
                        description = reader.IsDBNull(10) ? "" : reader.GetString(10)
                    });
                }
            }
            return Json(items);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult BulkUpdateServicesSave([FromBody] List<BulkUpdateServiceRequest> items)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var _connectionString = MUNEEMJI.DbConfig.ConnectionString;
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                int updated = 0;
                foreach (var item in items)
                {
                    var sql = @"UPDATE billitem SET 
                        item_name = @p_name, category = @p_category, item_hsn = @p_hsn, item_code = @p_code,
                        sale_price = @p_sp, sale_price_tax_type = @p_sptt,
                        discount_on_sale_price = @p_disc, discount_type = @p_dt, tax_rate = @p_tr,
                        description = @p_desc, updated_at = @p_ua
                        WHERE id = @p_id AND companyid = @p_cid AND item_type = 'service'";
                    using var cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("p_id", item.Id);
                    cmd.Parameters.AddWithValue("p_cid", companyId);
                    cmd.Parameters.AddWithValue("p_name", item.ItemName ?? "");
                    cmd.Parameters.AddWithValue("p_category", (object)item.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_hsn", (object)item.ItemHsn ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_code", (object)item.ItemCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_sp", item.SalePrice);
                    cmd.Parameters.AddWithValue("p_sptt", item.SalePriceTaxType ?? "Excluded");
                    cmd.Parameters.AddWithValue("p_disc", item.DiscountOnSalePrice);
                    cmd.Parameters.AddWithValue("p_dt", item.DiscountType ?? "Percentage");
                    cmd.Parameters.AddWithValue("p_tr", item.TaxRate ?? "None");
                    cmd.Parameters.AddWithValue("p_desc", (object)item.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ua", DateTime.UtcNow);
                    updated += cmd.ExecuteNonQuery();
                }
                return Json(new { success = true, message = $"{updated} service(s) updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }

    public class BulkUpdateServiceRequest
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string ItemHsn { get; set; }
        public string ItemCode { get; set; }
        public decimal SalePrice { get; set; }
        public string SalePriceTaxType { get; set; }
        public decimal DiscountOnSalePrice { get; set; }
        public string DiscountType { get; set; }
        public string TaxRate { get; set; }
        public string Description { get; set; }
    }
}
