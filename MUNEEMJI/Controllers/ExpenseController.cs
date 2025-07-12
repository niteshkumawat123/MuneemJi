using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MUNEEMJI.Models;
using Npgsql;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Data;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly string _connectionString;

        public ExpenseController(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        public IActionResult Index(int category = 1)
        {
            var Value = GetExpensesByCategory(category);
            var viewModel = new ExpenseViewModel
            {
                SelectedCategoryId = category,
                expenseCategories = GetExpenseCategory(),
                Expenses = Value != null && Value.Count() > 0 ? Value.FirstOrDefault() : new Expense(),
                ExpensesList = Value,
                CategoryTotals = GetCategoryTotals()
            };
            if (viewModel != null && viewModel.Expenses != null && viewModel.Expenses.Id > 0)
            {
                viewModel.ItemTransection = GetExpenseItemTransections(viewModel.Expenses.Id);
                viewModel.TotalAmount = viewModel.Expenses.Amount;
                viewModel.TotalBalance = viewModel.Expenses.Amount;
            }
            else
            {
                viewModel.ItemTransection = new List<ExpenseItemTransection>();
                viewModel.TotalAmount = 0;
                viewModel.TotalBalance = 0;
            }
            return View(viewModel);
        }

        public List<ExpenseCategoryModel> GetExpenseCategory()
        {
            List<ExpenseCategoryModel> expenseCategories = new List<ExpenseCategoryModel>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var ExpenseQuery = "select * from expensecategory ";
                expenseCategories = conn.QuerySql<ExpenseCategoryModel>(ExpenseQuery).ToList();
            }

            return expenseCategories;
        }

        public List<ExpenseItemMaster> GetExpenseItemMasters()
        {
            List<ExpenseItemMaster> expenseItemMasters = new List<ExpenseItemMaster>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var ExpenseItemQuery = "select * from expenseitemmaster";
                expenseItemMasters = conn.QuerySql<ExpenseItemMaster>(ExpenseItemQuery).ToList();
            }
            return expenseItemMasters;
        }

        public List<ExpenseItemTransection>GetExpenseItemTransections(int expenseId)
        {
            var transections = new List<ExpenseItemTransection>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT id, expenseid, itemid, quantity, price, amount
            FROM expenseitemtransection
            WHERE expenseid = @expenseid
            ORDER BY id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@expenseid", expenseId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transections.Add(new ExpenseItemTransection
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                ExpenseId = reader.GetInt32(reader.GetOrdinal("expenseid")),
                                ItemId = reader.GetInt32(reader.GetOrdinal("itemid")),
                                Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : reader.GetDecimal(reader.GetOrdinal("quantity")),
                                Price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("price")),
                                Amount = reader.IsDBNull(reader.GetOrdinal("amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount"))
                            });
                        }
                    }
                }
            }

            return transections;
        }

        [HttpPost]
        public IActionResult AddExpense(AddExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        INSERT INTO expenses 
                        (category, itemname, itemhsnsac, price, taxtype, taxrate, expensedate, paymenttype, amount, balance) 
                        VALUES 
                        (@category, @itemname, @itemhsnsac, @price, @taxtype, @taxrate, @expensedate, @paymenttype, @amount, @balance)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category", model.Category);
                        command.Parameters.AddWithValue("@itemname", model.ItemName);
                        command.Parameters.AddWithValue("@itemhsnsac", model.ItemHsnSac ?? "");
                        command.Parameters.AddWithValue("@price", model.Price);
                        command.Parameters.AddWithValue("@taxtype", model.TaxType);
                        command.Parameters.AddWithValue("@taxrate", model.TaxRate);
                        command.Parameters.AddWithValue("@expensedate", DateTime.Now);
                        command.Parameters.AddWithValue("@paymenttype", "Cash");
                        command.Parameters.AddWithValue("@amount", model.Price);
                        command.Parameters.AddWithValue("@balance", 0m);

                        command.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Expense added successfully!" });
            }

            return Json(new { success = false, message = "Please fill all required fields." });
        }

        private List<Expense> GetExpensesByCategory(int categoryId)
        {
            var expenses = new List<Expense>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"
            SELECT id, categoryid, category, expenseno, expensedate, isroundoff, 
                   roundoffvalue, total, amount, partyid, paymenttype, 
                   description, imagepath
            FROM expenses
            WHERE categoryid = @categoryid
            ORDER BY expensedate DESC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@categoryid", categoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new Expense
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CategoryId = reader.GetInt32(reader.GetOrdinal("categoryid")),
                                Category = reader.IsDBNull(reader.GetOrdinal("category")) ? "" : reader.GetString(reader.GetOrdinal("category")),
                                ExpenseNo = reader.IsDBNull(reader.GetOrdinal("expenseno")) ? "" : reader.GetString(reader.GetOrdinal("expenseno")),
                                ExpenseDate = reader.GetDateTime(reader.GetOrdinal("expensedate")),
                                Isroundoff = reader.IsDBNull(reader.GetOrdinal("isroundoff")) ? false : reader.GetBoolean(reader.GetOrdinal("isroundoff")),
                                roundoffvalue = reader.IsDBNull(reader.GetOrdinal("roundoffvalue")) ? 0 : reader.GetDecimal(reader.GetOrdinal("roundoffvalue")),
                                Balance = reader.IsDBNull(reader.GetOrdinal("total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total")),
                                Amount = reader.IsDBNull(reader.GetOrdinal("amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount")),
                                PartyId = reader.IsDBNull(reader.GetOrdinal("partyid")) ? 0 : reader.GetInt32(reader.GetOrdinal("partyid")),
                                PaymentType = reader.IsDBNull(reader.GetOrdinal("paymenttype")) ? "" : reader.GetString(reader.GetOrdinal("paymenttype")),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString(reader.GetOrdinal("description")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("imagepath")) ? "" : reader.GetString(reader.GetOrdinal("imagepath"))
                            });
                        }
                    }
                }
            }

            return expenses;
        }



        private List<Expense> GetCategoryTotals()
        {
            var totals = new List<Expense>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"
                                SELECT categoryid, COUNT(*)::int AS count
                                FROM expenses
                                GROUP BY categoryid
                                ORDER BY categoryid";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totals.Add(new Expense
                            {
                                CategoryId = reader.GetInt32(reader.GetOrdinal("categoryid")),
                                count = reader.GetInt32(reader.GetOrdinal("count"))
                            });
                        }
                    }
                }
            }

            return totals;
        }
        public async Task<IActionResult> Create()
        {

            await Task.Delay(1);
            var viewModel = new ExpenseViewModel
            {
                expenseCategories = GetExpenseCategory(),
                ExpenseDropDownItem = GetExpenseItemMasters(),
                Expenses = new Expense(),
                ItemTransection = new List<ExpenseItemTransection> { new ExpenseItemTransection(), new ExpenseItemTransection() }


            };
            //ViewBag.PartyList = await partyController.GetPartyDropDownAsync();
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel viewModel, IFormFile? imageFile)
        {
            try
            {
                if (viewModel != null && viewModel.Expenses != null && viewModel.Expenses.Amount > 0)
                {
                    using (var connection = new Npgsql.NpgsqlConnection(_connectionString))
                    {
                        connection.Open();
                        string query = @"
                                    INSERT INTO public.expenses
                                    (
                                        categoryid, category, expenseno, expensedate, isroundoff, roundoffvalue,
                                        total, amount, partyid, paymenttype, description, imagepath
                                    )
                                    VALUES
                                    (
                                        @categoryid, @category, @expenseno, @expensedate, @isroundoff, @roundoffvalue,
                                        @total, @amount, @partyid, @paymenttype, @description, @imagepath
                                    )  returning id "
                                        ;

                        var Expenseid = connection.ExecuteScalarSql<Int32>(query, new
                        {
                            categoryid = viewModel.Expenses.CategoryId > 0 ? viewModel.Expenses.CategoryId : 0,
                            category = viewModel.Expenses.Category ?? "",
                            expenseno = viewModel.Expenses.ExpenseNo ?? "",
                            expensedate = (viewModel.Expenses.ExpenseDate).ToUniversalTime(),
                            isroundoff = viewModel.Expenses.Isroundoff,
                            roundoffvalue = viewModel.Expenses.roundoffvalue > 0 ? viewModel.Expenses.roundoffvalue : 0,
                            total = viewModel.Expenses.Amount > 0? viewModel.Expenses.Amount:0,
                            amount = viewModel.Expenses.Amount > 0 ? viewModel.Expenses.Amount : 0,
                            partyid = viewModel.Expenses.PartyId,
                            paymenttype = viewModel.Expenses.PaymentType ?? "",
                            description = viewModel.Expenses.Description ?? "",
                            imagepath = viewModel.Expenses.ImageUrl ?? ""
                        });

                        if (viewModel != null && viewModel.ItemTransection != null && viewModel.ItemTransection.Count() > 0)
                        {
                            foreach (var item in viewModel.ItemTransection)
                            {
                                string Transquery = @"
                                           INSERT INTO public.expenseitemtransection
                                           (
                                               expenseid, itemid, quantity, price, amount
                                           )
                                           VALUES
                                           (
                                               @expenseid, @itemid, @quantity, @price, @amount
                                           )";

                                connection.ExecuteSql(Transquery, new
                                {
                                    expenseid = Expenseid,
                                    itemid = item.ItemId,
                                    quantity = item.Quantity,
                                    price = item.Price,
                                    amount = item.Amount
                                });
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating bill: {ex.Message}";
            }

            return View(viewModel);
        }
    }
}


