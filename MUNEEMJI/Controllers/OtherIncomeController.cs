
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
        private readonly IWebHostEnvironment _environment;
        public OtherIncomeController(IOtherIncomeRepository billItemService, ILogger<BillItemController> logger, ICompanyTenancy companyTenancy, IWebHostEnvironment environment)
        {
            _billItemService = billItemService;
            _logger = logger;
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;
            _companyTenancy = companyTenancy;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = new OtherIncomeModel
                {
                    Categories = await _billItemService.GetAllOtherIncomeCategories(),
                    SelectedItem = new OtherIncomeViewModel(),
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
        public async Task<IActionResult> Create( OtherIncomeModel model)
        {
            using var db = new NpgsqlConnection(_connectionString);
            var Companyid = _companyTenancy.GetCurrentCompanyId();
            await db.OpenAsync();
            using var transaction = await db.BeginTransactionAsync();
            try
            {
                if (model.OtherIncomeView.ImageUrl != null && model.OtherIncomeView.ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "transaction");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.OtherIncomeView.ImageUrl.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.OtherIncomeView.ImageUrl.CopyToAsync(fileStream);
                    }

                    model.OtherIncomeView.BaseImageUrl = "/Web/uploads/transaction/" + uniqueFileName;
                }
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
                    cmd.Parameters.AddWithValue("income_category", model.OtherIncomeView.IncomeCategory!=null ? model.OtherIncomeView.IncomeCategory:string.Empty);
                    cmd.Parameters.AddWithValue("incomecategoryid", model.OtherIncomeView.IncomeCategoryId);
                    cmd.Parameters.AddWithValue("entry_date", model.OtherIncomeView.EntryDate);
                    cmd.Parameters.AddWithValue("round_off", model.OtherIncomeView.RoundOff);
                    cmd.Parameters.AddWithValue("total", model.OtherIncomeView.Total);
                    cmd.Parameters.AddWithValue("payment_type", model.OtherIncomeView.PaymentType);
                    cmd.Parameters.AddWithValue("description", (object?)model.OtherIncomeView.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("image_url", (object?)model.OtherIncomeView.BaseImageUrl ?? DBNull.Value);
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
                //return Ok(new { success = true, entryId });
                return Json(new { success = true, message = "Saved successfully!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }


        public async Task<IActionResult> Index(int? id)
        {
            string godown = "All Godowns"; string search = ""; string sortBy = "date"; string sortDirection = "desc";
            List<OtherIncomeViewModel> Model = new List<OtherIncomeViewModel>();
            OtherIncomeModel otherIncomeModel = new OtherIncomeModel();
            otherIncomeModel.Categories = new List<OtherIncomeCategory>();
            try
            {
                var viewModel = await _billItemService.GetAllOtherIncomeCategories();           
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = $" select income_category as IncomeCategory , incomecategoryid, entry_date as EntryDate , total, amount from public.income_entries" +
                        $" left join income_entry_items on income_entry_items.entry_id = income_entries.id ";

                    Model = Conn.QuerySql<OtherIncomeViewModel>(QueryString).ToList();
                }

                foreach (var item in viewModel)
                {
                    OtherIncomeCategory otherIncome = new OtherIncomeCategory();
                    otherIncome.Id = item.Id;
                    otherIncome.Name = item.Name;
                    otherIncome.Amount = Model.Where(x => x.IncomeCategoryId == item.Id).Sum(x => x.amount);
                    otherIncomeModel.Categories.Add(otherIncome);
                }

                if (id > 0)
                {
                         Model = Model.Where(x => x.IncomeCategoryId == id).ToList();
                        otherIncomeModel.SelectedItem = Model.FirstOrDefault();
                        otherIncomeModel.otherIncomeEntries = Model;
                }
                else
                {

                    otherIncomeModel.SelectedItem = new OtherIncomeViewModel();
                    otherIncomeModel.otherIncomeEntries = Model.ToList();

                }
                

                return View(otherIncomeModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while loading the inventory data.";
                return View(new InventoryViewModel());
            }
        }
       

        public async Task<List<BillItem>> GetBillItemsAsync()
        {
            List<BillItem> items = new List<BillItem>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

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
            created_at AS ""CreatedAt"",
            updated_at AS ""UpdatedAt""
        FROM billitem
        ORDER BY id;
    ";

            // ? Fetch bill items
            var billItems = connection.QuerySql<BillItem>(billItemSql).ToList();

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

            return items;
        }
    }
}

