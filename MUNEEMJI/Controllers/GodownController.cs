using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    public class GodownController : Controller
    {
        private readonly IGodownService _godownService;

        public GodownController(IGodownService godownService)
        {
            _godownService = godownService;
        }

        // GET: Godown
        public async Task<IActionResult> Index()
        {
            var godowns = await _godownService.GetAllGodownsAsync();
            var totalCount = await _godownService.GetGodownCountAsync();

            var viewModel = new GodownViewModel
            {
                Godowns = godowns,
                TotalGodowns = totalCount
            };

            return View(viewModel);
        }

        // GET: Godown/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var godown = await _godownService.GetGodownByIdAsync(id);
            if (godown == null)
            {
                return NotFound();
            }

            return View(godown);
        }
        public IActionResult Create()
        {
            return View(new Godown());
        }

        // POST: Godown/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([FromBody] Godown godown)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var result = await _godownService.CreateGodownAsync(godown);
        //            if (result)
        //            {
        //                return Json(new { success = true, message = "Godown created successfully!" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "Failed to create godown." });
        //            }
        //        }

        //        var errors = ModelState
        //            .Where(x => x.Value.Errors.Count > 0)
        //            .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
        //            .ToList();

        //        return Json(new { success = false, message = "Validation failed.", errors = errors });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "An error occurred: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Godown godown)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _godownService.CreateGodownAsync(godown);
                    if (result)
                    {
                        return Json(new { success = true, message = "Godown created successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to create godown." });
                    }
                }

                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                    .ToList();

                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Godown godown)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _godownService.CreateGodownAsync(godown);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Godown created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create godown.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            return View(godown);
        }

        // GET: Godown/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var godown = await _godownService.GetGodownByIdAsync(id);
            if (godown == null)
            {
                return NotFound();
            }

            return View(godown);
        }

        // POST: Godown/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Godown godown)
        {
            if (id != godown.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _godownService.UpdateGodownAsync(godown);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Godown updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update godown.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            return View(godown);
        }

        // GET: Godown/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var godown = await _godownService.GetGodownByIdAsync(id);
            if (godown == null)
            {
                return NotFound();
            }

            return View(godown);
        }

        // POST: Godown/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _godownService.DeleteGodownAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Godown deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete godown.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Get godown types for dropdown
        [HttpGet]
        public IActionResult GetGodownTypes()
        {
            var godownTypes = new List<string>
            {
                "Retail Store",
                "Warehouse",
                "Distribution Center",
                "Manufacturing Unit",
                "Cold Storage",
                "Others"
            };

            return Json(godownTypes);
        }
    }

}
