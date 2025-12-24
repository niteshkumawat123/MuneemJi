using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MUNEEMJI.Models;
using Npgsql;
using SkiaSharp;
using System.Transactions;

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

        [HttpPost]
        public void SaveLoanAccount([FromBody] LoanAccountModel model)
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

        [HttpGet]
        public JsonResult GetLoanAccountDetails(int id)
        {
            try
            {
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
                processing_fee_paid_from
            FROM loan_accounts
            WHERE id = @id";

                using var conn = new NpgsqlConnection(_connStr);
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var account = new
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
                        CurrentBalance = reader.IsDBNull(reader.GetOrdinal("current_balance"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("current_balance")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("description")),
                        InterestRate = reader.IsDBNull(reader.GetOrdinal("interest_rate"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("interest_rate")),
                        TermDuration = reader.IsDBNull(reader.GetOrdinal("term_duration"))
                            ? 0
                            : reader.GetInt32(reader.GetOrdinal("term_duration"))
                    };

                    return Json(account);
                }

                return Json(null);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetLoanAccountDetails: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetLoanTransactions(int accountId)
        {
            try
            {
                string sql = @"
            SELECT 
                id,
                loanaccountid,
                transactiontype,
                principalamount,
                interestamount,
                totalamount,
                transactiondate,
                paymentmethod,
                interestrate,
                termduration,
                description,
                createddate
            FROM loantransactions
            WHERE loanaccountid = @loanaccountid
            ORDER BY transactiondate DESC, createddate DESC";

                using var conn = new NpgsqlConnection(_connStr);
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@loanaccountid", accountId);

                var transactions = new List<object>();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var transaction = new
                    {
                        id = reader.GetInt32(reader.GetOrdinal("id")),
                        LoanAccountId = reader.IsDBNull(reader.GetOrdinal("loanaccountid"))
                            ? 0
                            : reader.GetInt32(reader.GetOrdinal("loanaccountid")),
                        transactionType = reader.IsDBNull(reader.GetOrdinal("transactiontype"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("transactiontype")),
                        principalAmount = reader.IsDBNull(reader.GetOrdinal("principalamount"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("principalamount")),
                        interestAmount = reader.IsDBNull(reader.GetOrdinal("interestamount"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("interestamount")),
                        totalAmount = reader.IsDBNull(reader.GetOrdinal("totalamount"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("totalamount")),
                        transactionDate = reader.IsDBNull(reader.GetOrdinal("transactiondate"))
                            ? (DateTime?)null
                            : reader.GetDateTime(reader.GetOrdinal("transactiondate")),
                        PaymentMethod = reader.IsDBNull(reader.GetOrdinal("paymentmethod"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("paymentmethod")),
                        InterestRate = reader.IsDBNull(reader.GetOrdinal("interestrate"))
                            ? (decimal?)null
                            : reader.GetDecimal(reader.GetOrdinal("interestrate")),
                        TermDuration = reader.IsDBNull(reader.GetOrdinal("termduration"))
                            ? (int?)null
                            : reader.GetInt32(reader.GetOrdinal("termduration")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("description")),
                        CreatedDate = reader.IsDBNull(reader.GetOrdinal("createddate"))
                            ? (DateTime?)null
                            : reader.GetDateTime(reader.GetOrdinal("createddate"))
                    };
                    transactions.Add(transaction);
                }

                return Json(transactions);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetLoanTransactions: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }


        public IActionResult MakePayment()
        {
            return PartialView("_MakePayment");
        }

        [HttpPost]
        public IActionResult SaveMakePayment([FromBody] MakePaymentModel model)
        {
            try
            {

                string sql;
                var conn = new NpgsqlConnection(_connStr);
                conn.Open();

                if (model.Id > 0)
                {
                    sql = @"
                                UPDATE loantransactions
                                SET
                                    loanaccountid = @loanaccountid,
                                    transactiontype = @transactiontype,
                                    principalamount = @principalamount,
                                    interestamount = @interestamount,
                                    totalamount = @totalamount,
                                    transactiondate = @transactiondate,
                                    paymentmethod = @paymentmethod
                                WHERE id = @id;
                            ";
                }
                else
                {
                    sql = @"
                                INSERT INTO loantransactions
                                (
                                    loanaccountid, transactiontype, principalamount, interestamount,
                                    totalamount, transactiondate, paymentmethod
                                )
                                VALUES
                                (
                                    @loanaccountid, @transactiontype, @principalamount, @interestamount,
                                    @totalamount, @transactiondate, @paymentmethod
                                );
                            ";
                }

                using var cmd = new NpgsqlCommand(sql, conn);

                if (model.Id > 0)
                {
                    cmd.Parameters.AddWithValue("@id", model.Id);
                }

                cmd.Parameters.AddWithValue("@loanaccountid", model.LoanAccountId);
                cmd.Parameters.AddWithValue("@transactiontype", "EMI Paid");
                cmd.Parameters.AddWithValue("@principalamount", (object?)model.PrincipalAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@interestamount", (object?)model.InterestAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@totalamount", (object?)model.TotalAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@transactiondate", model.Date);
                cmd.Parameters.AddWithValue("@paymentmethod", (object?)model.PaidFrom ?? DBNull.Value);

                cmd.ExecuteNonQuery();

                conn.Close();

                return Json(new { success = true, message = "Payment saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult TakeMoreLoan()
        {
            return PartialView("_TakeMoreLoan");
        }

        [HttpPost]
        public IActionResult SaveTakeMoreLoan([FromBody] TakeMoreLoanModel model)
        {
            string sql;
            var conn = new NpgsqlConnection(_connStr);
            conn.Open();
            try
            {

                if (model.Id > 0)
                {
                    // UPDATE existing transaction
                    sql = @"
            UPDATE loantransactions
            SET
                loanaccountid = @loanaccountid,
                transactiontype = @transactiontype,
                principalamount = @principalamount,
                interestamount = @interestamount,
                totalamount = @totalamount,
                transactiondate = @transactiondate,
                paymentmethod = @paymentmethod
            WHERE id = @id;
        ";
                }
                else
                {
                    // INSERT new transaction
                    sql = @"
            INSERT INTO loantransactions
            (
                loanaccountid, transactiontype, principalamount, interestamount,
                totalamount, transactiondate, paymentmethod
            )
            VALUES
            (
                @loanaccountid, @transactiontype, @principalamount, @interestamount,
                @totalamount, @transactiondate, @paymentmethod
            );
        ";
                }

                using var cmd = new NpgsqlCommand(sql, conn);

                if (model.Id > 0)
                {
                    cmd.Parameters.AddWithValue("@id", model.Id);
                }

                cmd.Parameters.AddWithValue("@loanaccountid", model.LoanAccountId);
                cmd.Parameters.AddWithValue("@transactiontype", "Loan Adjustment");
                cmd.Parameters.AddWithValue("@principalamount", 0);
                cmd.Parameters.AddWithValue("@interestamount", 0);
                cmd.Parameters.AddWithValue("@totalamount", (object?)model.LoanAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@transactiondate", model.Date);
                cmd.Parameters.AddWithValue("@paymentmethod", (object?)model.LoanReceivedIn ?? DBNull.Value);

                cmd.ExecuteNonQuery();

                conn.Close();
                return Json(new { success = true, message = "Loan Transection Creadited succssfully" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public IActionResult ChargesOnLoan()
        {
            return PartialView("_ChargesOnLoan");
        }

        [HttpPost]
        public IActionResult SaveChargesOnLoan([FromBody] ChargesOnLoanModel model)
        {
            string sql;
            var conn = new NpgsqlConnection(_connStr);
            conn.Open();
            try
            {

                if (model.Id > 0)
                {
                    // UPDATE existing transaction
                    sql = @"
            UPDATE loantransactions
            SET
                loanaccountid = @loanaccountid,
                transactiontype = @transactiontype,
                principalamount = @principalamount,
                interestamount = @interestamount,
                totalamount = @totalamount,
                transactiondate = @transactiondate,
                paymentmethod = @paymentmethod
            WHERE id = @id;
        ";
                }
                else
                {
                    // INSERT new transaction
                    sql = @"
            INSERT INTO loantransactions
            (
                loanaccountid, transactiontype, principalamount, interestamount,
                totalamount, transactiondate, paymentmethod
            )
            VALUES
            (
                @loanaccountid, @transactiontype, @principalamount, @interestamount,
                @totalamount, @transactiondate, @paymentmethod
            );
        ";
                }

                using var cmd = new NpgsqlCommand(sql, conn);

                if (model.Id > 0)
                {
                    cmd.Parameters.AddWithValue("@id", model.Id);
                }

                cmd.Parameters.AddWithValue("@loanaccountid", model.LoanAccountId);
                cmd.Parameters.AddWithValue("@transactiontype", model.TransactionTypeName);
                cmd.Parameters.AddWithValue("@principalamount", 0);
                cmd.Parameters.AddWithValue("@interestamount", 0);
                cmd.Parameters.AddWithValue("@totalamount", (object?)model.Amount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@transactiondate", model.Date);
                cmd.Parameters.AddWithValue("@paymentmethod", (object?)model.LoanReceivedIn ?? DBNull.Value);

                cmd.ExecuteNonQuery();

                conn.Close();
                return Json(new { success = true, message = "Loan Transection Creadited succssfully" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

    }
}
