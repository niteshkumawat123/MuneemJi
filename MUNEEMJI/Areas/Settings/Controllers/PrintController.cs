using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using MUNEEMJI.PdfServices.Quest;
using MUNEEMJI.Services;
using Npgsql;
using QuestPDF.Fluent;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    /// <summary>
    /// Settings &gt; PRINT - the PDF template designer.
    /// Every option maps to a column on public.print_settings and is applied
    /// by QuestInvoiceDocument at render time.
    /// </summary>
    [Area("Settings")]
    [Authorize]
    public class PrintController : Controller
    {
        private readonly IPrintSettingsService _printSettings;
        private readonly ICompanyTenancy _tenancy;
        private readonly IWebHostEnvironment _env;
        private readonly string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public PrintController(IPrintSettingsService printSettings, ICompanyTenancy tenancy, IWebHostEnvironment env)
        {
            _printSettings = printSettings;
            _tenancy = tenancy;
            _env = env;
        }

        private int CompanyId()
        {
            try
            {
                return _tenancy.GetCurrentCompanyId();
            }
            catch
            {
                return 0;
            }
        }

        private string CurrentUser()
        {
            return HttpContext?.Session?.GetString("UserName")
                   ?? User?.Identity?.Name
                   ?? "system";
        }

        // =================================================================
        //  Screen
        // =================================================================
        [HttpGet]
        public IActionResult Index()
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return RedirectToAction("Login", "Account", new { area = "" });

            var model = _printSettings.GetViewModel(companyId);

            ViewBag.CompanyId = companyId;
            ViewBag.DocumentTypes = PrintSettingsService.DefaultTransactionTitles;
            ViewBag.ColumnCatalog = PrintItemColumnCatalog.All;

            return View(model);
        }

        // =================================================================
        //  Save - Change Layout tab and Change Colors tab both post here
        // =================================================================
        [HttpPost]
        public IActionResult SavePrintSettings([FromBody] PrintSettingsModel model)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            if (model == null)
                return Json(new { success = false, message = "No data received." });

            model.CompanyId = companyId;

            var ok = _printSettings.SaveSettings(model, CurrentUser(), out var message);
            return Json(new { success = ok, message });
        }

        // =================================================================
        //  Theme selection - copies the theme palette onto the settings row
        // =================================================================
        [HttpPost]
        public IActionResult ApplyTheme([FromBody] ApplyThemeRequest request)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            if (request == null || request.ThemeId <= 0)
                return Json(new { success = false, message = "No theme selected." });

            var ok = _printSettings.ResetToTheme(
                companyId,
                string.IsNullOrWhiteSpace(request.PrinterType) ? "Regular" : request.PrinterType,
                request.ThemeId,
                CurrentUser(),
                out var message);

            if (!ok) return Json(new { success = false, message });

            var settings = _printSettings.GetSettings(companyId,
                string.IsNullOrWhiteSpace(request.PrinterType) ? "Regular" : request.PrinterType);

            return Json(new
            {
                success = true,
                message = "Theme applied successfully.",
                colors = new
                {
                    primaryColor = settings.EffectivePrimaryColor,
                    headerBgColor = settings.EffectiveHeaderBgColor,
                    borderColor = settings.EffectiveBorderColor,
                    totalRowColor = settings.EffectiveTotalRowColor,
                    headerTextColor = settings.EffectiveHeaderTextColor
                },
                orientation = settings.Orientation
            });
        }

        // =================================================================
        //  Change Transaction Names
        // =================================================================
        [HttpGet]
        public IActionResult GetTransactionNames()
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            return Json(new { success = true, data = _printSettings.GetTransactionNames(companyId) });
        }

        [HttpPost]
        public IActionResult SaveTransactionNames([FromBody] List<PrintTransactionNameModel> rows)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            var ok = _printSettings.SaveTransactionNames(companyId, rows, out var message);
            return Json(new { success = ok, message });
        }

        // =================================================================
        //  Item Table Customization
        // =================================================================
        [HttpGet]
        public IActionResult GetItemColumns(int documentTypeId = 0)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            var columns = _printSettings.GetItemColumns(companyId, documentTypeId);

            var payload = columns.Select(c =>
            {
                var def = PrintItemColumnCatalog.Find(c.ColumnKey);
                return new
                {
                    columnKey = c.ColumnKey,
                    headerText = string.IsNullOrWhiteSpace(c.HeaderText) ? def?.DefaultHeader : c.HeaderText,
                    defaultHeader = def?.DefaultHeader,
                    isVisible = c.IsVisible,
                    sortOrder = c.SortOrder,
                    widthPercent = c.WidthPercent > 0 ? c.WidthPercent : (def?.DefaultWidth ?? 8m)
                };
            });

            return Json(new { success = true, data = payload });
        }

        [HttpPost]
        public IActionResult SaveItemColumns([FromBody] SaveItemColumnsRequest request)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            if (request == null || request.Columns == null)
                return Json(new { success = false, message = "No data received." });

            var ok = _printSettings.SaveItemColumns(companyId, request.DocumentTypeId, request.Columns, out var message);
            return Json(new { success = ok, message });
        }

        // =================================================================
        //  Live preview - renders a real PDF in memory, never touching disk
        // =================================================================
        [HttpPost]
        public async Task<IActionResult> PreviewPdf([FromBody] PreviewRequest request)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Content("No company context found.");

            try
            {
                QuestPdfEngine.EnsureInitialised(_env);

                var printerType = string.IsNullOrWhiteSpace(request?.PrinterType) ? "Regular" : request.PrinterType;
                var documentTypeId = request?.DocumentTypeId ?? (int)TradeDocumentTypes.SalesChallan;

                var context = _printSettings.GetPdfContextForCompany(companyId, documentTypeId, printerType);

                // Unsaved edits win, so the preview updates before the auto-save lands.
                if (request?.Settings != null)
                {
                    request.Settings.CompanyId = companyId;
                    request.Settings.PrinterType = printerType;
                    request.Settings.Theme = _printSettings.GetThemes()
                        .FirstOrDefault(t => t.Id == request.Settings.ThemeId) ?? context.Settings.Theme;
                    context.Settings = request.Settings;
                }

                QuestDocumentData data;
                var sampleId = FindLatestDocumentId(companyId, documentTypeId);

                if (sampleId > 0)
                {
                    var loader = new QuestPdfDataLoader();
                    data = await loader.LoadAsync(sampleId, context,
                        PrintSettingsService.DefaultTitleFor(documentTypeId));
                }
                else
                {
                    data = QuestPdfDataLoader.BuildSample(context,
                        PrintSettingsService.DefaultTitleFor(documentTypeId));
                }

                var bytes = QuestPdfGeneratorBase.Render(data, _env);

                Response.Headers["Content-Disposition"] = "inline; filename=preview.pdf";
                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Preview failed: " + ex.Message);
            }
        }

        /// <summary>Most recent document of this type for the company, or 0.</summary>
        private int FindLatestDocumentId(int companyId, int documentTypeId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT id FROM tradedocuments
                                   WHERE companyid = @p_companyid
                                     AND (@p_doctype <= 0 OR tradedocumenttypesid = @p_doctype)
                                   ORDER BY id DESC
                                   LIMIT 1";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", companyId);
                cmd.Parameters.AddWithValue("p_doctype", documentTypeId);

                var value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        // =================================================================
        //  Company Logo / Signature - "Change" links on the Print screen
        // =================================================================
        [HttpPost]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            return await SaveCompanyImage(file, "logos", "logo_path");
        }

        [HttpPost]
        public async Task<IActionResult> UploadSignature(IFormFile file)
        {
            return await SaveCompanyImage(file, "signatures", "signature_path");
        }

        private async Task<IActionResult> SaveCompanyImage(IFormFile file, string subfolder, string column)
        {
            var companyId = CompanyId();
            if (companyId <= 0)
                return Json(new { success = false, message = "No company context found." });

            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file selected." });

            var allowed = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowed.Contains(extension))
                return Json(new { success = false, message = "Only image files are allowed." });

            if (file.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "Image must be 5 MB or smaller." });

            try
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads", subfolder);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Same stored format the Business Profile screen uses.
                var storedPath = $"Web/uploads/{subfolder}/{fileName}";

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var sql = $"UPDATE business_profiles SET {column} = @p_path WHERE businessesid = @p_companyid";
                    using var cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("p_path", storedPath);
                    cmd.Parameters.AddWithValue("p_companyid", companyId);
                    await cmd.ExecuteNonQueryAsync();
                }

                _printSettings.InvalidateCompany(companyId);

                return Json(new
                {
                    success = true,
                    message = "Image updated successfully.",
                    path = "/" + storedPath.Substring("Web/".Length)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // =====================================================================
    //  Request payloads
    // =====================================================================
    public class ApplyThemeRequest
    {
        public int ThemeId { get; set; }
        public string PrinterType { get; set; } = "Regular";
    }

    public class SaveItemColumnsRequest
    {
        public int DocumentTypeId { get; set; }
        public List<PrintItemColumnModel> Columns { get; set; } = new List<PrintItemColumnModel>();
    }

    public class PreviewRequest
    {
        public string PrinterType { get; set; } = "Regular";
        public int DocumentTypeId { get; set; }
        public PrintSettingsModel Settings { get; set; }
    }
}
