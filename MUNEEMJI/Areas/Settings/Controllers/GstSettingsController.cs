using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models.Setting;
using MUNEEMJI.Services;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class GstSettingsController : Controller
    {
        private readonly IGstSettingsService _gstSettingsService;

        public GstSettingsController(IGstSettingsService gstSettingsService)
        {
            _gstSettingsService = gstSettingsService;
        }

        public IActionResult Index()
        {
            var model = _gstSettingsService.GetGstSettings(1);
            if (model == null) model = new GstSettingsModel();

            ViewBag.TaxRates = _gstSettingsService.GetTaxRates(1);
            ViewBag.TaxGroups = _gstSettingsService.GetTaxGroups(1);

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveSettings([FromBody] GstSettingsModel model)
        {
            model.FirmId = 1;
            bool res = _gstSettingsService.SaveGstSettings(model);
            return Json(new { success = res, message = res ? "Saved successfully" : "Error saving" });
        }

        [HttpPost]
        public IActionResult SaveTaxRate([FromBody] TaxRateModel model)
        {
            model.FirmId = 1;
            bool res = _gstSettingsService.SaveTaxRate(model);
            return Json(new { success = res, message = res ? "Saved successfully" : "Error saving" });
        }

        [HttpPost]
        public IActionResult DeleteTaxRate(int id)
        {
            bool res = _gstSettingsService.DeleteTaxRate(id);
            return Json(new { success = res, message = res ? "Deleted successfully" : "Error deleting" });
        }

        [HttpPost]
        public IActionResult SaveTaxGroup([FromBody] TaxGroupModel model)
        {
            model.FirmId = 1;
            int newId = _gstSettingsService.SaveTaxGroup(model);
            return Json(new { success = newId > 0, message = newId > 0 ? "Saved successfully" : "Error saving" });
        }

        [HttpPost]
        public IActionResult DeleteTaxGroup(int id)
        {
            bool res = _gstSettingsService.DeleteTaxGroup(id);
            return Json(new { success = res, message = res ? "Deleted successfully" : "Error deleting" });
        }
    }
}
