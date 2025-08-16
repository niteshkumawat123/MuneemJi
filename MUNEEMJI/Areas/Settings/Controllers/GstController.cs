using Dapper;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class GstController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public GstController(IConfiguration configuration)
        {
            _configuration = configuration;
              _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            
        }

        // GET: Settings/Gst/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var gstSettings = await GetGstSettingsAsync();
                return View(gstSettings);
            }
            catch (Exception ex)
            {
                // Log exception
                ViewBag.Error = "Failed to load GST settings.";
                return View(new GstSettingsViewModel());
            }
        }

        // POST: Settings/Gst/SaveGstSettings
        [HttpPost]
        public async Task<IActionResult> SaveGstSettings([FromBody] GstSettingsViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Invalid data received." });
                }

                // Set default FirmId if not provided (you might want to get this from session/user context)
                if (model.FirmId == 0)
                    model.FirmId = 1; // Default firm

                var result = await SaveOrUpdateGstSettingsAsync(model);

                if (result)
                {
                    return Json(new { success = true, message = "GST settings saved successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save GST settings." });
                }
            }
            catch (Exception ex)
            {
                // Log exception
                return Json(new { success = false, message = "An error occurred while saving GST settings." });
            }
        }

        #region Private Methods

        private async Task<GstSettingsViewModel> GetGstSettingsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT 
                enable_gst,
                enable_hsn_sac_code,
                additional_cess_on_item,
                reverse_charge,
                enable_place_of_supply,
                composite_scheme,
                composite_scheme_type,
                enable_tcs,
                enable_tds,
                firm_id
            FROM gst_settings 
            WHERE firm_id = @firmId";

            var parameters = new { firmId = 1 }; // You might want to get this from session/user context

            var gstSettings = await connection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);

            if (gstSettings != null)
            {
                return new GstSettingsViewModel
                {
                    EnableGst = gstSettings.enable_gst ?? false,
                    EnableHsnSacCode = gstSettings.enable_hsn_sac_code ?? false,
                    AdditionalCessOnItem = gstSettings.additional_cess_on_item ?? false,
                    ReverseCharge = gstSettings.reverse_charge ?? false,
                    EnablePlaceOfSupply = gstSettings.enable_place_of_supply ?? false,
                    CompositeScheme = gstSettings.composite_scheme ?? false,
                    CompositeSchemeType = gstSettings.composite_scheme_type ?? "Manufacturer 1.0%",
                    EnableTcs = gstSettings.enable_tcs ?? false,
                    EnableTds = gstSettings.enable_tds ?? false,
                    FirmId = gstSettings.firm_id ?? 1
                };
            }

            // Return default settings if no record found
            return new GstSettingsViewModel
            {
                FirmId = 1,
                CompositeSchemeType = "Manufacturer 1.0%"
            };
        }

        private async Task<bool> SaveOrUpdateGstSettingsAsync(GstSettingsViewModel model)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if record exists
            var checkQuery = "SELECT COUNT(*) FROM gst_settings WHERE firm_id = @firmId";
            var recordExists = await connection.QueryFirstAsync<int>(checkQuery, new { firmId = model.FirmId }) > 0;

            string query;
            if (recordExists)
            {
                // Update existing record
                query = @"
                UPDATE gst_settings 
                SET 
                    enable_gst = @EnableGst,
                    enable_hsn_sac_code = @EnableHsnSacCode,
                    additional_cess_on_item = @AdditionalCessOnItem,
                    reverse_charge = @ReverseCharge,
                    enable_place_of_supply = @EnablePlaceOfSupply,
                    composite_scheme = @CompositeScheme,
                    composite_scheme_type = @CompositeSchemeType,
                    enable_tcs = @EnableTcs,
                    enable_tds = @EnableTds,
                    updated_at = CURRENT_TIMESTAMP
                WHERE firm_id = @FirmId";
            }
            else
            {
                // Insert new record
                query = @"
                INSERT INTO gst_settings (
                    firm_id,
                    enable_gst,
                    enable_hsn_sac_code,
                    additional_cess_on_item,
                    reverse_charge,
                    enable_place_of_supply,
                    composite_scheme,
                    composite_scheme_type,
                    enable_tcs,
                    enable_tds,
                    created_at,
                    updated_at
                ) VALUES (
                    @FirmId,
                    @EnableGst,
                    @EnableHsnSacCode,
                    @AdditionalCessOnItem,
                    @ReverseCharge,
                    @EnablePlaceOfSupply,
                    @CompositeScheme,
                    @CompositeSchemeType,
                    @EnableTcs,
                    @EnableTds,
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP
                )";
            }

            var rowsAffected = await connection.ExecuteAsync(query, model);
            return rowsAffected > 0;
        }

        #endregion
    }
}
