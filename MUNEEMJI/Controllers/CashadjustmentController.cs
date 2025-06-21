using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Models.BusinessDashboard.Models;
using System.Transactions;


namespace MUNEEMJI.Controllers
{
    public class CashadjustmentController : Controller
    {
        private static List<CashAdjustmentModel> _transactions = new List<CashAdjustmentModel>
        {
            new CashAdjustmentModel
            {
                Id = 1,
                Type = "Purchase",
                Name = "test",
                Date = new DateTime(2025, 6, 14),
                Amount = 12.00m
            }
        };

        private static decimal _cashInHand = 12.00m;

        public IActionResult Index(string searchTerm = "")
        {
            var filteredTransactions = _transactions.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredTransactions = filteredTransactions.Where(t =>
                    t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var viewModel = new CashAdjustmenttransactionsViewModel
            {
                CashInHand = _cashInHand,
                Transactions = filteredTransactions.OrderByDescending(t => t.Date).ToList(),
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AdjustCash(CashAdjustmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Update cash in hand based on adjustment
                if (model.AdjustmentType == "Add Cash")
                {
                    _cashInHand += model.AdjustmentAmount;
                }
                else if (model.AdjustmentType == "Remove Cash")
                {
                    _cashInHand -= model.AdjustmentAmount;
                }

                // Add transaction record
                var transaction = new CashAdjustmentModel
                {
                    Id = _transactions.Count + 1,
                    Type = model.AdjustmentType,
                    Name = string.IsNullOrEmpty(model.Description) ? "Cash Adjustment" : model.Description,
                    Date = model.AdjustmentDate,
                    Amount = model.AdjustmentAmount
                };

                _transactions.Add(transaction);

                return Json(new { success = true, newCashAmount = _cashInHand });
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpGet]
        public IActionResult GetCashAdjustmentModal()
        {
            var model = new CashAdjustmentViewModel
            {
                CurrentCashInHand = _cashInHand,
                AdjustmentDate = DateTime.Now
            };

            return PartialView("_CashAdjustmentModal", model);
        }
    }
}

