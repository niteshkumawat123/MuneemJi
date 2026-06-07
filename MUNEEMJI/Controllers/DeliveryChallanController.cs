using Insight.Database;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.PdfServices;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class DeliveryChallanController:Controller
    {
        private readonly IDeliveryChallanService _billService;
        private readonly IWebHostEnvironment _environment;
        private readonly IBillItemService _IBillItemService;
        private readonly ICompanyTenancy _CompayTenancy;
        private readonly IParty partyController;
        private readonly IGstTaxService _gstTaxService;
        private readonly IDeliveryChallanPdf _pdfService;
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;
        public DeliveryChallanController(IDeliveryChallanService billService, IWebHostEnvironment environment, IBillItemService iBillItemService, ICompanyTenancy CompayTenancy, IParty partyController, IGstTaxService gstTaxService, IDeliveryChallanPdf pdfService)
        {
            _billService = billService;
            _environment = environment;
            _IBillItemService = iBillItemService;
            _CompayTenancy = CompayTenancy;
            this.partyController = partyController;
            _gstTaxService = gstTaxService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                DateTime? startDate = DateTime.UtcNow;
                DateTime? endDate = DateTime.UtcNow;
                string firmFilter = "ALL FIRMS";
                string vendorFilter = null;
                using var connection = new NpgsqlConnection(_connectionString);
                var companyId = _CompayTenancy.GetCurrentCompanyId();

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
                    pt.party_name as PartyName,
                    td.orderstatusid,
                    td.orderno,
                    td.orderdate,
                    td.duedate,
                    td.Challandate,
                    td.challanno,
                    td.INVOICENUMBER,
                    td.final_amount as ""finalamount""
                FROM public.tradedocuments as td left join parties as pt on td.partyid = pt.id where td.TradeDocumentTypesid=@TradeDocumentTypesid and  td.companyid = @p_companyid ;
            ";

                List<PurchaseBill> PurchaseList = connection.QuerySql<PurchaseBill>(query, new { TradeDocumentTypesid = (int)TradeDocumentTypes.DeliveryChallan, p_companyid= companyId }).ToList();

                if (PurchaseList != null && PurchaseList.Count() > 0)
                {

                }
                else
                {
                    PurchaseList = new List<PurchaseBill>();
                }

                return View(PurchaseList);
            }
            catch (Exception ex)
            {
                // Log error
                ViewBag.Error = "An error occurred while loading purchase bills.";
                return View(new List<PurchaseBill>());
            }

        }
        public async Task<IActionResult> Create()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var viewModel = new PurchaseBillViewModel();
                var transactionSettingsController = new TransactionSettingsController();
                var CategoryController = new CategoryController();
                int firmid = 1;
                await Task.Delay(1);
                viewModel = new PurchaseBillViewModel
                {
                    Bill = new PurchaseBill
                    {
                        BillNumber = _billService.GenerateBillNumber(),
                        BillDate = DateTime.Now,
                        BillItems = new List<PurchaseBillItem>
                    {
                    new PurchaseBillItem(),
                    new PurchaseBillItem()
                    },
                        transactionSettings = transactionSettingsController.GetTransactionByFirmId(firmid),
                        itemSettings = transactionSettingsController.GetItemSettings()
                    },
                    ViewTypeId = (int)ViewTypeEnum.Create,
                    DropDownItem = await _IBillItemService.GetItems(companyId),
                    DropDownCategory = CategoryController.GetCategoriesDropdown()
                };
                var PartyList = await partyController.GetPartyDropDownAsync(companyId);
                ViewBag.PartyList = PartyList;
                ViewBag.GstRateOptions = await _gstTaxService.GetGstRateOptionsAsync(companyId, viewModel.Bill?.StateOfSupply);
                ViewBag.IsSameState = await _gstTaxService.IsSameStateAsync(companyId, viewModel.Bill?.StateOfSupply);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                var errorQuery = @"
                             INSERT INTO error_logs (error_message, stack_trace, created_at)
                             VALUES (@message, @stack, NOW());";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(errorQuery, connection))
                    {
                        command.Parameters.AddWithValue("@message", ex.Message);
                        command.Parameters.AddWithValue("@stack", (object)ex.StackTrace ?? DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }



                return View(new PurchaseBillViewModel());
            }
        }

        // POST: Bill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseBillViewModel viewModel)
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {


                if (viewModel.Bill.imageFile != null && viewModel.Bill.imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "transaction");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Bill.imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Bill.imageFile.CopyToAsync(fileStream);
                    }

                    viewModel.Bill.ImagePath = "/Web/uploads/transaction/" + uniqueFileName;
                }
                if (viewModel.Bill.DocumentFile != null && viewModel.Bill.DocumentFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "transaction");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Bill.DocumentFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Bill.DocumentFile.CopyToAsync(fileStream);
                    }

                    viewModel.Bill.DocumentPath = "/Web/uploads/transaction/" + uniqueFileName;
                }

                // Calculate totals
                CalculateBillTotals(viewModel.Bill);

                var billId = await _billService.CreateBillAsync(viewModel.Bill, companyId);

                return Json(new { success = true, message = "Data saved successfully!" });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });

            }

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
        #region  Add Edit Delete
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
        public async Task<IActionResult> GetById(int id = 0, int typeid = 0)
        {
            var viewModel = new PurchaseBillViewModel();
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            var transactionSettingsController = new TransactionSettingsController();
            var CategoryController = new CategoryController();

            await Task.Delay(1);
            if (id > 0)
            {
                var bill = await _billService.GetBillByIdAsync(id);
                bill.transactionSettings = transactionSettingsController.GetTransactionByFirmId(1);
                bill.itemSettings = transactionSettingsController.GetItemSettings();
                viewModel = new PurchaseBillViewModel
                {
                    Bill = bill,

                    ViewTypeId = typeid,
                    DropDownItem = await _IBillItemService.GetItems(companyId),
                    DropDownCategory = CategoryController.GetCategoriesDropdown()

                };
            }
            else
            {
                viewModel = new PurchaseBillViewModel
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
                    ViewTypeId = typeid,
                    DropDownItem = await _IBillItemService.GetItems(companyId)
                };
            }
            ViewBag.PartyList = await partyController.GetPartyDropDownAsync(companyId);
            ViewBag.GstRateOptions = await _gstTaxService.GetGstRateOptionsAsync(companyId, viewModel.Bill?.StateOfSupply);
            ViewBag.IsSameState = await _gstTaxService.IsSameStateAsync(companyId, viewModel.Bill?.StateOfSupply);
            return View("Create", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEntries(PurchaseBillViewModel viewModel)
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();

            try
            {
                PurchaseBill existingProImage = null;
                if (viewModel.Bill.Id > 0)
                {
                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        conn.Open();
                        string selectSql = "SELECT image_path,documentpath FROM TradeDocuments WHERE id = @Id";
                        using (var cmd = new NpgsqlCommand(selectSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", viewModel.Bill.Id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    existingProImage = new PurchaseBill
                                    {
                                        ImagePath = reader["image_path"] as string,
                                        DocumentPath = reader["documentpath"] as string


                                    };
                                }
                            }
                        }
                    }
                }

                if (viewModel.Bill.IsDeleteImage)
                {
                    viewModel.Bill.imageFile = null;
                }
                else if (viewModel.Bill.imageFile != null && viewModel.Bill.imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "transaction");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Bill.imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Bill.imageFile.CopyToAsync(fileStream);
                    }

                    viewModel.Bill.ImagePath = "/Web/uploads/transaction/" + uniqueFileName;
                }
                else
                {
                    viewModel.Bill.ImagePath = existingProImage.ImagePath;

                }
                if (viewModel.Bill.DocumentFile != null && viewModel.Bill.DocumentFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "transaction");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Bill.DocumentFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Bill.DocumentFile.CopyToAsync(fileStream);
                    }

                    viewModel.Bill.DocumentPath = "/Web/uploads/transaction/" + uniqueFileName;
                }
                else
                {
                    viewModel.Bill.DocumentPath = existingProImage.DocumentPath;
                }

                // Calculate totals
                CalculateBillTotals(viewModel.Bill);

                var billId = await _billService.UpdateEntries(viewModel.Bill);

                return Json(new { success = true, message = "Data Update successfully!" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });

            }

        }


        #endregion

        [HttpGet]
        public async Task<IActionResult> GetGstRates(string? stateOfSupply = null)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var gstOptions = await _gstTaxService.GetGstRateOptionsAsync(companyId, stateOfSupply);
                bool isSameState = await _gstTaxService.IsSameStateAsync(companyId, stateOfSupply);
                return Json(new { success = true, isSameState, taxType = isSameState ? "CGST_SGST" : "IGST", options = gstOptions.Select(o => new { value = o.TaxPercentage.ToString("0.##"), text = o.DisplayText, taxType = o.TaxType, cgstRate = o.CgstRate, sgstRate = o.SgstRate, igstRate = o.IgstRate, isSameState = o.IsSameState }) });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var relativePath = await _pdfService.GetDeliveryChallanPdfById(id, _environment);
            if (string.IsNullOrEmpty(relativePath))
                return NotFound();

            var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, "application/pdf", Path.GetFileName(fullPath));
        }
    }
}
