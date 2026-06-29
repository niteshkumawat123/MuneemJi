using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.PdfServices;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using NuGet.Protocol.Plugins;
using System.Reflection;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ISalesBillService _billService;
        private readonly IWebHostEnvironment _environment;
        private readonly IBillItemService _IBillItemService;
        private readonly TransactionSettingsController settingsController;
        private readonly ICompanyTenancy _CompayTenancy;
        private readonly IParty partyController;
        private readonly ISalesInvoicesPdf _salesInvoicesPdf;
        private readonly IGstTaxService _gstTaxService;
        private readonly IErrorLogService _errorLogService;
        private readonly IStockAndBalanceService _stockAndBalanceService;

        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;
        public SalesController(ISalesBillService billService, IWebHostEnvironment environment, IBillItemService iBillItemService,
            ICompanyTenancy companyTenancy, IParty _partyController, ISalesInvoicesPdf salesInvoicesPdf, IGstTaxService gstTaxService, IErrorLogService errorLogService,
            IStockAndBalanceService stockAndBalanceService)
        {
            _billService = billService;
            _environment = environment;
            _IBillItemService = iBillItemService;
            _CompayTenancy = companyTenancy;
            partyController = _partyController;
            _salesInvoicesPdf = salesInvoicesPdf;
            _gstTaxService = gstTaxService;
            _errorLogService = errorLogService;
            _stockAndBalanceService = stockAndBalanceService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
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
                pt.party_name as PartyName,
                td.final_amount as ""FinalAmount"",
                td.invoicenumber as ""InvoiceNumber"",
                td.IsCredit as ""IsCredit""
            FROM public.tradedocuments as td 
            LEFT JOIN parties as pt ON td.partyid = pt.id  
            WHERE td.TradeDocumentTypesid = @TradeDocumentTypesid 
            AND td.companyid = @p_companyid;
        ";

                var PurchaseBill = connection.QuerySql<PurchaseBill>(query,
                    new
                    {
                        TradeDocumentTypesid = (int)TradeDocumentTypes.SalesChallan,
                        p_companyid = companyId
                    }).ToList();

                if (PurchaseBill != null && PurchaseBill.Count > 0)
                {
                    // Handle potential null values with null-coalescing operator
                    var paidTotal = PurchaseBill.Any(x => x.paidReciveamount > 0) ? PurchaseBill.Sum(b => b.paidReciveamount) : decimal.Zero;
                    var unpaidTotal = PurchaseBill.Any(y => y.Total > 0) ? (PurchaseBill.Sum(x => (x.Total)) - paidTotal) : decimal.Zero;
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
                    ViewBag.PaidTotal = 0m;
                    ViewBag.UnpaidTotal = 0m;
                    ViewBag.GrandTotal = 0m;
                    ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.FirmFilter = firmFilter;
                    ViewBag.VendorFilter = vendorFilter;

                    PurchaseBill = new List<PurchaseBill>();
                }

                return View(PurchaseBill);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Sales Index Error: {ex.Message}", ex.StackTrace);

                ViewBag.PaidTotal = 0m;
                ViewBag.UnpaidTotal = 0m;
                ViewBag.GrandTotal = 0m;
                ViewBag.StartDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                ViewBag.EndDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                ViewBag.FirmFilter = "ALL FIRMS";
                ViewBag.VendorFilter = null;
                ViewBag.Error = "An error occurred while loading purchase bills.";

                return View(new List<PurchaseBill>());
            }
        }



        // GET: Bill/Create
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
                await _errorLogService.LogErrorAsync($"Sales Create GET Error: {ex.Message}", ex.StackTrace);
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
                // Validate stock availability before creating the sale
                var stockError = await _stockAndBalanceService.ValidateStockForSaleAsync(viewModel.Bill.BillItems, companyId);
                if (stockError != null)
                {
                    return Json(new { success = false, message = stockError });
                }

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

                // After successful save: update stock (decrease) and party balance (increase)
                if (billId > 0)
                {
                    using var connection = new NpgsqlConnection(_connectionString);
                    await connection.OpenAsync();
                    using var transaction = await connection.BeginTransactionAsync();
                    try
                    {
                        await _stockAndBalanceService.UpdateStockAndBalanceForSaleAsync(
                            connection, transaction,
                            viewModel.Bill.BillItems,
                            viewModel.Bill.PartyId,
                            viewModel.Bill.Total,
                            companyId);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                string pdfPath = string.Empty;
                if (billId > 0)
                {
                    pdfPath = await _salesInvoicesPdf.GetContractPdfById(billId, _environment);
                }

                return Json(new { success = true, message = "Data saved successfully!", pdfPath = pdfPath });


            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Sales Create POST Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = false, message = "Error: " + ex.Message });

            }

        }

        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
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
            // If Total is already set from client-side (form binding), keep it
            if (bill.Total > 0)
                return;

            // Fallback: calculate from items if Total was not bound from form
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

            if (bill.IsRoundOff)
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
                return Json(new { success = true, message = "Record deleted successfully!" });

            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Sales DeleteConfirmed Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = false, message = "Error: " + ex.Message });

            }

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
                await _errorLogService.LogErrorAsync($"Sales UpdateEntries Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = false, message = "Error: " + ex.Message });

            }

        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> DownloadInvoicePdf(int id)
        {
            try
            {
                CleanupOldInvoicePdfs(_environment);

                string relativePath = await _salesInvoicesPdf.GetContractPdfById(id, _environment);

                if (string.IsNullOrEmpty(relativePath))
                    return NotFound("PDF could not be generated.");

                string absolutePath = Path.Combine(_environment.WebRootPath,
                    relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (!System.IO.File.Exists(absolutePath))
                    return NotFound("PDF file not found on server.");

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
                string downloadFileName = $"Invoice_{id}_{DateTime.Now:ddMMyyyyHHmm}.pdf";

                return File(fileBytes, "application/pdf", downloadFileName);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Sales DownloadInvoicePdf Error: {ex.Message}", ex.StackTrace);
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }

        private void CleanupOldInvoicePdfs(IWebHostEnvironment env, int keepLastNFiles = 100)
        {
            try
            {
                string folderPath = Path.Combine(env.WebRootPath, "DataContainer", "GeneratedInvoices");
                if (!Directory.Exists(folderPath)) return;

                var files = new DirectoryInfo(folderPath)
                    .GetFiles("*.pdf")
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(keepLastNFiles);

                foreach (var file in files)
                {
                    file.Delete();
                }
            }
            catch { }
        }

        [HttpGet]
        public async Task<IActionResult> GetGstRates(string? stateOfSupply = null)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                var gstOptions = await _gstTaxService.GetGstRateOptionsAsync(companyId, stateOfSupply);
                bool isSameState = await _gstTaxService.IsSameStateAsync(companyId, stateOfSupply);
                return Json(new
                {
                    success = true,
                    isSameState,
                    taxType = isSameState ? "CGST_SGST" : "IGST",
                    options = gstOptions.Select(o => new
                    {
                        value = o.TaxPercentage.ToString("0.##"),
                        text = o.DisplayText,
                        taxType = o.TaxType,
                        cgstRate = o.CgstRate,
                        sgstRate = o.SgstRate,
                        igstRate = o.IgstRate,
                        isSameState = o.IsSameState
                    })
                });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync($"Sales GetGstRates Error: {ex.Message}", ex.StackTrace);
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
