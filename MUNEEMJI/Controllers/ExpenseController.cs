using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
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
        private readonly ICompanyTenancy _CompanyTenancy;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExpenseController(IConfiguration configuration, ICompanyTenancy tenancy, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;
            _CompanyTenancy = tenancy;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int category = 1)
        {
            var CompanyId = _CompanyTenancy.GetCurrentCompanyId();
            var Value = GetExpensesByCategory(category, CompanyId);
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

        public List<ExpenseItemTransection> GetExpenseItemTransections(int expenseId)
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

        [HttpGet]
        public IActionResult GetExpenseById(int id)
        {
            try
            {
                var CompanyId = _CompanyTenancy.GetCurrentCompanyId();
                Expense expense = null;

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT id, categoryid, category, expenseno, expensedate, isroundoff, 
                               roundoffvalue, total, amount, partyid, paymenttype, 
                               description, imagepath
                        FROM expenses
                        WHERE id = @id AND companyid = @p_companyid";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@p_companyid", CompanyId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                expense = new Expense
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
                                };
                            }
                        }
                    }
                }

                if (expense == null)
                {
                    return Json(new { success = false, message = "Expense not found." });
                }

                var items = GetExpenseItemTransections(id);

                return Json(new
                {
                    success = true,
                    expense = new
                    {
                        expense.Id,
                        expense.CategoryId,
                        expense.Category,
                        expense.ExpenseNo,
                        ExpenseDate = expense.ExpenseDate.ToString("yyyy-MM-dd"),
                        expense.Isroundoff,
                        expense.roundoffvalue,
                        expense.Amount,
                        expense.Balance,
                        expense.PartyId,
                        expense.PaymentType,
                        expense.Description,
                        expense.ImageUrl
                    },
                    items = items.Select(i => new
                    {
                        i.Id,
                        i.ExpenseId,
                        i.ItemId,
                        i.Quantity,
                        i.Price,
                        i.Amount
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateExpense(ExpenseViewModel viewModel, IFormFile? imageFile)
        {
            try
            {
                var CompanyId = _CompanyTenancy.GetCurrentCompanyId();

                if (viewModel == null || viewModel.Expenses == null || viewModel.Expenses.Id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid expense data.";
                    return RedirectToAction("Index", new { category = viewModel?.Expenses?.CategoryId ?? 1 });
                }

                string imagePath = viewModel.Expenses.ImageUrl ?? "";

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "expense");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                    imagePath = "/uploads/expense/" + uniqueFileName;
                }

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE public.expenses
                        SET categoryid = @categoryid,
                            category = @category,
                            expenseno = @expenseno,
                            expensedate = @expensedate,
                            isroundoff = @isroundoff,
                            roundoffvalue = @roundoffvalue,
                            total = @total,
                            amount = @amount,
                            partyid = @partyid,
                            paymenttype = @paymenttype,
                            description = @description,
                            imagepath = @imagepath
                        WHERE id = @id AND companyid = @p_CompanyId";

                    connection.ExecuteSql(query, new
                    {
                        id = viewModel.Expenses.Id,
                        categoryid = viewModel.Expenses.CategoryId > 0 ? viewModel.Expenses.CategoryId : 0,
                        category = viewModel.Expenses.Category ?? "",
                        expenseno = viewModel.Expenses.ExpenseNo ?? "",
                        expensedate = viewModel.Expenses.ExpenseDate.ToUniversalTime(),
                        isroundoff = viewModel.Expenses.Isroundoff,
                        roundoffvalue = viewModel.Expenses.roundoffvalue > 0 ? viewModel.Expenses.roundoffvalue : 0,
                        total = viewModel.Expenses.Amount > 0 ? viewModel.Expenses.Amount : 0,
                        amount = viewModel.Expenses.Amount > 0 ? viewModel.Expenses.Amount : 0,
                        partyid = viewModel.Expenses.PartyId,
                        paymenttype = viewModel.Expenses.PaymentType ?? "",
                        description = viewModel.Expenses.Description ?? "",
                        imagepath = imagePath,
                        p_CompanyId = CompanyId
                    });

                    // Delete existing item transactions and re-insert
                    string deleteQuery = "DELETE FROM public.expenseitemtransection WHERE expenseid = @expenseid";
                    connection.ExecuteSql(deleteQuery, new { expenseid = viewModel.Expenses.Id });

                    if (viewModel.ItemTransection != null && viewModel.ItemTransection.Count > 0)
                    {
                        foreach (var item in viewModel.ItemTransection)
                        {
                            if (item.ItemId <= 0 && item.Amount <= 0) continue;

                            string transQuery = @"
                                INSERT INTO public.expenseitemtransection
                                (expenseid, itemid, quantity, price, amount)
                                VALUES
                                (@expenseid, @itemid, @quantity, @price, @amount)";

                            connection.ExecuteSql(transQuery, new
                            {
                                expenseid = viewModel.Expenses.Id,
                                itemid = item.ItemId,
                                quantity = item.Quantity,
                                price = item.Price,
                                amount = item.Amount
                            });
                        }
                    }
                }

                TempData["SuccessMessage"] = "Expense updated successfully!";
                return RedirectToAction("Index", new { category = viewModel.Expenses.CategoryId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating expense: {ex.Message}";
                return RedirectToAction("Index", new { category = viewModel?.Expenses?.CategoryId ?? 1 });
            }
        }

        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            try
            {
                var CompanyId = _CompanyTenancy.GetCurrentCompanyId();
                int categoryId = 1;

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Get the category before deleting
                    using (var cmd = new NpgsqlCommand("SELECT categoryid FROM expenses WHERE id = @id AND companyid = @cid", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@cid", CompanyId);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            categoryId = Convert.ToInt32(result);
                    }

                    // Delete item transactions first
                    using (var cmd = new NpgsqlCommand("DELETE FROM expenseitemtransection WHERE expenseid = @expenseid", connection))
                    {
                        cmd.Parameters.AddWithValue("@expenseid", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete the expense
                    using (var cmd = new NpgsqlCommand("DELETE FROM expenses WHERE id = @id AND companyid = @cid", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@cid", CompanyId);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Expense deleted successfully!", categoryId = categoryId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting expense: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] ExpenseCategoryModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model?.Category))
                {
                    return Json(new { success = false, message = "Category name is required." });
                }

                int newId = 0;
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO expensecategory (category, expensetype)
                        VALUES (@category, @expensetype)
                        RETURNING id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category", model.Category);
                        command.Parameters.AddWithValue("@expensetype", model.ExpenseType ?? "Direct Expense");
                        newId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                return Json(new { success = true, message = "Category added successfully!", id = newId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding category: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if any expenses exist for this category
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM expenses WHERE categoryid = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            return Json(new { success = false, message = "Cannot delete category with existing expenses. Please delete all expenses in this category first." });
                        }
                    }

                    using (var cmd = new NpgsqlCommand("DELETE FROM expensecategory WHERE id = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Category deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting category: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult AddExpenseItem([FromBody] ExpenseItemMaster model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model?.Name))
                {
                    return Json(new { success = false, message = "Item name is required." });
                }

                int newId = 0;
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO expenseitemmaster (name, hsnsaccode, price, taxtype, taxrate, statusid)
                        VALUES (@name, @hsnsaccode, @price, @taxtype, @taxrate, @statusid)
                        RETURNING id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", model.Name);
                        command.Parameters.AddWithValue("@hsnsaccode", model.HsnSacCode ?? "");
                        command.Parameters.AddWithValue("@price", model.Price);
                        command.Parameters.AddWithValue("@taxtype", model.TaxType ?? "Tax Excluded");
                        command.Parameters.AddWithValue("@taxrate", model.TaxRate ?? "IGST@0.25%");
                        command.Parameters.AddWithValue("@statusid", 1);
                        newId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                return Json(new { success = true, message = "Item added successfully!", id = newId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding item: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult DeleteExpenseItem(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if item is used in any transactions
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM expenseitemtransection WHERE itemid = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            return Json(new { success = false, message = "Cannot delete item that is used in expense transactions." });
                        }
                    }

                    using (var cmd = new NpgsqlCommand("DELETE FROM expenseitemmaster WHERE id = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Item deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting item: {ex.Message}" });
            }
        }

        private List<Expense> GetExpensesByCategory(int categoryId, int CompanyId)
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
            WHERE categoryid = @categoryid and CompanyId = @p_companyid
            ORDER BY expensedate DESC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@categoryid", categoryId);
                    command.Parameters.AddWithValue("@p_companyid", CompanyId);

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
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel viewModel, IFormFile? imageFile)
        {
            try
            {
                var CompanyId = _CompanyTenancy.GetCurrentCompanyId();

                if (viewModel != null && viewModel.Expenses != null && viewModel.Expenses.Amount > 0)
                {
                    string imagePath = "";

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "expense");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        imagePath = "/uploads/expense/" + uniqueFileName;
                    }

                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();
                        string query = @"
                                    INSERT INTO public.expenses
                                    (
                                        categoryid, category, expenseno, expensedate, isroundoff, roundoffvalue,
                                        total, amount, partyid, paymenttype, description, imagepath, CompanyId
                                    )
                                    VALUES
                                    (
                                        @categoryid, @category, @expenseno, @expensedate, @isroundoff, @roundoffvalue,
                                        @total, @amount, @partyid, @paymenttype, @description, @imagepath, @p_CompanyId
                                    ) RETURNING id";

                        var Expenseid = connection.ExecuteScalarSql<Int32>(query, new
                        {
                            categoryid = viewModel.Expenses.CategoryId > 0 ? viewModel.Expenses.CategoryId : 0,
                            category = viewModel.Expenses.Category ?? "",
                            expenseno = viewModel.Expenses.ExpenseNo ?? "",
                            expensedate = (viewModel.Expenses.ExpenseDate).ToUniversalTime(),
                            isroundoff = viewModel.Expenses.Isroundoff,
                            roundoffvalue = viewModel.Expenses.roundoffvalue > 0 ? viewModel.Expenses.roundoffvalue : 0,
                            total = viewModel.Expenses.Amount > 0 ? viewModel.Expenses.Amount : 0,
                            amount = viewModel.Expenses.Amount > 0 ? viewModel.Expenses.Amount : 0,
                            partyid = viewModel.Expenses.PartyId,
                            paymenttype = viewModel.Expenses.PaymentType ?? "",
                            description = viewModel.Expenses.Description ?? "",
                            imagepath = imagePath,
                            p_CompanyId = CompanyId
                        });

                        if (viewModel.ItemTransection != null && viewModel.ItemTransection.Count > 0)
                        {
                            foreach (var item in viewModel.ItemTransection)
                            {
                                if (item.ItemId <= 0 && item.Amount <= 0) continue;

                                string Transquery = @"
                                           INSERT INTO public.expenseitemtransection
                                           (expenseid, itemid, quantity, price, amount)
                                           VALUES
                                           (@expenseid, @itemid, @quantity, @price, @amount)";

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

                    TempData["SuccessMessage"] = "Expense created successfully!";
                    return RedirectToAction("Index", new { category = viewModel.Expenses.CategoryId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating expense: {ex.Message}";
            }

            // Re-populate dropdowns on validation failure
            viewModel.expenseCategories = GetExpenseCategory();
            viewModel.ExpenseDropDownItem = GetExpenseItemMasters();
            if (viewModel.ItemTransection == null || viewModel.ItemTransection.Count == 0)
            {
                viewModel.ItemTransection = new List<ExpenseItemTransection> { new ExpenseItemTransection(), new ExpenseItemTransection() };
            }

            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var CompanyId = _CompanyTenancy.GetCurrentCompanyId();
            Expense expense = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"
                    SELECT id, categoryid, category, expenseno, expensedate, isroundoff, 
                           roundoffvalue, total, amount, partyid, paymenttype, 
                           description, imagepath
                    FROM expenses
                    WHERE id = @id AND companyid = @p_companyid";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@p_companyid", CompanyId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            expense = new Expense
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
                            };
                        }
                    }
                }
            }

            if (expense == null)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction("Index");
            }

            var items = GetExpenseItemTransections(id);
            if (items.Count == 0)
            {
                items = new List<ExpenseItemTransection> { new ExpenseItemTransection(), new ExpenseItemTransection() };
            }

            var viewModel = new ExpenseViewModel
            {
                Expenses = expense,
                expenseCategories = GetExpenseCategory(),
                ExpenseDropDownItem = GetExpenseItemMasters(),
                ItemTransection = items
            };

            return View("Create", viewModel);
        }
    }
}
