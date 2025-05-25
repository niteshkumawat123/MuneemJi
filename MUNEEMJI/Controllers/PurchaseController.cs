using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class PurchaseController:Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;

        public PurchaseController(IConfiguration config, IWebHostEnvironment env)
        {
            _connectionString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";
            _env = env;
        }

        // GET: Display purchase entry form
        [HttpGet]
        public IActionResult Purchase()
        {
            // Initialize with one empty line item
            var model = new PurchaseHeaderModel
            {
                Items = new List<PurchaseItemModel> { new PurchaseItemModel() }
            };
            return View("Purchase", model);
        }

        // POST: Handle form submission and save to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(PurchaseHeaderModel model)
        {
            // Parse line items from form (dynamic lists via Request.Form)
            var itemNames = Request.Form["ItemName"];
            var qtys = Request.Form["Quantity"];
            var units = Request.Form["Unit"];
            var prices = Request.Form["Price"];
            var discounts = Request.Form["Discount"];
            var taxes = Request.Form["Tax"];

            // Calculate totals for quantity, amount, and tax
            decimal totalQty = 0, totalAmt = 0, totalTax = 0;
            int itemCount = itemNames.Count;
            for (int i = 0; i < itemCount; i++)
            {
                decimal qty = Convert.ToDecimal(qtys[i]);
                decimal price = Convert.ToDecimal(prices[i]);
                decimal discount = Convert.ToDecimal(discounts[i]);
                decimal taxRate = Convert.ToDecimal(taxes[i]);
                decimal amount = (qty * price) - discount;
                totalQty += qty;
                totalAmt += amount;
                totalTax += amount * taxRate / 100;
            }

            // Save uploaded bill file to server (if provided)
            string billPath = null;
            if (model.BillFile != null && model.BillFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var billFileName = Path.GetFileName(model.BillFile.FileName);
                billPath = Path.Combine("uploads", billFileName);
                var fullBillPath = Path.Combine(_env.WebRootPath, billPath);
                using (var stream = System.IO.File.Create(fullBillPath))
                {
                    await model.BillFile.CopyToAsync(stream);
                }
            }

            // Open DB connection and begin transaction
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var tran = await conn.BeginTransactionAsync();
            try
            {
                // Insert into purchase_header and get new ID
                var headerCmd = new NpgsqlCommand(@"
                INSERT INTO purchase_header(payment_type, round_off, total_quantity, total_amount, total_tax, description, bill_file_path)
                VALUES(@paymentType, @roundOff, @totalQty, @totalAmt, @totalTax, @description, @billPath)
                RETURNING id", conn);
                headerCmd.Parameters.AddWithValue("paymentType", model.PaymentType ?? "");
                headerCmd.Parameters.AddWithValue("roundOff", model.RoundOff);
                headerCmd.Parameters.AddWithValue("totalQty", totalQty);
                headerCmd.Parameters.AddWithValue("totalAmt", totalAmt);
                headerCmd.Parameters.AddWithValue("totalTax", totalTax);
                headerCmd.Parameters.AddWithValue("description", model.Description ?? "");
                headerCmd.Parameters.AddWithValue("billPath", (object)billPath ?? DBNull.Value);

                int purchaseId = Convert.ToInt32(await headerCmd.ExecuteScalarAsync());

                // Insert each purchase item
                for (int i = 0; i < itemCount; i++)
                {
                    var itemCmd = new NpgsqlCommand(@"
                    INSERT INTO purchase_item(purchase_id, item_name, quantity, unit, price, discount, tax, amount)
                    VALUES(@purchaseId, @itemName, @qty, @unit, @price, @discount, @tax, @amount)", conn);
                    itemCmd.Parameters.AddWithValue("purchaseId", purchaseId);
                    itemCmd.Parameters.AddWithValue("itemName", itemNames[i]);
                    itemCmd.Parameters.AddWithValue("qty", Convert.ToDecimal(qtys[i]));
                    itemCmd.Parameters.AddWithValue("unit", units[i]);
                    itemCmd.Parameters.AddWithValue("price", Convert.ToDecimal(prices[i]));
                    itemCmd.Parameters.AddWithValue("discount", Convert.ToDecimal(discounts[i]));
                    itemCmd.Parameters.AddWithValue("tax", Convert.ToDecimal(taxes[i]));
                    decimal lineAmount = (Convert.ToDecimal(qtys[i]) * Convert.ToDecimal(prices[i])) - Convert.ToDecimal(discounts[i]);
                    itemCmd.Parameters.AddWithValue("amount", lineAmount);
                    await itemCmd.ExecuteNonQueryAsync();
                }

                // Save image files and insert references
                if (model.ImageFiles != null)
                {
                    foreach (var img in model.ImageFiles)
                    {
                        if (img != null && img.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsFolder);
                            var imgFileName = Path.GetFileName(img.FileName);
                            var imgPath = Path.Combine("uploads", imgFileName);
                            var fullImgPath = Path.Combine(_env.WebRootPath, imgPath);
                            using (var stream = System.IO.File.Create(fullImgPath))
                            {
                                await img.CopyToAsync(stream);
                            }
                            var fileCmd = new NpgsqlCommand(@"
                            INSERT INTO purchase_file(purchase_id, file_type, file_path)
                            VALUES(@purchaseId, @fileType, @filePath)", conn);
                            fileCmd.Parameters.AddWithValue("purchaseId", purchaseId);
                            fileCmd.Parameters.AddWithValue("fileType", "Image");
                            fileCmd.Parameters.AddWithValue("filePath", imgPath);
                            await fileCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                // Save document files and insert references
                if (model.DocumentFiles != null)
                {
                    foreach (var doc in model.DocumentFiles)
                    {
                        if (doc != null && doc.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsFolder);
                            var docFileName = Path.GetFileName(doc.FileName);
                            var docPath = Path.Combine("uploads", docFileName);
                            var fullDocPath = Path.Combine(_env.WebRootPath, docPath);
                            using (var stream = System.IO.File.Create(fullDocPath))
                            {
                                await doc.CopyToAsync(stream);
                            }
                            var fileCmd = new NpgsqlCommand(@"
                            INSERT INTO purchase_file(purchase_id, file_type, file_path)
                            VALUES(@purchaseId, @fileType, @filePath)", conn);
                            fileCmd.Parameters.AddWithValue("purchaseId", purchaseId);
                            fileCmd.Parameters.AddWithValue("fileType", "Document");
                            fileCmd.Parameters.AddWithValue("filePath", docPath);
                            await fileCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await tran.CommitAsync();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                // Handle error (log, show message, etc.)
                throw;
            }

            // After successful save, redirect back (or to a confirmation page)
            return RedirectToAction("Purchase");
        }
    }
}
