
using Insight.Database;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace MUNEEMJI.Controllers
{
    public class OtherIncomeController : Controller
    {
        private readonly IOtherIncomeRepository _billItemService;
        private readonly ILogger<BillItemController> _logger;
        string _connectionString = string.Empty;
        private readonly ICompanyTenancy _companyTenancy;
        public OtherIncomeController(IOtherIncomeRepository billItemService, ILogger<BillItemController> logger, ICompanyTenancy companyTenancy)
        {
            _billItemService = billItemService;
            _logger = logger;
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            _companyTenancy = companyTenancy;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = new OtherIncomeModel
                {
                    Categories = GetAllOtherIncomeCategories(),
                    SelectedItem = new IncomeEntry(),
                    OtherIncomeView  = new IncomeEntry
                    {
                        Items = new List<IncomeEntryItem> { new IncomeEntryItem() }
                    }
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
        public async Task<IActionResult> Create([FromBody] OtherIncomeModel model)
        {
            using var db = new NpgsqlConnection(_connectionString);
            var Companyid = _companyTenancy.GetCurrentCompanyId();
            await db.OpenAsync();

            using var transaction = await db.BeginTransactionAsync();

            try
            {
                int entryId;

                // Insert into income_entries
                var insertEntrySql = @"
            INSERT INTO income_entries 
                (income_category, incomecategoryid, entry_date, round_off, total, payment_type, description, image_url,companyid)
            VALUES 
                (@income_category, @incomecategoryid, @entry_date, @round_off, @total, @payment_type, @description, @image_url,@p_companyid)
            RETURNING id;
        ";

                using (var cmd = new NpgsqlCommand(insertEntrySql, db, transaction))
                {
                    cmd.Parameters.AddWithValue("income_category", model.OtherIncomeView.IncomeCategory);
                    cmd.Parameters.AddWithValue("incomecategoryid", model.OtherIncomeView.IncomeCategoryId);
                    cmd.Parameters.AddWithValue("entry_date", model.OtherIncomeView.EntryDate);
                    cmd.Parameters.AddWithValue("round_off", model.OtherIncomeView.RoundOff);
                    cmd.Parameters.AddWithValue("total", model.OtherIncomeView.Total);
                    cmd.Parameters.AddWithValue("payment_type", model.OtherIncomeView.PaymentType);
                    cmd.Parameters.AddWithValue("description", (object?)model.OtherIncomeView.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("image_url", (object?)model.OtherIncomeView.ImageUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_companyid", Companyid);

                    object result = await cmd.ExecuteScalarAsync();
                    entryId = Convert.ToInt32(result);
                }

                // Insert line items
                var insertItemSql = @"
            INSERT INTO income_entry_items 
                (entry_id, item_name, qty, price_per_unit)
            VALUES 
                (@entry_id, @item_name, @qty, @price_per_unit);
        ";

                foreach (var item in model.OtherIncomeView.Items)
                {
                    using var itemCmd = new NpgsqlCommand(insertItemSql, db, transaction);
                    itemCmd.Parameters.AddWithValue("entry_id", entryId);
                    itemCmd.Parameters.AddWithValue("item_name", item.ItemName);
                    itemCmd.Parameters.AddWithValue("qty", item.Qty);
                    itemCmd.Parameters.AddWithValue("price_per_unit", item.PricePerUnit);

                    await itemCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { success = true, entryId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
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
                else
                {

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
        public List<OtherIncomeCategory> GetAllOtherIncomeCategories()
        {
            List<OtherIncomeCategory> returnobj = new List<OtherIncomeCategory>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                             SELECT 
                                 id, 
                                 name 
                             FROM public.other_income_categorieses";


                returnobj = conn.QuerySql<OtherIncomeCategory>(query).ToList();
            }
            return returnobj;
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
    }
}

