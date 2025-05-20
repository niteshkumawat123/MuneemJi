using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;

namespace MUNEEMJI.Controllers
{
  
        public class PartyController : Controller
        {
            [HttpGet]
            public IActionResult Add()
            {
                return View(new PartyModel());
            }

            [HttpPost]
            public IActionResult Add(PartyModel model, string save)
            {
                if (ModelState.IsValid)
                {
                    // Save to database logic here...

                    if (save == "new")
                    {
                        TempData["Message"] = "Party saved. Ready to add new.";
                        return RedirectToAction("Add");
                    }

                    TempData["Message"] = "Party saved successfully.";
                    return RedirectToAction("Index");
                }

                return View(model);
            }

            public IActionResult Index()
            {
                // List view of all parties
                return View();
            }
        }
    
}
