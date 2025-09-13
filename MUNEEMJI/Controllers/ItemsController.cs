using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using Npgsql;
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
        public ItemsController(IWebHostEnvironment webHostEnv, IBillItemService billItemService)
        {
            _webHostEnv = webHostEnv;
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            _billItemService = billItemService;
        }

        [HttpGet]
        public IActionResult MainIndex(int? id)
        {
            var viewModel =  GetItemsAsync();

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
        public async Task<IActionResult> Create(int id = 0)
        {
            try
            {
                BillItem billItem = new BillItem();
                var model = new BillItemViewModel();
                if (id > 0)
                {
                    var data = GetItemsAsync();
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
            try
            {
                if (ModelState.IsValid)
                {
                    // Set service-specific fields based on item type
                    if (model.ItemType == "Service")
                    {
                        model.ServiceName = model.ItemName;
                        model.ServiceHsn = model.ItemHsn;
                        model.ServiceCode = model.ItemCode;
                    }

                    bool result = await _billItemService.SaveBillItemAsync(model);

                    if (result)
                    {
                        //TempData["SuccessMessage"] = $"{model.ItemType} saved successfully!";
                        return RedirectToAction("MainIndex");
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
              
                ModelState.AddModelError("", "An error occurred while saving the item.");

                // Reload dropdown data
                viewModel.Categories = await _billItemService.GetCategoriesAsync();
                viewModel.Units = await _billItemService.GetUnitsAsync();
                viewModel.TaxRates = await _billItemService.GetTaxRatesAsync();

                return View(viewModel);
            }
        }
        public  List<BillItem> GetItemsAsync()
        {
          var  _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

            List<BillItem> items = new List<BillItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {

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
                                    FROM billitem where  item_type = @p_itemtype
                                    ORDER BY id;
                                    ";

                // ✅ Fetch bill items
                var billItems = connection.QuerySql<BillItem>(billItemSql,new { p_itemtype= "product" }).ToList();

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
            }

            return items;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriespartialView()
        {
            ItemViewModel ViewModel = new ItemViewModel();
            ViewModel.Categories = new List<Category>();
            try
            {


                using (var conn = new NpgsqlConnection("Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser"))
                {
                    conn.Open();
                    string query = @"SELECT id, name 
                             FROM categorieses 
                             ";

                    using (var cmd = new NpgsqlCommand(query, conn))
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
            catch (Exception ex)
            {

            }
            return PartialView("_CategoryPartial", ViewModel);
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
                BillItem billItem = new BillItem();
                var model = new BillItemViewModel();
                model.IsView = true;
                if (id > 0)
                {
                    var data =  GetItemsAsync();
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
            var _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

            List<BillItem> items = new List<BillItem>();
            try
            {

                using (var connection = new NpgsqlConnection(_connectionString))
                {

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
                                    FROM billitem where item_type = @p_itemtye
                                    ORDER BY id;
                                    ";

                    // ✅ Fetch bill items
                    var billItems = connection.QuerySql<BillItem>(billItemSql, new { p_itemtye = "service" }).ToList();

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
                }
            }
            catch(Exception ex)
            {

            }

            return items;
        }
    }
}

