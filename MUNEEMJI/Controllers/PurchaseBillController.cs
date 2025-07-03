using Dapper;
using Insight.Database;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class PurchaseBillController: Controller
    {
        private readonly IPurchaseBillService _billService;
        private readonly IWebHostEnvironment _environment;
        private readonly IBillItemService _IBillItemService;
        string _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

        public PurchaseBillController(IPurchaseBillService billService, IWebHostEnvironment environment, IBillItemService iBillItemService)
        {
            _billService = billService;
            _environment = environment;
            _IBillItemService = iBillItemService;
        }

        // GET: Bill
        public async Task<IActionResult> Index()
        {
            try
            {
                DateTime? startDate = DateTime.UtcNow;
                DateTime? endDate = DateTime.UtcNow;
                string firmFilter = "ALL FIRMS";
                string vendorFilter = null;
                using var connection = new NpgsqlConnection(_connectionString);


                string query = @"
                SELECT 
                    td.id AS ""Id"",
                    td.bill_number AS ""BillNumber"",
                    td.bill_date AS ""BillDate"",
                    td.state_of_supply AS ""StateOfSupply"",
                    td.phone_no AS ""PhoneNo"",
                    td.po_no AS ""PONo"",
                    td.po_date AS ""PODate"",
                    td.eway_bill_no AS ""EWayBillNo"",
                    td.transport_name AS ""TransportName"",
                    td.delivery_location AS ""DeliveryLocation"",
                    td.vehicle_number AS ""VehicleNumber"",
                    td.delivery_date AS ""DeliveryDate"",
                    td.payment_type AS ""PaymentType"",
                    td.description AS ""Description"",
                    td.image_path AS ""ImagePath"",
                    td.round_off AS ""RoundOff"",
                    td.total AS ""Total"",
                    td.paidreciveamount AS ""paidReciveamount"",
                    td.partyid AS ""PartyId"",
                    pt.party_name as PartyName
                FROM public.tradedocuments as td left join parties as pt on td.partyid = pt.id;
            ";

                var PurchaseBill =  connection.QuerySql<PurchaseBill>(query).ToList();









                // Set default date range if not provided
                //if (!startDate.HasValue || !endDate.HasValue)
                //{
                //    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //    endDate = startDate.Value.AddMonths(1).AddDays(-1);
                //}
                //var query = @"
                //                SELECT 
                //                    pb.""id"",
                //                    pb.""bill_number"",
                //                    pb.""bill_date"",
                //                    pb.""payment_type"",
                //                    pb.""total"",
                //                    pb.""created_date"",
                //                    COALESCE(SUM(CASE WHEN pb.""payment_type"" = 'Cash' THEN pb.""total"" ELSE 0 END), 0) AS ""PaidAmount"",
                //                    COALESCE(SUM(CASE WHEN pb.""payment_type"" != 'Cash' THEN pb.""total"" ELSE 0 END), 0) AS ""UnpaidAmount""
                //                FROM ""purchasebills"" pb
                //                WHERE pb.""bill_date"" >= @StartDate::date AND pb.""bill_date"" <= @EndDate::date";

                //                            if (firmFilter != "ALL FIRMS")
                //                            {
                //                                query += " AND pb.\"state_of_supply\" = @FirmFilter";
                //                            }

                //                            query += @"
                //                GROUP BY 
                //                    pb.""id"", 
                //                    pb.""bill_number"", 
                //                    pb.""bill_date"", 
                //                    pb.""payment_type"", 
                //                    pb.""total"", 
                //                    pb.""created_date""
                //                ORDER BY pb.""bill_date"" DESC";


                //var bills = await connection.QueryAsync<dynamic>(query, new
                //{
                //    StartDate = startDate.Value,
                //    EndDate = endDate.Value,
                //    FirmFilter = firmFilter
                //});

                // Calculate summary
                if (PurchaseBill != null && PurchaseBill.Count() > 0)
                {


                    var paidTotal = PurchaseBill.Sum(b => b.paidReciveamount);
                    //var unpaidTotal = bills.Where(b => b.PaymentType != "Cash").Sum(b => (decimal)b.Total);
                    var unpaidTotal = PurchaseBill.Sum(x => x.Total) - paidTotal;

                    var grandTotal = paidTotal + unpaidTotal;

                    ViewBag.PaidTotal = paidTotal;
                    ViewBag.UnpaidTotal = unpaidTotal;
                    ViewBag.GrandTotal = grandTotal;
                    ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.FirmFilter = firmFilter;
                    ViewBag.VendorFilter = vendorFilter;
                }
                else
                {
                    PurchaseBill = new List<PurchaseBill>();
                }

                return View(PurchaseBill);
            }
            catch (Exception ex)
            {
                // Log error
                ViewBag.Error = "An error occurred while loading purchase bills.";
                return View(new List<dynamic>());
            }
        }

        

        // GET: Bill/Create
        public async Task<IActionResult> Create()
        {
            PartyController partyController = new PartyController();
            await Task.Delay(1);
            var viewModel = new PurchaseBillViewModel
            {
                Bill = new PurchaseBill
                {
                    BillNumber = _billService.GenerateBillNumber(),

                    BillDate = DateTime.Now,
                    BillItems = new List<PurchaseBillItem>
                    {
                        new PurchaseBillItem(),
                        new PurchaseBillItem()
                    }
                },
                DropDownItem = await _IBillItemService.GetItems()
            };
            ViewBag.PartyList = await partyController.GetPartyDropDownAsync();
            return View(viewModel);
        }

        // POST: Bill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseBillViewModel viewModel, IFormFile? imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        viewModel.Bill.ImagePath = "/uploads/" + uniqueFileName;
                    }

                    // Calculate totals
                    CalculateBillTotals(viewModel.Bill);

                    var billId = await _billService.CreateBillAsync(viewModel.Bill);
                    TempData["SuccessMessage"] = "Bill created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating bill: {ex.Message}";
            }

            return View(viewModel);
        }

        // GET: Bill/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bill/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _billService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            var viewModel = new PurchaseBillViewModel
            {
                Bill = bill
            };

            return View(viewModel);
        }

        // POST: Bill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseBillViewModel viewModel, IFormFile? imageFile)
        {
            if (id != viewModel.Bill.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        viewModel.Bill.ImagePath = "/uploads/" + uniqueFileName;
                    }

                    // Calculate totals
                    CalculateBillTotals(viewModel.Bill);

                    await _billService.UpdateBillAsync(viewModel.Bill);
                    TempData["SuccessMessage"] = "Bill updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = viewModel.Bill.Id });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating bill: {ex.Message}";
            }

            return View(viewModel);
        }

        // GET: Bill/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _billService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _billService.DeleteBillAsync(id);
                TempData["SuccessMessage"] = "Bill deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting bill: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult AddBillItem(PurchaseBillViewModel viewModel)
        {
            viewModel.Bill.BillItems ??= new List<PurchaseBillItem>();
            viewModel.Bill.BillItems.Add(new PurchaseBillItem());
            return View("Create", viewModel);
        }

        [HttpPost]
        public IActionResult RemoveBillItem(PurchaseBillViewModel viewModel, int index)
        {
            if (viewModel.Bill.BillItems != null && index >= 0 && index < viewModel.Bill.BillItems.Count)
            {
                viewModel.Bill.BillItems.RemoveAt(index);
            }
            return View("Create", viewModel);
        }

        [HttpPost]
        public JsonResult CalculateItemAmount([FromBody] BillItemCalculationRequest request)
        {
            try
            {
                var quantity = request.Quantity;
                var pricePerUnit = request.PricePerUnit;
                var discountPercentage = request.DiscountPercentage;
                var taxRate = ExtractTaxRate(request.Tax);

                var subtotal = quantity * pricePerUnit;
                var discountAmount = subtotal * (discountPercentage / 100);
                var afterDiscount = subtotal - discountAmount;
                var taxAmount = afterDiscount * (taxRate / 100);
                var finalAmount = afterDiscount + taxAmount;

                return Json(new
                {
                    success = true,
                    discountAmount = Math.Round(discountAmount, 2),
                    taxAmount = Math.Round(taxAmount, 2),
                    amount = Math.Round(finalAmount, 2)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private void CalculateBillTotals(PurchaseBill bill)
        {
            decimal total = 0;

            foreach (var item in bill.BillItems)
            {
                var subtotal = item.Quantity * item.PricePerUnit;
                item.DiscountAmount = subtotal * (item.DiscountPercentage / 100);
                var afterDiscount = subtotal - item.DiscountAmount;

                var taxRate = ExtractTaxRate(item.Tax);
                item.TaxAmount = afterDiscount * (taxRate / 100);
                item.Amount = afterDiscount + item.TaxAmount;

                total += item.Amount;
            }

            if (bill.RoundOff)
            {
                bill.Total = Math.Round(total);
            }
            else
            {
                bill.Total = Math.Round(total, 2);
            }
        }

        private decimal ExtractTaxRate(string tax)
        {
            if (string.IsNullOrEmpty(tax) || tax == "Select")
                return 0;

            var taxString = tax.Replace("%", "");
            if (decimal.TryParse(taxString, out decimal rate))
                return rate;

            return 0;
        }
    }

    public class BillItemCalculationRequest
    {
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Tax { get; set; } = "Select";
    }
}

