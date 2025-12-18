using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly string _connStr = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

        public IActionResult Index()
        {
            var loanAccounts = new List<LoanAccountModel>();

            string sql = @"
                SELECT 
                    id,
                    account_name,
                    lender_bank,
                    account_number,
                    description,
                    current_balance,
                    balance_as_of,
                    loan_received_in,
                    interest_rate,
                    term_duration,
                    processing_fee,
                    processing_fee_paid_from,
                    created_at,
                    updated_at
                FROM loan_accounts
                ORDER BY id DESC";

            var conn = new NpgsqlConnection(_connStr);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var loanAccount = new LoanAccountModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    AccountName = reader.IsDBNull(reader.GetOrdinal("account_name"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("account_name")),
                    LenderBank = reader.IsDBNull(reader.GetOrdinal("lender_bank"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("lender_bank")),
                    AccountNumber = reader.IsDBNull(reader.GetOrdinal("account_number"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("account_number")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("description")),
                    CurrentBalance = reader.IsDBNull(reader.GetOrdinal("current_balance"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("current_balance")),
                    BalanceAsOf = reader.IsDBNull(reader.GetOrdinal("balance_as_of"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("balance_as_of")),
                    LoanReceivedIn = reader.IsDBNull(reader.GetOrdinal("loan_received_in"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("loan_received_in")),
                    InterestRate = reader.IsDBNull(reader.GetOrdinal("interest_rate"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("interest_rate")),
                    TermDuration = reader.IsDBNull(reader.GetOrdinal("term_duration"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("term_duration")),
                    ProcessingFee = reader.IsDBNull(reader.GetOrdinal("processing_fee"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("processing_fee")),
                    ProcessingFeePaidFrom = reader.IsDBNull(reader.GetOrdinal("processing_fee_paid_from"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("processing_fee_paid_from")),


                };

                loanAccounts.Add(loanAccount);
            }

            conn.Close();

            return View(loanAccounts);
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

        public void SaveLoanAccount(LoanAccountModel model)
        {
            string sql;
            var conn = new NpgsqlConnection(_connStr);
            conn.Open();
            if (model.Id > 0)
            {
                // UPDATE
                sql = @"
            UPDATE loan_accounts
            SET
                account_name = @account_name,
                lender_bank = @lender_bank,
                account_number = @account_number,
                description = @description,
                current_balance = @current_balance,
                balance_as_of = @balance_as_of,
                loan_received_in = @loan_received_in,
                interest_rate = @interest_rate,
                term_duration = @term_duration,
                processing_fee = @processing_fee,
                processing_fee_paid_from = @processing_fee_paid_from,
                updated_at = NOW()
            WHERE id = @id;
        ";
            }
            else
            {
                // INSERT
                sql = @"
            INSERT INTO loan_accounts
            (
                account_name, lender_bank, account_number, description,
                current_balance, balance_as_of, loan_received_in,
                interest_rate, term_duration, processing_fee, processing_fee_paid_from
            )
            VALUES
            (
                @account_name, @lender_bank, @account_number, @description,
                @current_balance, @balance_as_of, @loan_received_in,
                @interest_rate, @term_duration, @processing_fee, @processing_fee_paid_from
            );
        ";
            }

            using var cmd = new NpgsqlCommand(sql, conn);

            if (model.Id > 0)
            {
                cmd.Parameters.AddWithValue("@id", model.Id);
            }

            cmd.Parameters.AddWithValue("@account_name", (object?)model.AccountName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@lender_bank", (object?)model.LenderBank ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@account_number", (object?)model.AccountNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)model.Description ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@current_balance", (object?)model.CurrentBalance ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@balance_as_of", (object?)model.BalanceAsOf ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@loan_received_in", (object?)model.LoanReceivedIn ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@interest_rate", (object?)model.InterestRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@term_duration", (object?)model.TermDuration ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@processing_fee", (object?)model.ProcessingFee ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@processing_fee_paid_from", (object?)model.ProcessingFeePaidFrom ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public List<LoanAccountModel> GetAllLoanAccounts()
        {
            var loanAccounts = new List<LoanAccountModel>();

            string sql = @"
                SELECT 
                    id,
                    account_name,
                    lender_bank,
                    account_number,
                    description,
                    current_balance,
                    balance_as_of,
                    loan_received_in,
                    interest_rate,
                    term_duration,
                    processing_fee,
                    processing_fee_paid_from,
                    created_at,
                    updated_at
                FROM loan_accounts
                ORDER BY id DESC";

            var conn = new NpgsqlConnection(_connStr);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var loanAccount = new LoanAccountModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    AccountName = reader.IsDBNull(reader.GetOrdinal("account_name"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("account_name")),
                    LenderBank = reader.IsDBNull(reader.GetOrdinal("lender_bank"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("lender_bank")),
                    AccountNumber = reader.IsDBNull(reader.GetOrdinal("account_number"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("account_number")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("description")),
                    CurrentBalance = reader.IsDBNull(reader.GetOrdinal("current_balance"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("current_balance")),
                    BalanceAsOf = reader.IsDBNull(reader.GetOrdinal("balance_as_of"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("balance_as_of")),
                    LoanReceivedIn = reader.IsDBNull(reader.GetOrdinal("loan_received_in"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("loan_received_in")),
                    InterestRate = reader.IsDBNull(reader.GetOrdinal("interest_rate"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("interest_rate")),
                    TermDuration = reader.IsDBNull(reader.GetOrdinal("term_duration"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("term_duration")),
                    ProcessingFee = reader.IsDBNull(reader.GetOrdinal("processing_fee"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("processing_fee")),
                    ProcessingFeePaidFrom = reader.IsDBNull(reader.GetOrdinal("processing_fee_paid_from"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("processing_fee_paid_from")),

                   
                };

                loanAccounts.Add(loanAccount);
            }

            conn.Close();
            return loanAccounts;
        }
    }
}
