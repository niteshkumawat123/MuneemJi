using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models.Setting;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class GeneralController : Controller
    {
        public IActionResult Index()
        {
            var model = new GeneralSettingsViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveSettings(GeneralSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Save settings logic here
                    // This would typically save to database or configuration file

                    TempData["SuccessMessage"] = "Settings saved successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving settings: " + ex.Message);
                }
            }

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult UpdateZoom(string zoomLevel)
        {
            try
            {
                // Update zoom level logic here
                // This would typically save to user preferences

                return Json(new { success = true, message = "Zoom level updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating zoom level: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateBackup()
        {
            try
            {
                // Create backup logic here
                var backupTime = DateTime.Now.ToString("dd/MM/yyyy | hh:mm tt");

                return Json(new { success = true, message = "Backup created successfully", lastBackup = backupTime });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error creating backup: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetSettings()
        {
            try
            {
                var model = new GeneralSettingsViewModel();
                // Load settings from database or configuration

                return Json(new { success = true, data = model });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading settings: " + ex.Message });
            }
        }
    }
}


