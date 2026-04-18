using Dapper;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class ItemSettingsController : Controller
    {
        private readonly string _connectionString;

        public ItemSettingsController(IConfiguration configuration)
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString; ;
        }

        // GET: ItemSettings
        public async Task<IActionResult> Index()
        {
            var model = new ItemSettingsViewModel();

            using var connection = new NpgsqlConnection(_connectionString);

            // Get existing settings - assuming there's only one global setting record
            var settingsQuery = @"
                SELECT 
                    enable_item,
                    what_do_you_sell,
                    barcode_scan,
                    direct_barcode_scan,
                    stock_maintenance,
                    manufacturing,
                    show_low_stock_dialog,
                    items_unit,
                    default_unit,
                    item_category,
                    party_wise_item_rate,
                    description,
                    item_wise_tax,
                    item_wise_discount,
                    update_sale_price_from_transaction,
                    mrp_enabled,
                    calculate_sale_price_from_mrp,
                    use_mrp_for_batch_tracking,
                    serial_no_tracking,
                    batch_no_enabled,
                    exp_date_enabled,
                    mfg_date_enabled,
                    model_no_enabled,
                    size_enabled
                FROM item_settings 
                WHERE id = 1";

            var settings = await connection.QuerySingleOrDefaultAsync<ItemSettingsData>(settingsQuery);

            if (settings != null)
            {
                model.EnableItem = settings.enable_item;
                model.WhatDoYouSell = settings.what_do_you_sell ?? "Product/Service";
                model.BarcodeScanning = settings.barcode_scan;
                model.DirectBarcodeScanning = settings.direct_barcode_scan;
                model.StockMaintenance = settings.stock_maintenance;
                model.Manufacturing = settings.manufacturing;
                model.ShowLowStockDialog = settings.show_low_stock_dialog;
                model.ItemsUnit = settings.items_unit;
                model.DefaultUnit = settings.default_unit ?? "";
                model.ItemCategory = settings.item_category;
                model.PartyWiseItemRate = settings.party_wise_item_rate;
                model.Description = settings.description;
                model.ItemWiseTax = settings.item_wise_tax;
                model.ItemWiseDiscount = settings.item_wise_discount;
                model.UpdateSalePriceFromTransaction = settings.update_sale_price_from_transaction;

                // MRP/Price settings
                model.MrpEnabled = settings.mrp_enabled;
                model.CalculateSalePriceFromMrp = settings.calculate_sale_price_from_mrp;
                model.UseMrpForBatchTracking = settings.use_mrp_for_batch_tracking;

                // Serial No. Tracking
                model.SerialNoTracking = settings.serial_no_tracking;

                // Batch Tracking
                model.BatchNoEnabled = settings.batch_no_enabled;
                model.ExpDateEnabled = settings.exp_date_enabled;
                model.MfgDateEnabled = settings.mfg_date_enabled;
                model.ModelNoEnabled = settings.model_no_enabled;
                model.SizeEnabled = settings.size_enabled;
            }
            else
            {
                // Set default values for new settings
                model.EnableItem = true;
                model.WhatDoYouSell = "Product/Service";
                model.BarcodeScanning = true;
                model.DirectBarcodeScanning = true;
                model.StockMaintenance = true;
                model.Manufacturing = true;
                model.ShowLowStockDialog = true;
                model.ItemsUnit = true;
                model.ItemCategory = true;
                model.PartyWiseItemRate = true;
                model.Description = true;
                model.ItemWiseTax = true;
                model.UpdateSalePriceFromTransaction = true;
                model.MrpEnabled = true;
                model.CalculateSalePriceFromMrp = true;
                model.UseMrpForBatchTracking = true;
                model.SerialNoTracking = true;
                model.BatchNoEnabled = true;
                model.ExpDateEnabled = true;
                model.MfgDateEnabled = true;
                model.ModelNoEnabled = true;
                model.SizeEnabled = true;
            }

            return View(model);
        }

        // POST: ItemSettings/SaveOrUpdate
        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromBody]ItemSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();

                return Json(new { success = false, message = "Validation failed", errors = errors });
            }

            using var connection = new NpgsqlConnection(_connectionString);

            // Check if settings exist
            var existsQuery = "SELECT COUNT(*) FROM item_settings WHERE id = 1";
            var exists = await connection.QuerySingleAsync<int>(existsQuery) > 0;

            if (exists)
            {
                // Update existing settings
                var updateQuery = @"
                    UPDATE item_settings SET
                        enable_item = @EnableItem,
                        what_do_you_sell = @WhatDoYouSell,
                        barcode_scan = @BarcodeScanning,
                        direct_barcode_scan = @DirectBarcodeScanning,
                        stock_maintenance = @StockMaintenance,
                        manufacturing = @Manufacturing,
                        show_low_stock_dialog = @ShowLowStockDialog,
                        items_unit = @ItemsUnit,
                        default_unit = @DefaultUnit,
                        item_category = @ItemCategory,
                        party_wise_item_rate = @PartyWiseItemRate,
                        description = @Description,
                        item_wise_tax = @ItemWiseTax,
                        item_wise_discount = @ItemWiseDiscount,
                        update_sale_price_from_transaction = @UpdateSalePriceFromTransaction,
                        mrp_enabled = @MrpEnabled,
                        calculate_sale_price_from_mrp = @CalculateSalePriceFromMrp,
                        use_mrp_for_batch_tracking = @UseMrpForBatchTracking,
                        serial_no_tracking = @SerialNoTracking,
                        batch_no_enabled = @BatchNoEnabled,
                        exp_date_enabled = @ExpDateEnabled,
                        mfg_date_enabled = @MfgDateEnabled,
                        model_no_enabled = @ModelNoEnabled,
                        size_enabled = @SizeEnabled,
                        updated_at = NOW()
                    WHERE id = 1";

                await connection.ExecuteAsync(updateQuery, model);
            }
            else
            {
                // Insert new settings
                var insertQuery = @"
                    INSERT INTO item_settings (
                        id, enable_item, what_do_you_sell, barcode_scan, direct_barcode_scan,
                        stock_maintenance, manufacturing, show_low_stock_dialog, items_unit,
                        default_unit, item_category, party_wise_item_rate, description,
                        item_wise_tax, item_wise_discount, update_sale_price_from_transaction,
                        mrp_enabled, calculate_sale_price_from_mrp, use_mrp_for_batch_tracking,
                        serial_no_tracking, batch_no_enabled, exp_date_enabled, mfg_date_enabled,
                        model_no_enabled, size_enabled, created_at, updated_at
                    ) VALUES (
                        1, @EnableItem, @WhatDoYouSell, @BarcodeScanning, @DirectBarcodeScanning,
                        @StockMaintenance, @Manufacturing, @ShowLowStockDialog, @ItemsUnit,
                        @DefaultUnit, @ItemCategory, @PartyWiseItemRate, @Description,
                        @ItemWiseTax, @ItemWiseDiscount, @UpdateSalePriceFromTransaction,
                        @MrpEnabled, @CalculateSalePriceFromMrp, @UseMrpForBatchTracking,
                        @SerialNoTracking, @BatchNoEnabled, @ExpDateEnabled, @MfgDateEnabled,
                        @ModelNoEnabled, @SizeEnabled, NOW(), NOW()
                    )";

                await connection.ExecuteAsync(insertQuery, model);
            }

            TempData["SuccessMessage"] = "Item settings saved successfully!";
            return Json(new
            {
                success = true,
                message = "Settings saved successfully!",
                redirectUrl = Url.Action("Index", "ItemSettings", new { area = "Settings" })
            });
        }
        public async Task<IActionResult> SaveOrUpdateColumnSettings([FromBody] ItemSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();

                return Json(new { success = false, message = "Validation failed", errors = errors });
            }

            using var connection = new NpgsqlConnection(_connectionString);

            // Check if record exists (id = 1 for global settings)
            var existsQuery = "SELECT COUNT(*) FROM item_settings WHERE id = 1";
            var exists = await connection.QuerySingleAsync<int>(existsQuery) > 0;

            if (exists)
            {
                // Update existing record
                var updateQuery = @"
            UPDATE item_settings SET
                item_category = @Category,
                item_code = @ItemCode,
                hsn_sac_code = @HsnSacCode,
                description = @Description,
                item_wise_discount = @Discount
            WHERE id = 1";

                await connection.ExecuteAsync(updateQuery, new {
                    Category= model.ItemCategory,
                    ItemCode =  model.ItemCode,
                    HsnSacCode = model.HsnSacCode,
                    Description = model.Description,
                    Discount =  model.ItemWiseDiscount
                });
            }
            else
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO item_settings (
                id, item_category, item_code, hsn_sac_code, description, item_wise_discount, created_at, updated_at
            ) VALUES (
                1, @Category, @ItemCode, @HsnSacCode, @Description, @Discount, NOW(), NOW()
            )";

                await connection.ExecuteAsync(insertQuery, model);
            }

            TempData["SuccessMessage"] = "Column settings saved successfully!";
            return Json(new
            {
                success = true,
                message = "Settings saved successfully!",
                redirectUrl = Url.Action("create", "Sales")
            });
        }

    }
    public class ItemSettingsData
    {
        public bool enable_item { get; set; }
        public string? what_do_you_sell { get; set; }
        public bool barcode_scan { get; set; }
        public bool direct_barcode_scan { get; set; }
        public bool stock_maintenance { get; set; }
        public bool manufacturing { get; set; }
        public bool show_low_stock_dialog { get; set; }
        public bool items_unit { get; set; }
        public string? default_unit { get; set; }
        public bool item_category { get; set; }
        public bool party_wise_item_rate { get; set; }
        public bool description { get; set; }
        public bool item_wise_tax { get; set; }
        public bool item_wise_discount { get; set; }
        public bool update_sale_price_from_transaction { get; set; }
        public bool mrp_enabled { get; set; }
        public bool calculate_sale_price_from_mrp { get; set; }
        public bool use_mrp_for_batch_tracking { get; set; }
        public bool serial_no_tracking { get; set; }
        public bool batch_no_enabled { get; set; }
        public bool exp_date_enabled { get; set; }
        public bool mfg_date_enabled { get; set; }
        public bool model_no_enabled { get; set; }
        public bool size_enabled { get; set; }
    }
}
