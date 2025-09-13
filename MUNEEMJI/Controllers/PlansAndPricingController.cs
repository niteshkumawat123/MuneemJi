using Microsoft.AspNetCore.Mvc;

namespace MUNEEMJI.Controllers
{
    public class PlansAndPricingController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpgradeToPro()
        {
            // Handle upgrade logic here
            // For example: redirect to payment gateway
            return RedirectToAction("Index");
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Business()
        {
            return View();
        }

        public IActionResult Preferences()
        {
            return View();
        }

        public IActionResult Support()
        {
            return View();
        }
    }
}
