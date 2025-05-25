using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUNEEMJI.Models.BankAccount;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class BankController: Controller
    {
        private readonly string _connStr = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

        [HttpGet]
        public IActionResult AddBankAccount()
        {
            return View(new BankAccountModel());
        }

        [HttpPost]
        public IActionResult AddBankAccount(BankAccountModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var conn = new NpgsqlConnection(_connStr))
            {
                conn.Open();

                string sql = @"
            INSERT INTO extended_bank_accounts 
            (account_display_name, opening_balance, as_of_date, print_upi_qr, 
             print_bank_details, account_number, ifsc_code, upi_id, bank_name, account_holder_name)
            VALUES 
            (@account_display_name, @opening_balance, @as_of_date, @print_upi_qr_code,
             @print_bank_details, @account_number, @ifsc_code, @upi_id, @bank_name, @account_holder_name);
        ";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
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
            // List to hold all bank accounts (basic info + details)
            var accounts = new List<BankAccountModel>();

            // Get connection string from configuration
            string connStr = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022"; 
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();  // Open the PostgreSQL connection:contentReference[oaicite:6]{index=6}

            // Query all accounts (id, display name, opening balance)
            string sqlAll = "SELECT id, account_display_name, opening_balance FROM extended_bank_accounts";
            using var cmd1 = new NpgsqlCommand(sqlAll, conn);
            using var reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                accounts.Add(new BankAccountModel
                {
                    Id = reader.GetInt32(0),
                    AccountDisplayName = reader.GetString(1),
                    OpeningBalance = reader.GetDecimal(2)
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
                WHERE id = @id";
            using var cmd2 = new NpgsqlCommand(sqlDetail, conn);
            cmd2.Parameters.AddWithValue("id", id.Value);
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
    }
}
