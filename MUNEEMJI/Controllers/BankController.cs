using Insight.Database;
using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUNEEMJI.Models.BankAccount;
using MUNEEMJI.Services;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class BankController: Controller
    {
        private readonly string _connStr = MUNEEMJI.DbConfig.ConnectionString;
        private readonly ICompanyTenancy _companyTenancy;

        public BankController(ICompanyTenancy companyTenancy)
        {
            _companyTenancy = companyTenancy;
        }

        [HttpGet]
        public IActionResult AddBankAccount(int id = 0, int typeid = 0)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();
            BankAccountModel model = new BankAccountModel();

            if (id > 0)
            {
                using (var conn = new NpgsqlConnection(_connStr))
                {
                    string query = @"
                SELECT  
                    id                      AS ""Id"",
                    account_display_name    AS ""AccountDisplayName"",
                    opening_balance         AS ""OpeningBalance"",
                    as_of_date              AS ""AsOfDate"",
                    print_upi_qr             AS ""PrintUPIQrCode"",
                    print_bank_details      AS ""PrintBankDetails"",
                    account_number          AS ""AccountNumber"",
                    ifsc_code               AS ""IFSCCode"",
                    upi_id                  AS ""UPIID"",
                    bank_name               AS ""BankName"",
                    account_holder_name     AS ""AccountHolderName""
                FROM public.extended_bank_accounts
                WHERE id = @p_id AND companyid = @p_companyid;
            ";

                    model = conn
                        .QuerySql<BankAccountModel>(query, new { p_id = id, p_companyid = companyId })
                        .FirstOrDefault() ?? new BankAccountModel();

                    model.RequestTypeId = typeid;
                }
            }
            else
            {
                model.RequestTypeId = typeid;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult AddBankAccount(BankAccountModel model)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();

            if (string.IsNullOrWhiteSpace(model.AccountNumber))
            {
                ModelState.AddModelError("AccountNumber", "Account Number is required.");
                return View(model);
            }

            using (var conn = new NpgsqlConnection(_connStr))
            {
                conn.Open();

                string sql;

                if (model.Id > 0)
                {
                    // UPDATE
                    sql = @"
                                UPDATE extended_bank_accounts
                                SET
                                    account_display_name = @account_display_name,
                                    opening_balance = @opening_balance,
                                    as_of_date = @as_of_date,
                                    print_upi_qr = @print_upi_qr_code,
                                    print_bank_details = @print_bank_details,
                                    account_number = @account_number,
                                    ifsc_code = @ifsc_code,
                                    upi_id = @upi_id,
                                    bank_name = @bank_name,
                                    account_holder_name = @account_holder_name
                                WHERE id = @id AND companyid = @companyid;
                            ";
                }
                else
                {
                    // INSERT
                    sql = @"
                             INSERT INTO extended_bank_accounts
                             (
                                 account_display_name, opening_balance, as_of_date,
                                 print_upi_qr, print_bank_details,
                                 account_number, ifsc_code, upi_id,
                                 bank_name, account_holder_name, companyid
                             )
                             VALUES
                             (
                                 @account_display_name, @opening_balance, @as_of_date,
                                 @print_upi_qr_code, @print_bank_details,
                                 @account_number, @ifsc_code, @upi_id,
                                 @bank_name, @account_holder_name, @companyid
                             );
                         ";
                 }

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (model.Id > 0)
                    {
                        cmd.Parameters.AddWithValue("@id", model.Id);
                    }

                    cmd.Parameters.AddWithValue("@account_display_name", (object?)model.AccountDisplayName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@opening_balance", (object?)model.OpeningBalance ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@as_of_date", (object?)model.AsOfDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@print_upi_qr_code", model.PrintUPIQrCode);
                    cmd.Parameters.AddWithValue("@print_bank_details", model.PrintBankDetails);
                    cmd.Parameters.AddWithValue("@account_number", (object?)model.AccountNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ifsc_code", (object?)model.IFSCCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@upi_id", (object?)model.UPIID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@bank_name", (object?)model.BankName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@account_holder_name", (object?)model.AccountHolderName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@companyid", companyId);

                    cmd.ExecuteNonQuery();
                }


                conn.Close();
            }

            TempData["SuccessMessage"] = "Bank account saved successfully!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Index(int? id)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();

            // List to hold all bank accounts (basic info + details)
            var accounts = new List<BankAccountModel>();

            // Get connection string from configuration
            string connStr = MUNEEMJI.DbConfig.ConnectionString; 
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            // Query all accounts (id, display name, opening balance)
            string sqlAll = "SELECT id, account_display_name, opening_balance FROM extended_bank_accounts WHERE companyid = @p_companyid";
            using var cmd1 = new NpgsqlCommand(sqlAll, conn);
            cmd1.Parameters.AddWithValue("p_companyid", companyId);
            using var reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                accounts.Add(new BankAccountModel
                {
                    Id = reader.GetInt32(0),
                    AccountDisplayName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    OpeningBalance = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
                });
            }
            reader.Close();

            if (!accounts.Any())
            {
                // No accounts found; return empty list to view
                return View(accounts);
            }

            // If no id provided, use the first account's id
            if (id == null)
            {
                id = accounts[0].Id;
            }

            // Query details for the selected account (account number, IFSC, UPI, date)
            string sqlDetail = @"
                SELECT account_number, ifsc_code, upi_id, as_of_date 
                FROM extended_bank_accounts 
                WHERE id = @id AND companyid = @p_companyid";
            using var cmd2 = new NpgsqlCommand(sqlDetail, conn);
            cmd2.Parameters.AddWithValue("id", id.Value);
            cmd2.Parameters.AddWithValue("p_companyid", companyId);
            using var reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                // Find the matching BankAccountModel and set its detail fields
                var selectedAccount = accounts.First(a => a.Id == id.Value);
                selectedAccount.AccountNumber = reader2.GetString(0);
                selectedAccount.IFSCCode = reader2.GetString(1);
                selectedAccount.UPIID = reader2.GetString(2);
                selectedAccount.AsOfDate = reader2.GetDateTime(3);
            }
            reader2.Close();

            // Reorder the list so that the selected account is first (for display as Model[0])
            var sel = accounts.First(a => a.Id == id.Value);
            accounts.Remove(sel);
            accounts.Insert(0, sel);

            return View(accounts);
        }

        [HttpGet]
        public IActionResult CashInhand( )
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();

            using (var conn = new NpgsqlConnection(_connStr))
            {
                string query = @"
                    SELECT 
                        id AS ""Id"",
                        adjusttypeid AS ""AdjustTypeId"",
                        amount AS ""Amount"",
                        adjustmentdate AS ""AdjustmentDate"",
                        description AS ""Description""
                    FROM bank_cash
                    WHERE companyid = @p_companyid
                    ORDER BY adjustmentdate DESC";

                var transactions = conn.QuerySql<BankCash>(query, new { p_companyid = companyId }).ToList();
                if(transactions!=null && transactions.Count()>0)
                {
                    var addcash = transactions?
                        .Where(x => x.AdjustTypeId == 1)
                        .Sum(x => x.Amount) ?? 0;

                    var reducecash = transactions?
                        .Where(x => x.AdjustTypeId == 2)
                        .Sum(x => x.Amount) ?? 0;

                    var totalcashInHand = addcash - reducecash;

                    if (transactions != null)
                    {
                        foreach (var item in transactions)
                        {
                            item.TotalCash = totalcashInHand;
                        }
                    }

                }


                return View(transactions);
            }
        }

        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();

            using (var conn = new NpgsqlConnection(_connStr))
            {
                var QueryString = " delete from extended_bank_accounts where id = @p_id AND companyid = @p_companyid ";
                conn.ExecuteSql(QueryString, new { p_id = id, p_companyid = companyId });
            }
                return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult DeleteCashConfirmed(int id)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();

            using (var conn = new NpgsqlConnection(_connStr))
            {
                var QueryString = " delete from bank_cash where id = @p_id AND companyid = @p_companyid ";
                conn.ExecuteSql(QueryString, new { p_id = id, p_companyid = companyId });
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> AdjustCash([FromBody] AdjustCashRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var companyId = _companyTenancy.GetCurrentCompanyId();

                using (var connection = new NpgsqlConnection(_connStr))
                {
                    await connection.OpenAsync();

                    // Check if this is an update or insert operation
                    if (request.Id>0)
                    {
                        // UPDATE existing record
                        string updateQuery = @"
                    UPDATE bank_cash 
                    SET adjusttypeid = @adjustTypeId, 
                        amount = @amount, 
                        adjustmentdate = @adjustmentDate, 
                        description = @description
                    WHERE id = @id AND companyid = @companyId
                    RETURNING id";

                        using (var command = new NpgsqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", request.Id);
                            command.Parameters.AddWithValue("@adjustTypeId", request.AdjustTypeId);
                            command.Parameters.AddWithValue("@amount", request.Amount);
                            command.Parameters.AddWithValue("@adjustmentDate", request.AdjustmentDate);
                            command.Parameters.AddWithValue("@description",
                                string.IsNullOrEmpty(request.Description) ? DBNull.Value : (object)request.Description);
                            command.Parameters.AddWithValue("@companyId", companyId);

                            var updatedId = await command.ExecuteScalarAsync();

                            if (updatedId == null)
                            {
                                return NotFound(new
                                {
                                    success = false,
                                    message = "Transaction not found"
                                });
                            }

                            return Ok(new
                            {
                                success = true,
                                message = "Transaction updated successfully",
                                id = updatedId
                            });
                        }
                    }
                    else
                    {
                        // INSERT new record
                        string insertQuery = @"
                    INSERT INTO bank_cash (adjusttypeid, amount, adjustmentdate, description, companyid) 
                    VALUES (@adjustTypeId, @amount, @adjustmentDate, @description, @companyId)
                    RETURNING id";

                        using (var command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@adjustTypeId", request.AdjustTypeId);
                            command.Parameters.AddWithValue("@amount", request.Amount);
                            command.Parameters.AddWithValue("@adjustmentDate", request.AdjustmentDate);
                            command.Parameters.AddWithValue("@description",
                                string.IsNullOrEmpty(request.Description) ? DBNull.Value : (object)request.Description);
                            command.Parameters.AddWithValue("@companyId", companyId);

                            var insertedId = await command.ExecuteScalarAsync();

                            return Ok(new
                            {
                                success = true,
                                message = "Cash adjusted successfully",
                                id = insertedId
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                Console.WriteLine($"Error: {ex.Message}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while saving data",
                    error = ex.Message
                });
            }
        }
        // Optional: Get all transactions
        [HttpGet]
        public IActionResult GetCashTransactionById(int id)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();

            using (var conn = new NpgsqlConnection(_connStr))
            {
                string query = @"
                    SELECT 
                        id AS ""Id"",
                        adjusttypeid AS ""AdjustTypeId"",
                        amount AS ""Amount"",
                        adjustmentdate AS ""AdjustmentDate"",
                        description AS ""Description""
                    FROM bank_cash
                    WHERE id = @p_id AND companyid = @p_companyid";

                var transaction = conn.QuerySql<BankCash>(query, new { p_id = id, p_companyid = companyId }).FirstOrDefault();

                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                return Ok(transaction);
            }
        }

        // Optional: Get current cash balance
        [HttpGet("GetCashBalance")]
        public async Task<IActionResult> GetCashBalance()
        {
            try
            {
                var companyId = _companyTenancy.GetCurrentCompanyId();

                string query = @"
                    SELECT 
                        COALESCE(SUM(CASE WHEN adjusttypeid = 1 THEN amount ELSE -amount END), 0) as balance
                    FROM bank_cash
                    WHERE companyid = @companyId";

                using (var connection = new NpgsqlConnection(_connStr))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@companyId", companyId);
                        var balance = await command.ExecuteScalarAsync();

                        return Ok(new
                        {
                            balance = Convert.ToDecimal(balance)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while calculating balance",
                    error = ex.Message
                });
            }
        }
    }
}


