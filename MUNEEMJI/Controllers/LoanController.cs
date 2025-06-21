using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;

namespace MUNEEMJI.Controllers
{
    public class LoanController : Controller
    {
        public IActionResult Index()
        {
            var model = new LoanDashboardViewModel
            {
                Account = new LoanAccountViewModel
                {
                    AccountName = "NITEH",
                    LendingBank = "Lending Bank",
                    Agency = "Agency: 1",
                    AccountNumber = "23456543245434",
                    BalanceAmount = 2345.00m,
                    Transactions = new List<LoanTransactionViewModel>
                    {
                        new LoanTransactionViewModel
                        {
                            Type = "Opening Loan",
                            Date = new DateTime(2025, 6, 17),
                            Principal = 2345.00m,
                            InterestAndOtherCharges = 0.00m,
                            TotalAmount = 2345.00m
                        }
                    }
                },
                PaymentFromOptions = new List<string> { "Cash", "Bank", "Other" }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult MakePayment()
        {
            var model = new LoanPaymentViewModel
            {
                Date = DateTime.Now,
                PaidFrom = "Cash"
            };
            return PartialView("_MakePaymentModal", model);
        }

        [HttpPost]
        public IActionResult MakePayment(LoanPaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process payment logic here
                TempData["Success"] = "Payment made successfully!";
                return RedirectToAction("Index");
            }
            return PartialView("_MakePaymentModal", model);
        }

        [HttpGet]
        public IActionResult TakeMoreLoan()
        {
            var model = new LoanIncreaseViewModel
            {
                Date = DateTime.Now,
                LoanReceivedIn = "Cash"
            };
            return PartialView("_TakeMoreLoanModal", model);
        }

        [HttpPost]
        public IActionResult TakeMoreLoan(LoanIncreaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process loan increase logic here
                TempData["Success"] = "Loan increased successfully!";
                return RedirectToAction("Index");
            }
            return PartialView("_TakeMoreLoanModal", model);
        }

        [HttpGet]
        public IActionResult ChargesOnLoan()
        {
            var model = new LoanChargesViewModel
            {
                Date = DateTime.Now,
                PaidFrom = "Cash"
            };
            return PartialView("_ChargesOnLoanModal", model);
        }

        [HttpPost]
        public IActionResult ChargesOnLoan(LoanChargesViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process charges logic here
                TempData["Success"] = "Charges added successfully!";
                return RedirectToAction("Index");
            }
            return PartialView("_ChargesOnLoanModal", model);
        }

        [HttpGet]
        public IActionResult ViewLoanStatement()
        {
            // Logic to generate and return loan statement
            var model = new LoanDashboardViewModel
            {
                Account = new LoanAccountViewModel
                {
                    AccountName = "NITEH",
                    LendingBank = "Lending Bank",
                    Agency = "Agency: 1",
                    AccountNumber = "23456543245434",
                    BalanceAmount = 2345.00m,
                    Transactions = new List<LoanTransactionViewModel>
                    {
                        new LoanTransactionViewModel
                        {
                            Type = "Opening Loan",
                            Date = new DateTime(2025, 6, 17),
                            Principal = 2345.00m,
                            InterestAndOtherCharges = 0.00m,
                            TotalAmount = 2345.00m
                        }
                    }
                }
            };
            return View("LoanStatement", model);
        }

        [HttpGet]
        public IActionResult AddLoanAccount()
        {
            var model = new LoanAccountViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddLoanAccount(LoanAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to add new loan account
                TempData["Success"] = "Loan account added successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
