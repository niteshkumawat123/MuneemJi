using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class SettingsController : Controller
    {
        private readonly ITransactionSettingsService _transactionSettingsService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(
            ITransactionSettingsService transactionSettingsService,
            ILogger<SettingsController> logger)
        {
            _transactionSettingsService = transactionSettingsService;
            _logger = logger;
        }

        // GET: Settings/SettingTransaction
        [HttpGet]
        public async Task<IActionResult> SettingTransaction()
        {
            try
            {
                var settings = await _transactionSettingsService.GetTransactionSettingsAsync();

                // If no settings exist, return default model
                if (settings == null)
                {
                    settings = new TransactionSettingsModel();
                }

                return View(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading transaction settings");
                TempData["ErrorMessage"] = "An error occurred while loading the settings. Please try again.";
                return View(new TransactionSettingsModel());
            }
        }

        // POST: Settings/SettingTransaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SettingTransaction(TransactionSettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return View(model);
            }

            try
            {
                var result = await _transactionSettingsService.SaveTransactionSettingsAsync(model);

                if (result)
                {
                    TempData["SuccessMessage"] = "Transaction settings have been saved successfully.";
                    _logger.LogInformation("Transaction settings updated successfully");

                    // Redirect to prevent form resubmission
                    return RedirectToAction(nameof(SettingTransaction));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to save transaction settings. Please try again.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving transaction settings");
                TempData["ErrorMessage"] = "An error occurred while saving the settings. Please try again.";
                return View(model);
            }
        }

        // GET: Settings/GetTransactionPrefixes - AJAX endpoint for dynamic prefix loading
        [HttpGet]
        public async Task<IActionResult> GetTransactionPrefixes(string firm)
        {
            try
            {
                var prefixes = await _transactionSettingsService.GetTransactionPrefixesAsync(firm);
                return Json(prefixes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading transaction prefixes for firm: {Firm}", firm);
                return Json(new { error = "Failed to load prefixes" });
            }
        }

        // POST: Settings/ResetTransactionSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetTransactionSettings()
        {
            try
            {
                var result = await _transactionSettingsService.ResetToDefaultSettingsAsync();

                if (result)
                {
                    TempData["SuccessMessage"] = "Transaction settings have been reset to default values.";
                    _logger.LogInformation("Transaction settings reset to default");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reset transaction settings.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting transaction settings");
                TempData["ErrorMessage"] = "An error occurred while resetting the settings.";
            }

            return RedirectToAction(nameof(SettingTransaction));
        }

        // GET: Settings/ExportTransactionSettings
        [HttpGet]
        public async Task<IActionResult> ExportTransactionSettings()
        {
            try
            {
                var settings = await _transactionSettingsService.GetTransactionSettingsAsync();
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);
                var fileName = $"transaction_settings_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                return File(bytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while exporting transaction settings");
                TempData["ErrorMessage"] = "Failed to export transaction settings.";
                return RedirectToAction(nameof(SettingTransaction));
            }
        }

        // POST: Settings/ImportTransactionSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportTransactionSettings(IFormFile settingsFile)
        {
            if (settingsFile == null || settingsFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid settings file.";
                return RedirectToAction(nameof(SettingTransaction));
            }

            try
            {
                using (var stream = settingsFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var jsonContent = await reader.ReadToEndAsync();
                    var settings = System.Text.Json.JsonSerializer.Deserialize<TransactionSettingsModel>(jsonContent);

                    if (settings != null)
                    {
                        var result = await _transactionSettingsService.SaveTransactionSettingsAsync(settings);

                        if (result)
                        {
                            TempData["SuccessMessage"] = "Transaction settings imported successfully.";
                            _logger.LogInformation("Transaction settings imported from file: {FileName}", settingsFile.FileName);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Failed to import transaction settings.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid settings file format.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing transaction settings from file: {FileName}", settingsFile?.FileName);
                TempData["ErrorMessage"] = "Failed to import transaction settings. Please check the file format.";
            }

            return RedirectToAction(nameof(SettingTransaction));
        }

        // POST: Settings/ValidateSettings - AJAX endpoint for client-side validation
        [HttpPost]
        public IActionResult ValidateSettings([FromBody] TransactionSettingsModel model)
        {
            var validationErrors = new List<string>();

            // Custom validation logic
            if (model.RoundOffTotal && string.IsNullOrWhiteSpace(model.RoundOffTo))
            {
                validationErrors.Add("Round Off To is required when Round Off Total is enabled.");
            }

            if (model.EnablePasscodeForTransactionEditDelete && string.IsNullOrWhiteSpace(model.Firm))
            {
                validationErrors.Add("Firm selection is required when passcode protection is enabled.");
            }

            return Json(new
            {
                isValid = validationErrors.Count == 0,
                errors = validationErrors
            });
        }
    }

    // Service Interface (you'll need to implement this)
    public interface ITransactionSettingsService
    {
        Task<TransactionSettingsModel> GetTransactionSettingsAsync();
        Task<bool> SaveTransactionSettingsAsync(TransactionSettingsModel settings);
        Task<bool> ResetToDefaultSettingsAsync();
        Task<Dictionary<string, List<string>>> GetTransactionPrefixesAsync(string firm);
    }

    // Sample Service Implementation
    public class TransactionSettingsService : ITransactionSettingsService
    {
        private readonly ILogger<TransactionSettingsService> _logger;
        // Add your data context or repository here
        // private readonly ApplicationDbContext _context;

        public TransactionSettingsService(ILogger<TransactionSettingsService> logger)
        {
            _logger = logger;
            // _context = context;
        }

        public async Task<TransactionSettingsModel> GetTransactionSettingsAsync()
        {
            try
            {
                // Implement your data retrieval logic here
                // Example: return await _context.TransactionSettings.FirstOrDefaultAsync();

                // For now, return default settings
                await Task.Delay(1); // Simulate async operation
                return new TransactionSettingsModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction settings");
                throw;
            }
        }

        public async Task<bool> SaveTransactionSettingsAsync(TransactionSettingsModel settings)
        {
            try
            {
                // Implement your data saving logic here
                // Example:
                // var existingSettings = await _context.TransactionSettings.FirstOrDefaultAsync();
                // if (existingSettings == null)
                // {
                //     _context.TransactionSettings.Add(settings);
                // }
                // else
                // {
                //     // Update existing settings
                //     _context.Entry(existingSettings).CurrentValues.SetValues(settings);
                // }
                // return await _context.SaveChangesAsync() > 0;

                await Task.Delay(100); // Simulate async operation
                _logger.LogInformation("Transaction settings saved successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving transaction settings");
                return false;
            }
        }

        public async Task<bool> ResetToDefaultSettingsAsync()
        {
            try
            {
                var defaultSettings = new TransactionSettingsModel();
                return await SaveTransactionSettingsAsync(defaultSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting transaction settings");
                return false;
            }
        }

        public async Task<Dictionary<string, List<string>>> GetTransactionPrefixesAsync(string firm)
        {
            try
            {
                // Implement logic to get prefixes based on firm
                await Task.Delay(1); // Simulate async operation

                return new Dictionary<string, List<string>>
                {
                    { "Sale", new List<string> { "None", "INV", "SALE" } },
                    { "CreditNote", new List<string> { "None", "CN", "CREDIT" } },
                    { "SaleOrder", new List<string> { "None", "SO", "ORDER" } },
                    { "PurchaseOrder", new List<string> { "None", "PO", "PORDER" } },
                    { "Estimate", new List<string> { "None", "EST", "QUOTE" } },
                    { "DeliveryChallan", new List<string> { "None", "DC", "CHALLAN" } },
                    { "PaymentIn", new List<string> { "None", "PI", "PAYMENT" } }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction prefixes for firm: {Firm}", firm);
                throw;
            }
        }
    }

}
