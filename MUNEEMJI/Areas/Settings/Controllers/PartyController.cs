using Dapper;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class PartyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PartyController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = MUNEEMJI.DbConfig.ConnectionString; 
        }

        // GET: Settings/Party/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var partySettings = await GetPartySettingsAsync();
                return View(partySettings);
            }
            catch (Exception ex)
            {
                // Log exception
                ViewBag.Error = "Failed to load party settings.";
                return View(new PartySettingsViewModel());
            }
        }

        // POST: Settings/Party/SavePartySettings
        [HttpPost]
        public async Task<IActionResult> SavePartySettings([FromBody] PartySettingsViewModel model)
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

                var result = await SaveOrUpdatePartySettingsAsync(model);

                if (result)
                {
                    return Json(new { success = true, message = "Party settings saved successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save party settings." });
                }
            }
            catch (Exception ex)
            {
                // Log exception
                return Json(new { success = false, message = "An error occurred while saving party settings." });
            }
        }

        #region Private Methods

        private async Task<PartySettingsViewModel> GetPartySettingsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT 
                party_grouping,
                shipping_address,
                print_shipping_address,
                manage_party_status,
                enable_payment_reminder,
                payment_reminder_days,
                additional_field1_enabled,
                additional_field1,
                additional_field1_show_in_print,
                additional_field2_enabled,
                additional_field2,
                additional_field2_show_in_print,
                additional_field3_enabled,
                additional_field3,
                additional_field3_show_in_print,
                additional_field4_enabled,
                additional_field4,
                additional_field4_type,
                additional_field4_show_in_print,
                enable_loyalty_point,
                firm_id
            FROM party_settings 
            WHERE firm_id = @firmId";

            var parameters = new { firmId = 1 }; // You might want to get this from session/user context

            var partySettings = await connection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);

            if (partySettings != null)
            {
                return new PartySettingsViewModel
                {
                    PartyGrouping = partySettings.party_grouping ?? false,
                    ShippingAddress = partySettings.shipping_address ?? false,
                    PrintShippingAddress = partySettings.print_shipping_address ?? false,
                    ManagePartyStatus = partySettings.manage_party_status ?? false,
                    EnablePaymentReminder = partySettings.enable_payment_reminder ?? false,
                    PaymentReminderDays = partySettings.payment_reminder_days ?? 1,

                    AdditionalField1Enabled = partySettings.additional_field1_enabled ?? false,
                    AdditionalField1 = partySettings.additional_field1 ?? "",
                    AdditionalField1ShowInPrint = partySettings.additional_field1_show_in_print ?? false,

                    AdditionalField2Enabled = partySettings.additional_field2_enabled ?? false,
                    AdditionalField2 = partySettings.additional_field2 ?? "",
                    AdditionalField2ShowInPrint = partySettings.additional_field2_show_in_print ?? false,

                    AdditionalField3Enabled = partySettings.additional_field3_enabled ?? false,
                    AdditionalField3 = partySettings.additional_field3 ?? "",
                    AdditionalField3ShowInPrint = partySettings.additional_field3_show_in_print ?? false,

                    AdditionalField4Enabled = partySettings.additional_field4_enabled ?? false,
                    AdditionalField4 = partySettings.additional_field4 ?? "",
                    AdditionalField4Type = partySettings.additional_field4_type ?? "dd/mm/yy",
                    AdditionalField4ShowInPrint = partySettings.additional_field4_show_in_print ?? false,

                    EnableLoyaltyPoint = partySettings.enable_loyalty_point ?? false,
                    FirmId = partySettings.firm_id ?? 1
                };
            }

            // Return default settings if no record found
            return new PartySettingsViewModel
            {
                FirmId = 1,
                PaymentReminderDays = 1,
                AdditionalField4Type = "dd/mm/yy"
            };
        }

        private async Task<bool> SaveOrUpdatePartySettingsAsync(PartySettingsViewModel model)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if record exists
            var checkQuery = "SELECT COUNT(*) FROM party_settings WHERE firm_id = @firmId";
            var recordExists = await connection.QueryFirstAsync<int>(checkQuery, new { firmId = model.FirmId }) > 0;

            string query;
            if (recordExists)
            {
                // Update existing record
                query = @"
                UPDATE party_settings 
                SET 
                    party_grouping = @PartyGrouping,
                    shipping_address = @ShippingAddress,
                    print_shipping_address = @PrintShippingAddress,
                    manage_party_status = @ManagePartyStatus,
                    enable_payment_reminder = @EnablePaymentReminder,
                    payment_reminder_days = @PaymentReminderDays,
                    additional_field1_enabled = @AdditionalField1Enabled,
                    additional_field1 = @AdditionalField1,
                    additional_field1_show_in_print = @AdditionalField1ShowInPrint,
                    additional_field2_enabled = @AdditionalField2Enabled,
                    additional_field2 = @AdditionalField2,
                    additional_field2_show_in_print = @AdditionalField2ShowInPrint,
                    additional_field3_enabled = @AdditionalField3Enabled,
                    additional_field3 = @AdditionalField3,
                    additional_field3_show_in_print = @AdditionalField3ShowInPrint,
                    additional_field4_enabled = @AdditionalField4Enabled,
                    additional_field4 = @AdditionalField4,
                    additional_field4_type = @AdditionalField4Type,
                    additional_field4_show_in_print = @AdditionalField4ShowInPrint,
                    enable_loyalty_point = @EnableLoyaltyPoint,
                    updated_at = CURRENT_TIMESTAMP
                WHERE firm_id = @FirmId";
            }
            else
            {
                // Insert new record
                query = @"
                INSERT INTO party_settings (
                    firm_id,
                    party_grouping,
                    shipping_address,
                    print_shipping_address,
                    manage_party_status,
                    enable_payment_reminder,
                    payment_reminder_days,
                    additional_field1_enabled,
                    additional_field1,
                    additional_field1_show_in_print,
                    additional_field2_enabled,
                    additional_field2,
                    additional_field2_show_in_print,
                    additional_field3_enabled,
                    additional_field3,
                    additional_field3_show_in_print,
                    additional_field4_enabled,
                    additional_field4,
                    additional_field4_type,
                    additional_field4_show_in_print,
                    enable_loyalty_point,
                    created_at,
                    updated_at
                ) VALUES (
                    @FirmId,
                    @PartyGrouping,
                    @ShippingAddress,
                    @PrintShippingAddress,
                    @ManagePartyStatus,
                    @EnablePaymentReminder,
                    @PaymentReminderDays,
                    @AdditionalField1Enabled,
                    @AdditionalField1,
                    @AdditionalField1ShowInPrint,
                    @AdditionalField2Enabled,
                    @AdditionalField2,
                    @AdditionalField2ShowInPrint,
                    @AdditionalField3Enabled,
                    @AdditionalField3,
                    @AdditionalField3ShowInPrint,
                    @AdditionalField4Enabled,
                    @AdditionalField4,
                    @AdditionalField4Type,
                    @AdditionalField4ShowInPrint,
                    @EnableLoyaltyPoint,
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
