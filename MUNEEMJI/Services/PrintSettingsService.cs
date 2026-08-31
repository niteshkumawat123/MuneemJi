using Microsoft.Extensions.Caching.Memory;
using MUNEEMJI.Models;
using MUNEEMJI.Models.BankAccount;
using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Services
{
    public class PrintSettingsService : IPrintSettingsService
    {
        private readonly IMemoryCache _cache;
        private readonly string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(20);
        private const string ThemesCacheKey = "print_themes_all";

        public PrintSettingsService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // =================================================================
        //  Cache keys
        // =================================================================
        private static string SettingsKey(int companyId, string printerType) => $"print_settings_{companyId}_{printerType}";
        private static string TxnNamesKey(int companyId) => $"print_txn_names_{companyId}";
        private static string ItemColsKey(int companyId, int docType) => $"print_item_cols_{companyId}_{docType}";
        private static string BankKey(int companyId) => $"print_bank_{companyId}";
        private static string CompanyKey(int companyId) => $"print_company_{companyId}";

        public void InvalidateCompany(int companyId)
        {
            _cache.Remove(SettingsKey(companyId, "Regular"));
            _cache.Remove(SettingsKey(companyId, "Thermal"));
            _cache.Remove(TxnNamesKey(companyId));
            _cache.Remove(BankKey(companyId));
            _cache.Remove(CompanyKey(companyId));
            _cache.Remove(ItemColsKey(companyId, 0));
            foreach (var t in Enum.GetValues(typeof(TradeDocumentTypes)))
                _cache.Remove(ItemColsKey(companyId, (int)t));
        }

        // =================================================================
        //  Themes
        // =================================================================
        public List<PrintThemeModel> GetThemes()
        {
            if (_cache.TryGetValue(ThemesCacheKey, out List<PrintThemeModel> cached) && cached != null)
                return cached;

            var list = new List<PrintThemeModel>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT * FROM public.print_themes
                                   WHERE is_active = TRUE
                                   ORDER BY sort_order, id";
                using var cmd = new NpgsqlCommand(q, conn);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new PrintThemeModel
                    {
                        Id = Int(r, "id"),
                        ThemeKey = Str(r, "theme_key"),
                        DisplayName = Str(r, "display_name"),
                        LayoutKey = Str(r, "layout_key"),
                        Orientation = Str(r, "orientation"),
                        PrinterType = Str(r, "printer_type") ?? "Regular",
                        PrimaryColor = Str(r, "primary_color"),
                        HeaderBgColor = Str(r, "header_bg_color"),
                        BorderColor = Str(r, "border_color"),
                        TotalRowColor = Str(r, "total_row_color"),
                        HeaderTextColor = Str(r, "header_text_color"),
                        SortOrder = Int(r, "sort_order"),
                        IsActive = Bool(r, "is_active")
                    });
                }
            }
            catch
            {
                // Table not created yet - fall back to a single built-in theme so the
                // PDF pipeline keeps working before print_settings.sql has been run.
            }

            if (list.Count == 0)
            {
                list.Add(new PrintThemeModel
                {
                    Id = 1,
                    ThemeKey = "tally",
                    DisplayName = "Tally Theme",
                    LayoutKey = "standard",
                    Orientation = "Portrait",
                    PrinterType = "Regular",
                    PrimaryColor = "#4E2A0A",
                    HeaderBgColor = "#BBBBBB",
                    BorderColor = "#A9A9A9",
                    TotalRowColor = "#FFF3CD",
                    HeaderTextColor = "#FFFFFF",
                    SortOrder = 1,
                    IsActive = true
                });
                list.Add(new PrintThemeModel
                {
                    Id = 101,
                    ThemeKey = "thermal1",
                    DisplayName = "Theme 1",
                    LayoutKey = "thermal-classic",
                    Orientation = "Portrait",
                    PrinterType = "Thermal",
                    PrimaryColor = "#000000",
                    HeaderBgColor = "#FFFFFF",
                    BorderColor = "#000000",
                    TotalRowColor = "#FFFFFF",
                    HeaderTextColor = "#000000",
                    SortOrder = 1,
                    IsActive = true
                });
            }

            _cache.Set(ThemesCacheKey, list, CacheTtl);
            return list;
        }

        public List<PrintThemeModel> GetThemes(string printerType)
        {
            if (string.IsNullOrWhiteSpace(printerType)) printerType = "Regular";

            var matching = GetThemes()
                .Where(t => string.Equals(t.PrinterType, printerType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.SortOrder).ThenBy(t => t.Id)
                .ToList();

            // Pre-migration databases have no printer_type column: fall back to all.
            return matching.Count > 0 ? matching : GetThemes();
        }

        // =================================================================
        //  Settings
        // =================================================================
        public PrintSettingsModel GetSettings(int companyId, string printerType)
        {
            if (string.IsNullOrWhiteSpace(printerType)) printerType = "Regular";

            var key = SettingsKey(companyId, printerType);
            if (_cache.TryGetValue(key, out PrintSettingsModel cached) && cached != null)
                return cached;

            PrintSettingsModel model = ReadSettingsRow(companyId, printerType);

            if (model == null)
            {
                model = BuildDefaults(companyId, printerType);
                TryInsertDefaults(model);
            }

            model.Theme = GetThemes().FirstOrDefault(t => t.Id == model.ThemeId) ?? GetThemes().FirstOrDefault();

            _cache.Set(key, model, CacheTtl);
            return model;
        }

        private PrintSettingsModel ReadSettingsRow(int companyId, string printerType)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT * FROM public.print_settings
                                   WHERE companyid = @p_companyid AND printer_type = @p_printer
                                   LIMIT 1";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", companyId);
                cmd.Parameters.AddWithValue("p_printer", printerType);
                using var r = cmd.ExecuteReader();
                if (!r.Read()) return null;

                return new PrintSettingsModel
                {
                    Id = Convert.ToInt32(r["id"]),
                    CompanyId = Convert.ToInt32(r["companyid"]),
                    PrinterType = r["printer_type"]?.ToString(),
                    ThemeId = r["theme_id"] != DBNull.Value ? Convert.ToInt32(r["theme_id"]) : 1,

                    PrimaryColor = Str(r, "primary_color"),
                    HeaderBgColor = Str(r, "header_bg_color"),
                    BorderColor = Str(r, "border_color"),
                    TotalRowColor = Str(r, "total_row_color"),
                    HeaderTextColor = Str(r, "header_text_color"),

                    MakeDefault = Bool(r, "make_default"),
                    RepeatHeader = Bool(r, "repeat_header"),
                    PrintCompanyName = Bool(r, "print_company_name"),
                    CompanyNameText = Str(r, "company_name_text"),
                    PrintLogo = Bool(r, "print_logo"),
                    PrintAddress = Bool(r, "print_address"),
                    AddressText = Str(r, "address_text"),
                    PrintEmail = Bool(r, "print_email"),
                    EmailText = Str(r, "email_text"),
                    PrintPhone = Bool(r, "print_phone"),
                    PhoneText = Str(r, "phone_text"),
                    PrintGstin = Bool(r, "print_gstin"),
                    GstinText = Str(r, "gstin_text"),
                    PrintState = Bool(r, "print_state"),

                    PaperSize = Str(r, "paper_size"),
                    Orientation = Str(r, "orientation"),
                    CompanyNameTextSize = Str(r, "company_name_text_size"),
                    InvoiceTextSize = Str(r, "invoice_text_size"),
                    ExtraSpaceTop = Dec(r, "extra_space_top"),
                    MarginLeft = Dec(r, "margin_left"),
                    MarginRight = Dec(r, "margin_right"),
                    MarginBottom = Dec(r, "margin_bottom"),
                    FontFamily = Str(r, "font_family"),

                    PrintOriginalDuplicate = Bool(r, "print_original_duplicate"),
                    PrintCopyOriginal = Bool(r, "print_copy_original"),
                    LabelOriginal = Str(r, "label_original"),
                    PrintCopyDuplicate = Bool(r, "print_copy_duplicate"),
                    LabelDuplicate = Str(r, "label_duplicate"),
                    PrintCopyTriplicate = Bool(r, "print_copy_triplicate"),
                    LabelTriplicate = Str(r, "label_triplicate"),

                    ExpandItemTable = Bool(r, "expand_item_table"),
                    MinItemRows = Int(r, "min_item_rows"),

                    PrintTotalItemQuantity = Bool(r, "print_total_item_quantity"),
                    PrintAmountWithDecimal = Bool(r, "print_amount_with_decimal"),
                    PrintReceivedAmount = Bool(r, "print_received_amount"),
                    PrintBalanceAmount = Bool(r, "print_balance_amount"),
                    PrintCurrentBalanceParty = Bool(r, "print_current_balance_party"),
                    PrintTaxDetails = Bool(r, "print_tax_details"),
                    PrintYouSaved = Bool(r, "print_you_saved"),
                    PrintAmountWithGrouping = Bool(r, "print_amount_with_grouping"),
                    AmountInWordsFormat = Str(r, "amount_in_words_format"),

                    PrintDescription = Bool(r, "print_description"),
                    PrintTermsConditions = Bool(r, "print_terms_conditions"),
                    DefaultTermsText = Str(r, "default_terms_text"),
                    PrintReceivedBy = Bool(r, "print_received_by"),
                    PrintDeliveredBy = Bool(r, "print_delivered_by"),
                    PrintSignatureText = Bool(r, "print_signature_text"),
                    SignatureText = Str(r, "signature_text"),
                    PrintSignatureImage = Bool(r, "print_signature_image"),
                    PrintPaymentMode = Bool(r, "print_payment_mode"),
                    PrintAcknowledgement = Bool(r, "print_acknowledgement"),

                    PrintBankDetails = Bool(r, "print_bank_details"),
                    PrintUpiQr = Bool(r, "print_upi_qr"),

                    PrintPageNumbers = Bool(r, "print_page_numbers"),
                    WatermarkText = Str(r, "watermark_text"),

                    PrintingType = Str(r, "printing_type") ?? "Text Printing",
                    UseTextStyling = Bool(r, "use_text_styling"),
                    AutoCutPaper = Bool(r, "auto_cut_paper"),
                    OpenCashDrawer = Bool(r, "open_cash_drawer"),
                    ExtraLinesEnd = Int(r, "extra_lines_end"),
                    NumberOfCopies = Math.Max(1, Int(r, "number_of_copies")),
                    CustomChars = Int(r, "custom_chars") > 0 ? Int(r, "custom_chars") : 48,

                    PrintItemSrNo = Bool(r, "print_item_srno"),
                    PrintItemHsn = Bool(r, "print_item_hsn"),
                    PrintItemUom = Bool(r, "print_item_uom"),
                    PrintItemMrp = Bool(r, "print_item_mrp"),
                    PrintItemDescription = Bool(r, "print_item_description"),
                    PrintItemBatchNo = Bool(r, "print_item_batch_no"),
                    PrintItemExpDate = Bool(r, "print_item_exp_date"),
                    PrintItemMfgDate = Bool(r, "print_item_mfg_date"),
                    PrintItemModelNo = Bool(r, "print_item_model_no"),
                    PrintItemSize = Bool(r, "print_item_size"),
                    PrintItemSerialNo = Bool(r, "print_item_serial_no"),

                    CreatedAt = Dt(r, "created_at"),
                    UpdatedAt = Dt(r, "updated_at"),
                    UpdatedBy = Str(r, "updated_by")
                };
            }
            catch
            {
                return null;
            }
        }

        private PrintSettingsModel BuildDefaults(int companyId, string printerType)
        {
            var defaultTheme = GetThemes(printerType).FirstOrDefault();
            var model = new PrintSettingsModel
            {
                CompanyId = companyId,
                PrinterType = printerType,
                ThemeId = defaultTheme?.Id ?? 1,
                Theme = defaultTheme
            };

            if (string.Equals(printerType, "Thermal", StringComparison.OrdinalIgnoreCase))
            {
                model.PaperSize = "3 Inch";
                model.Orientation = "Portrait";
                model.CompanyNameTextSize = "Medium";
                model.InvoiceTextSize = "Small";
                model.MarginLeft = 6m;
                model.MarginRight = 6m;
                model.MarginBottom = 6m;
                model.PrintOriginalDuplicate = false;
                model.PrintCopyDuplicate = false;
                model.PrintDeliveredBy = false;
                model.PrintReceivedBy = false;
                model.PrintBankDetails = false;
                model.PrintSignatureImage = false;
                model.PrintSignatureText = false;
                model.ExpandItemTable = false;
                model.MakeDefault = false;
                model.PrintingType = "Text Printing";
                model.UseTextStyling = true;
                model.NumberOfCopies = 1;
                model.CustomChars = 48;
            }

            return model;
        }

        private void TryInsertDefaults(PrintSettingsModel m)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"
INSERT INTO public.print_settings
    (companyid, printer_type, theme_id, paper_size, orientation,
     company_name_text_size, invoice_text_size, margin_left, margin_right, margin_bottom,
     print_original_duplicate, print_copy_duplicate, print_tax_details,
     print_delivered_by, print_received_by, print_bank_details,
     expand_item_table, make_default)
VALUES
    (@p_companyid, @p_printer, @p_theme, @p_paper, @p_orientation,
     @p_cnsize, @p_invsize, @p_ml, @p_mr, @p_mb,
     @p_origdup, @p_copydup, @p_tax,
     @p_delby, @p_recby, @p_bank,
     @p_expand, @p_default)
ON CONFLICT (companyid, printer_type) DO NOTHING
RETURNING id";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", m.CompanyId);
                cmd.Parameters.AddWithValue("p_printer", m.PrinterType ?? "Regular");
                cmd.Parameters.AddWithValue("p_theme", m.ThemeId);
                cmd.Parameters.AddWithValue("p_paper", m.PaperSize ?? "A4");
                cmd.Parameters.AddWithValue("p_orientation", m.Orientation ?? "Portrait");
                cmd.Parameters.AddWithValue("p_cnsize", m.CompanyNameTextSize ?? "Large");
                cmd.Parameters.AddWithValue("p_invsize", m.InvoiceTextSize ?? "Medium");
                cmd.Parameters.AddWithValue("p_ml", m.MarginLeft);
                cmd.Parameters.AddWithValue("p_mr", m.MarginRight);
                cmd.Parameters.AddWithValue("p_mb", m.MarginBottom);
                cmd.Parameters.AddWithValue("p_origdup", m.PrintOriginalDuplicate);
                cmd.Parameters.AddWithValue("p_copydup", m.PrintCopyDuplicate);
                cmd.Parameters.AddWithValue("p_tax", m.PrintTaxDetails);
                cmd.Parameters.AddWithValue("p_delby", m.PrintDeliveredBy);
                cmd.Parameters.AddWithValue("p_recby", m.PrintReceivedBy);
                cmd.Parameters.AddWithValue("p_bank", m.PrintBankDetails);
                cmd.Parameters.AddWithValue("p_expand", m.ExpandItemTable);
                cmd.Parameters.AddWithValue("p_default", m.MakeDefault);

                var newId = cmd.ExecuteScalar();
                if (newId != null && newId != DBNull.Value)
                    m.Id = Convert.ToInt32(newId);
            }
            catch
            {
                // print_settings.sql not run yet - in-memory defaults still work.
            }
        }

        public bool SaveSettings(PrintSettingsModel model, string updatedBy, out string message)
        {
            message = string.Empty;
            if (model == null) { message = "No data received."; return false; }
            if (model.CompanyId <= 0) { message = "No company context found."; return false; }
            if (string.IsNullOrWhiteSpace(model.PrinterType)) model.PrinterType = "Regular";

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"
INSERT INTO public.print_settings (
    companyid, printer_type, theme_id,
    primary_color, header_bg_color, border_color, total_row_color, header_text_color,
    make_default, repeat_header,
    print_company_name, company_name_text, print_logo,
    print_address, address_text, print_email, email_text,
    print_phone, phone_text, print_gstin, gstin_text, print_state,
    paper_size, orientation, company_name_text_size, invoice_text_size,
    extra_space_top, margin_left, margin_right, margin_bottom, font_family,
    print_original_duplicate, print_copy_original, label_original,
    print_copy_duplicate, label_duplicate, print_copy_triplicate, label_triplicate,
    expand_item_table, min_item_rows,
    print_total_item_quantity, print_amount_with_decimal, print_received_amount,
    print_balance_amount, print_current_balance_party, print_tax_details,
    print_you_saved, print_amount_with_grouping, amount_in_words_format,
    print_description, print_terms_conditions, default_terms_text,
    print_received_by, print_delivered_by, print_signature_text, signature_text,
    print_signature_image, print_payment_mode, print_acknowledgement,
    print_bank_details, print_upi_qr, print_page_numbers, watermark_text,
    printing_type, use_text_styling, auto_cut_paper, open_cash_drawer,
    extra_lines_end, number_of_copies, custom_chars,
    print_item_srno, print_item_hsn, print_item_uom, print_item_mrp, print_item_description,
    print_item_batch_no, print_item_exp_date, print_item_mfg_date,
    print_item_model_no, print_item_size, print_item_serial_no,
    updated_at, updated_by
) VALUES (
    @companyid, @printer_type, @theme_id,
    @primary_color, @header_bg_color, @border_color, @total_row_color, @header_text_color,
    @make_default, @repeat_header,
    @print_company_name, @company_name_text, @print_logo,
    @print_address, @address_text, @print_email, @email_text,
    @print_phone, @phone_text, @print_gstin, @gstin_text, @print_state,
    @paper_size, @orientation, @company_name_text_size, @invoice_text_size,
    @extra_space_top, @margin_left, @margin_right, @margin_bottom, @font_family,
    @print_original_duplicate, @print_copy_original, @label_original,
    @print_copy_duplicate, @label_duplicate, @print_copy_triplicate, @label_triplicate,
    @expand_item_table, @min_item_rows,
    @print_total_item_quantity, @print_amount_with_decimal, @print_received_amount,
    @print_balance_amount, @print_current_balance_party, @print_tax_details,
    @print_you_saved, @print_amount_with_grouping, @amount_in_words_format,
    @print_description, @print_terms_conditions, @default_terms_text,
    @print_received_by, @print_delivered_by, @print_signature_text, @signature_text,
    @print_signature_image, @print_payment_mode, @print_acknowledgement,
    @print_bank_details, @print_upi_qr, @print_page_numbers, @watermark_text,
    @printing_type, @use_text_styling, @auto_cut_paper, @open_cash_drawer,
    @extra_lines_end, @number_of_copies, @custom_chars,
    @print_item_srno, @print_item_hsn, @print_item_uom, @print_item_mrp, @print_item_description,
    @print_item_batch_no, @print_item_exp_date, @print_item_mfg_date,
    @print_item_model_no, @print_item_size, @print_item_serial_no,
    NOW(), @updated_by
)
ON CONFLICT (companyid, printer_type) DO UPDATE SET
    theme_id = EXCLUDED.theme_id,
    primary_color = EXCLUDED.primary_color,
    header_bg_color = EXCLUDED.header_bg_color,
    border_color = EXCLUDED.border_color,
    total_row_color = EXCLUDED.total_row_color,
    header_text_color = EXCLUDED.header_text_color,
    make_default = EXCLUDED.make_default,
    repeat_header = EXCLUDED.repeat_header,
    print_company_name = EXCLUDED.print_company_name,
    company_name_text = EXCLUDED.company_name_text,
    print_logo = EXCLUDED.print_logo,
    print_address = EXCLUDED.print_address,
    address_text = EXCLUDED.address_text,
    print_email = EXCLUDED.print_email,
    email_text = EXCLUDED.email_text,
    print_phone = EXCLUDED.print_phone,
    phone_text = EXCLUDED.phone_text,
    print_gstin = EXCLUDED.print_gstin,
    gstin_text = EXCLUDED.gstin_text,
    print_state = EXCLUDED.print_state,
    paper_size = EXCLUDED.paper_size,
    orientation = EXCLUDED.orientation,
    company_name_text_size = EXCLUDED.company_name_text_size,
    invoice_text_size = EXCLUDED.invoice_text_size,
    extra_space_top = EXCLUDED.extra_space_top,
    margin_left = EXCLUDED.margin_left,
    margin_right = EXCLUDED.margin_right,
    margin_bottom = EXCLUDED.margin_bottom,
    font_family = EXCLUDED.font_family,
    print_original_duplicate = EXCLUDED.print_original_duplicate,
    print_copy_original = EXCLUDED.print_copy_original,
    label_original = EXCLUDED.label_original,
    print_copy_duplicate = EXCLUDED.print_copy_duplicate,
    label_duplicate = EXCLUDED.label_duplicate,
    print_copy_triplicate = EXCLUDED.print_copy_triplicate,
    label_triplicate = EXCLUDED.label_triplicate,
    expand_item_table = EXCLUDED.expand_item_table,
    min_item_rows = EXCLUDED.min_item_rows,
    print_total_item_quantity = EXCLUDED.print_total_item_quantity,
    print_amount_with_decimal = EXCLUDED.print_amount_with_decimal,
    print_received_amount = EXCLUDED.print_received_amount,
    print_balance_amount = EXCLUDED.print_balance_amount,
    print_current_balance_party = EXCLUDED.print_current_balance_party,
    print_tax_details = EXCLUDED.print_tax_details,
    print_you_saved = EXCLUDED.print_you_saved,
    print_amount_with_grouping = EXCLUDED.print_amount_with_grouping,
    amount_in_words_format = EXCLUDED.amount_in_words_format,
    print_description = EXCLUDED.print_description,
    print_terms_conditions = EXCLUDED.print_terms_conditions,
    default_terms_text = EXCLUDED.default_terms_text,
    print_received_by = EXCLUDED.print_received_by,
    print_delivered_by = EXCLUDED.print_delivered_by,
    print_signature_text = EXCLUDED.print_signature_text,
    signature_text = EXCLUDED.signature_text,
    print_signature_image = EXCLUDED.print_signature_image,
    print_payment_mode = EXCLUDED.print_payment_mode,
    print_acknowledgement = EXCLUDED.print_acknowledgement,
    print_bank_details = EXCLUDED.print_bank_details,
    print_upi_qr = EXCLUDED.print_upi_qr,
    print_page_numbers = EXCLUDED.print_page_numbers,
    watermark_text = EXCLUDED.watermark_text,
    printing_type = EXCLUDED.printing_type,
    use_text_styling = EXCLUDED.use_text_styling,
    auto_cut_paper = EXCLUDED.auto_cut_paper,
    open_cash_drawer = EXCLUDED.open_cash_drawer,
    extra_lines_end = EXCLUDED.extra_lines_end,
    number_of_copies = EXCLUDED.number_of_copies,
    custom_chars = EXCLUDED.custom_chars,
    print_item_srno = EXCLUDED.print_item_srno,
    print_item_hsn = EXCLUDED.print_item_hsn,
    print_item_uom = EXCLUDED.print_item_uom,
    print_item_mrp = EXCLUDED.print_item_mrp,
    print_item_description = EXCLUDED.print_item_description,
    print_item_batch_no = EXCLUDED.print_item_batch_no,
    print_item_exp_date = EXCLUDED.print_item_exp_date,
    print_item_mfg_date = EXCLUDED.print_item_mfg_date,
    print_item_model_no = EXCLUDED.print_item_model_no,
    print_item_size = EXCLUDED.print_item_size,
    print_item_serial_no = EXCLUDED.print_item_serial_no,
    updated_at = NOW(),
    updated_by = EXCLUDED.updated_by";

                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("companyid", model.CompanyId);
                cmd.Parameters.AddWithValue("printer_type", model.PrinterType);
                cmd.Parameters.AddWithValue("theme_id", model.ThemeId <= 0 ? 1 : model.ThemeId);

                AddNullable(cmd, "primary_color", model.PrimaryColor);
                AddNullable(cmd, "header_bg_color", model.HeaderBgColor);
                AddNullable(cmd, "border_color", model.BorderColor);
                AddNullable(cmd, "total_row_color", model.TotalRowColor);
                AddNullable(cmd, "header_text_color", model.HeaderTextColor);

                cmd.Parameters.AddWithValue("make_default", model.MakeDefault);
                cmd.Parameters.AddWithValue("repeat_header", model.RepeatHeader);
                cmd.Parameters.AddWithValue("print_company_name", model.PrintCompanyName);
                AddNullable(cmd, "company_name_text", model.CompanyNameText);
                cmd.Parameters.AddWithValue("print_logo", model.PrintLogo);
                cmd.Parameters.AddWithValue("print_address", model.PrintAddress);
                AddNullable(cmd, "address_text", model.AddressText);
                cmd.Parameters.AddWithValue("print_email", model.PrintEmail);
                AddNullable(cmd, "email_text", model.EmailText);
                cmd.Parameters.AddWithValue("print_phone", model.PrintPhone);
                AddNullable(cmd, "phone_text", model.PhoneText);
                cmd.Parameters.AddWithValue("print_gstin", model.PrintGstin);
                AddNullable(cmd, "gstin_text", model.GstinText);
                cmd.Parameters.AddWithValue("print_state", model.PrintState);

                cmd.Parameters.AddWithValue("paper_size", model.PaperSize ?? "A4");
                cmd.Parameters.AddWithValue("orientation", model.Orientation ?? "Portrait");
                cmd.Parameters.AddWithValue("company_name_text_size", model.CompanyNameTextSize ?? "Large");
                cmd.Parameters.AddWithValue("invoice_text_size", model.InvoiceTextSize ?? "Medium");
                cmd.Parameters.AddWithValue("extra_space_top", model.ExtraSpaceTop);
                cmd.Parameters.AddWithValue("margin_left", model.MarginLeft);
                cmd.Parameters.AddWithValue("margin_right", model.MarginRight);
                cmd.Parameters.AddWithValue("margin_bottom", model.MarginBottom);
                cmd.Parameters.AddWithValue("font_family", model.FontFamily ?? "Arial");

                cmd.Parameters.AddWithValue("print_original_duplicate", model.PrintOriginalDuplicate);
                cmd.Parameters.AddWithValue("print_copy_original", model.PrintCopyOriginal);
                cmd.Parameters.AddWithValue("label_original", model.LabelOriginal ?? "ORIGINAL FOR RECIPIENT");
                cmd.Parameters.AddWithValue("print_copy_duplicate", model.PrintCopyDuplicate);
                cmd.Parameters.AddWithValue("label_duplicate", model.LabelDuplicate ?? "DUPLICATE FOR TRANSPORTER");
                cmd.Parameters.AddWithValue("print_copy_triplicate", model.PrintCopyTriplicate);
                cmd.Parameters.AddWithValue("label_triplicate", model.LabelTriplicate ?? "TRIPLICATE FOR SUPPLIER");

                cmd.Parameters.AddWithValue("expand_item_table", model.ExpandItemTable);
                cmd.Parameters.AddWithValue("min_item_rows", model.MinItemRows < 0 ? 0 : model.MinItemRows);

                cmd.Parameters.AddWithValue("print_total_item_quantity", model.PrintTotalItemQuantity);
                cmd.Parameters.AddWithValue("print_amount_with_decimal", model.PrintAmountWithDecimal);
                cmd.Parameters.AddWithValue("print_received_amount", model.PrintReceivedAmount);
                cmd.Parameters.AddWithValue("print_balance_amount", model.PrintBalanceAmount);
                cmd.Parameters.AddWithValue("print_current_balance_party", model.PrintCurrentBalanceParty);
                cmd.Parameters.AddWithValue("print_tax_details", model.PrintTaxDetails);
                cmd.Parameters.AddWithValue("print_you_saved", model.PrintYouSaved);
                cmd.Parameters.AddWithValue("print_amount_with_grouping", model.PrintAmountWithGrouping);
                cmd.Parameters.AddWithValue("amount_in_words_format", model.AmountInWordsFormat ?? "Indian");

                cmd.Parameters.AddWithValue("print_description", model.PrintDescription);
                cmd.Parameters.AddWithValue("print_terms_conditions", model.PrintTermsConditions);
                AddNullable(cmd, "default_terms_text", model.DefaultTermsText);
                cmd.Parameters.AddWithValue("print_received_by", model.PrintReceivedBy);
                cmd.Parameters.AddWithValue("print_delivered_by", model.PrintDeliveredBy);
                cmd.Parameters.AddWithValue("print_signature_text", model.PrintSignatureText);
                cmd.Parameters.AddWithValue("signature_text", model.SignatureText ?? "Authorized Signatory");
                cmd.Parameters.AddWithValue("print_signature_image", model.PrintSignatureImage);
                cmd.Parameters.AddWithValue("print_payment_mode", model.PrintPaymentMode);
                cmd.Parameters.AddWithValue("print_acknowledgement", model.PrintAcknowledgement);

                cmd.Parameters.AddWithValue("print_bank_details", model.PrintBankDetails);
                cmd.Parameters.AddWithValue("print_upi_qr", model.PrintUpiQr);
                cmd.Parameters.AddWithValue("print_page_numbers", model.PrintPageNumbers);
                AddNullable(cmd, "watermark_text", model.WatermarkText);

                cmd.Parameters.AddWithValue("printing_type", model.PrintingType ?? "Text Printing");
                cmd.Parameters.AddWithValue("use_text_styling", model.UseTextStyling);
                cmd.Parameters.AddWithValue("auto_cut_paper", model.AutoCutPaper);
                cmd.Parameters.AddWithValue("open_cash_drawer", model.OpenCashDrawer);
                cmd.Parameters.AddWithValue("extra_lines_end", model.ExtraLinesEnd < 0 ? 0 : model.ExtraLinesEnd);
                cmd.Parameters.AddWithValue("number_of_copies", model.NumberOfCopies < 1 ? 1 : model.NumberOfCopies);
                cmd.Parameters.AddWithValue("custom_chars", model.CustomChars < 20 ? 48 : model.CustomChars);

                cmd.Parameters.AddWithValue("print_item_srno", model.PrintItemSrNo);
                cmd.Parameters.AddWithValue("print_item_hsn", model.PrintItemHsn);
                cmd.Parameters.AddWithValue("print_item_uom", model.PrintItemUom);
                cmd.Parameters.AddWithValue("print_item_mrp", model.PrintItemMrp);
                cmd.Parameters.AddWithValue("print_item_description", model.PrintItemDescription);
                cmd.Parameters.AddWithValue("print_item_batch_no", model.PrintItemBatchNo);
                cmd.Parameters.AddWithValue("print_item_exp_date", model.PrintItemExpDate);
                cmd.Parameters.AddWithValue("print_item_mfg_date", model.PrintItemMfgDate);
                cmd.Parameters.AddWithValue("print_item_model_no", model.PrintItemModelNo);
                cmd.Parameters.AddWithValue("print_item_size", model.PrintItemSize);
                cmd.Parameters.AddWithValue("print_item_serial_no", model.PrintItemSerialNo);

                AddNullable(cmd, "updated_by", updatedBy);

                cmd.ExecuteNonQuery();

                _cache.Remove(SettingsKey(model.CompanyId, model.PrinterType));
                message = "Print settings saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool ResetToTheme(int companyId, string printerType, int themeId, string updatedBy, out string message)
        {
            var theme = GetThemes().FirstOrDefault(t => t.Id == themeId);
            if (theme == null) { message = "Theme not found."; return false; }

            var settings = GetSettings(companyId, printerType);
            settings.ThemeId = theme.Id;
            settings.PrimaryColor = theme.PrimaryColor;
            settings.HeaderBgColor = theme.HeaderBgColor;
            settings.BorderColor = theme.BorderColor;
            settings.TotalRowColor = theme.TotalRowColor;
            settings.HeaderTextColor = theme.HeaderTextColor;
            settings.Orientation = theme.Orientation;

            return SaveSettings(settings, updatedBy, out message);
        }

        // =================================================================
        //  View model for the Settings screen
        // =================================================================
        public PrintSettingsViewModel GetViewModel(int companyId)
        {
            return new PrintSettingsViewModel
            {
                Regular = GetSettings(companyId, "Regular"),
                Thermal = GetSettings(companyId, "Thermal"),
                Themes = GetThemes(),
                TransactionNames = GetTransactionNames(companyId),
                ItemColumns = GetItemColumns(companyId, 0),
                Company = GetCompany(companyId)
            };
        }

        // =================================================================
        //  Transaction names
        // =================================================================
        public List<PrintTransactionNameModel> GetTransactionNames(int companyId)
        {
            var key = TxnNamesKey(companyId);
            if (_cache.TryGetValue(key, out List<PrintTransactionNameModel> cached) && cached != null)
                return cached;

            var saved = new List<PrintTransactionNameModel>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT id, companyid, tradedocumenttypesid, display_title,
                                          label_original, label_duplicate, label_triplicate
                                   FROM public.print_transaction_names
                                   WHERE companyid = @p_companyid";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", companyId);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    saved.Add(new PrintTransactionNameModel
                    {
                        Id = Convert.ToInt32(r["id"]),
                        CompanyId = Convert.ToInt32(r["companyid"]),
                        TradeDocumentTypesId = Convert.ToInt32(r["tradedocumenttypesid"]),
                        DisplayTitle = r["display_title"]?.ToString(),
                        LabelOriginal = Str(r, "label_original"),
                        LabelDuplicate = Str(r, "label_duplicate"),
                        LabelTriplicate = Str(r, "label_triplicate")
                    });
                }
            }
            catch
            {
                // table missing - defaults below still render the screen
            }

            var result = new List<PrintTransactionNameModel>();
            foreach (var pair in DefaultTransactionTitles)
            {
                var row = saved.FirstOrDefault(x => x.TradeDocumentTypesId == pair.Key);
                result.Add(row ?? new PrintTransactionNameModel
                {
                    CompanyId = companyId,
                    TradeDocumentTypesId = pair.Key,
                    DisplayTitle = pair.Value,
                    LabelOriginal = "ORIGINAL FOR RECIPIENT",
                    LabelDuplicate = "DUPLICATE FOR TRANSPORTER",
                    LabelTriplicate = "TRIPLICATE FOR SUPPLIER"
                });
            }

            _cache.Set(key, result, CacheTtl);
            return result;
        }

        public bool SaveTransactionNames(int companyId, List<PrintTransactionNameModel> rows, out string message)
        {
            message = string.Empty;
            if (rows == null || rows.Count == 0) { message = "No data received."; return false; }
            if (companyId <= 0) { message = "No company context found."; return false; }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var tx = conn.BeginTransaction();

                const string q = @"
INSERT INTO public.print_transaction_names
    (companyid, tradedocumenttypesid, display_title, label_original, label_duplicate, label_triplicate, updated_at)
VALUES (@companyid, @doctype, @title, @lo, @ld, @lt, NOW())
ON CONFLICT (companyid, tradedocumenttypesid) DO UPDATE SET
    display_title = EXCLUDED.display_title,
    label_original = EXCLUDED.label_original,
    label_duplicate = EXCLUDED.label_duplicate,
    label_triplicate = EXCLUDED.label_triplicate,
    updated_at = NOW()";

                foreach (var row in rows)
                {
                    if (row == null || row.TradeDocumentTypesId <= 0) continue;
                    using var cmd = new NpgsqlCommand(q, conn, tx);
                    cmd.Parameters.AddWithValue("companyid", companyId);
                    cmd.Parameters.AddWithValue("doctype", row.TradeDocumentTypesId);
                    cmd.Parameters.AddWithValue("title",
                        string.IsNullOrWhiteSpace(row.DisplayTitle)
                            ? DefaultTitleFor(row.TradeDocumentTypesId)
                            : row.DisplayTitle.Trim());
                    AddNullable(cmd, "lo", row.LabelOriginal);
                    AddNullable(cmd, "ld", row.LabelDuplicate);
                    AddNullable(cmd, "lt", row.LabelTriplicate);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                _cache.Remove(TxnNamesKey(companyId));
                message = "Transaction names saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        // =================================================================
        //  Item columns
        // =================================================================
        public List<PrintItemColumnModel> GetItemColumns(int companyId, int tradeDocumentTypesId)
        {
            var key = ItemColsKey(companyId, tradeDocumentTypesId);
            if (_cache.TryGetValue(key, out List<PrintItemColumnModel> cached) && cached != null)
                return cached;

            var rows = ReadItemColumnRows(companyId, tradeDocumentTypesId);

            // Fall back to the "all document types" set when this type has none.
            if (rows.Count == 0 && tradeDocumentTypesId != 0)
                rows = ReadItemColumnRows(companyId, 0);

            var result = new List<PrintItemColumnModel>();
            int order = 0;
            foreach (var def in PrintItemColumnCatalog.All)
            {
                var saved = rows.FirstOrDefault(x => string.Equals(x.ColumnKey, def.Key, StringComparison.OrdinalIgnoreCase));
                result.Add(saved ?? new PrintItemColumnModel
                {
                    CompanyId = companyId,
                    TradeDocumentTypesId = tradeDocumentTypesId,
                    ColumnKey = def.Key,
                    HeaderText = def.DefaultHeader,
                    IsVisible = def.DefaultVisible,
                    SortOrder = order,
                    WidthPercent = def.DefaultWidth
                });
                order++;
            }

            result = result.OrderBy(x => x.SortOrder).ToList();
            _cache.Set(key, result, CacheTtl);
            return result;
        }

        private List<PrintItemColumnModel> ReadItemColumnRows(int companyId, int docType)
        {
            var rows = new List<PrintItemColumnModel>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT id, companyid, tradedocumenttypesid, column_key,
                                          header_text, is_visible, sort_order, width_percent
                                   FROM public.print_item_columns
                                   WHERE companyid = @p_companyid AND tradedocumenttypesid = @p_doctype
                                   ORDER BY sort_order";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", companyId);
                cmd.Parameters.AddWithValue("p_doctype", docType);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    rows.Add(new PrintItemColumnModel
                    {
                        Id = Convert.ToInt32(r["id"]),
                        CompanyId = Convert.ToInt32(r["companyid"]),
                        TradeDocumentTypesId = Convert.ToInt32(r["tradedocumenttypesid"]),
                        ColumnKey = r["column_key"]?.ToString(),
                        HeaderText = Str(r, "header_text"),
                        IsVisible = Bool(r, "is_visible"),
                        SortOrder = Int(r, "sort_order"),
                        WidthPercent = Dec(r, "width_percent")
                    });
                }
            }
            catch
            {
                // table missing - catalogue defaults are used instead
            }
            return rows;
        }

        public bool SaveItemColumns(int companyId, int tradeDocumentTypesId, List<PrintItemColumnModel> rows, out string message)
        {
            message = string.Empty;
            if (rows == null || rows.Count == 0) { message = "No data received."; return false; }
            if (companyId <= 0) { message = "No company context found."; return false; }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var tx = conn.BeginTransaction();

                const string q = @"
INSERT INTO public.print_item_columns
    (companyid, tradedocumenttypesid, column_key, header_text, is_visible, sort_order, width_percent)
VALUES (@companyid, @doctype, @colkey, @header, @visible, @sortorder, @width)
ON CONFLICT (companyid, tradedocumenttypesid, column_key) DO UPDATE SET
    header_text = EXCLUDED.header_text,
    is_visible = EXCLUDED.is_visible,
    sort_order = EXCLUDED.sort_order,
    width_percent = EXCLUDED.width_percent";

                int order = 0;
                foreach (var row in rows)
                {
                    if (row == null || string.IsNullOrWhiteSpace(row.ColumnKey)) continue;
                    var def = PrintItemColumnCatalog.Find(row.ColumnKey);
                    if (def == null) continue; // ignore unknown keys

                    using var cmd = new NpgsqlCommand(q, conn, tx);
                    cmd.Parameters.AddWithValue("companyid", companyId);
                    cmd.Parameters.AddWithValue("doctype", tradeDocumentTypesId);
                    cmd.Parameters.AddWithValue("colkey", def.Key);
                    cmd.Parameters.AddWithValue("header",
                        string.IsNullOrWhiteSpace(row.HeaderText) ? def.DefaultHeader : row.HeaderText.Trim());
                    cmd.Parameters.AddWithValue("visible", row.IsVisible);
                    cmd.Parameters.AddWithValue("sortorder", row.SortOrder > 0 ? row.SortOrder : order);
                    cmd.Parameters.AddWithValue("width", row.WidthPercent > 0 ? row.WidthPercent : def.DefaultWidth);
                    cmd.ExecuteNonQuery();
                    order++;
                }

                tx.Commit();
                _cache.Remove(ItemColsKey(companyId, tradeDocumentTypesId));
                message = "Item table layout saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        // =================================================================
        //  PDF context
        // =================================================================
        public async Task<PdfCompanyContext> GetPdfContextAsync(int tradeDocumentId, int fallbackDocumentTypeId, string printerType = "Regular")
        {
            int companyId = 0;
            int docType = fallbackDocumentTypeId;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                const string q = @"SELECT companyid, tradedocumenttypesid FROM tradedocuments WHERE id = @p_id";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_id", tradeDocumentId);
                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    companyId = r["companyid"] != DBNull.Value ? Convert.ToInt32(r["companyid"]) : 0;
                    if (r["tradedocumenttypesid"] != DBNull.Value)
                        docType = Convert.ToInt32(r["tradedocumenttypesid"]);
                }
            }
            catch
            {
                // fall through with companyId 0 - handled below
            }

            return GetPdfContextForCompany(companyId, docType, printerType);
        }

        public PdfCompanyContext GetPdfContextForCompany(int companyId, int tradeDocumentTypesId, string printerType = "Regular")
        {
            var ctx = new PdfCompanyContext
            {
                CompanyId = companyId,
                Company = GetCompany(companyId),
                Bank = GetBank(companyId),
                Settings = GetSettings(companyId, printerType),
                ItemColumns = GetItemColumns(companyId, tradeDocumentTypesId)
            };

            ctx.TransactionName = GetTransactionNames(companyId)
                .FirstOrDefault(x => x.TradeDocumentTypesId == tradeDocumentTypesId);

            return ctx;
        }

        private BusinessProfileModel GetCompany(int companyId)
        {
            var key = CompanyKey(companyId);
            if (_cache.TryGetValue(key, out BusinessProfileModel cached) && cached != null)
                return cached;

            var model = new BusinessProfileModel();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT bp.*, sts.name AS state_name, sts.code AS state_code
                                   FROM business_profiles AS bp
                                   LEFT JOIN states AS sts ON bp.state_id = sts.id
                                   WHERE bp.businessesid = @p_companyid
                                   ORDER BY bp.id
                                   LIMIT 1";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", companyId);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    model.Id = Int(r, "id");
                    model.BusinessName = Str(r, "business_name");
                    model.PhoneNumber = Str(r, "phone_number");
                    model.Gstin = Str(r, "gstin");
                    model.Email = Str(r, "email");
                    model.BusinessTypeId = Int(r, "business_type_id");
                    model.BusinessCategoryId = Int(r, "business_category_id");
                    model.StateId = Int(r, "state_id");
                    model.Pincode = Str(r, "pincode");
                    model.Address = Str(r, "address");
                    model.LogoPath = Str(r, "logo_path");
                    model.SignaturePath = Str(r, "signature_path");
                    model.statename = Str(r, "state_name");
                    model.statecode = Str(r, "state_code");
                    model.businessesid = companyId;
                }
            }
            catch
            {
                // leave the blank profile - the PDF still renders
            }

            _cache.Set(key, model, CacheTtl);
            return model;
        }

        private BankAccountModel GetBank(int companyId)
        {
            var key = BankKey(companyId);
            if (_cache.TryGetValue(key, out BankAccountModel cached) && cached != null)
                return cached;

            var model = new BankAccountModel();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string q = @"SELECT id, account_display_name, opening_balance, as_of_date,
                                          print_upi_qr, print_bank_details, account_number,
                                          ifsc_code, upi_id, bank_name, account_holder_name
                                   FROM public.extended_bank_accounts
                                   WHERE companyid = @p_companyid
                                   ORDER BY id
                                   LIMIT 1";
                using var cmd = new NpgsqlCommand(q, conn);
                cmd.Parameters.AddWithValue("p_companyid", companyId);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    model.Id = Int(r, "id");
                    model.AccountDisplayName = Str(r, "account_display_name");
                    model.OpeningBalance = r["opening_balance"] != DBNull.Value ? Convert.ToDecimal(r["opening_balance"]) : (decimal?)null;
                    model.AsOfDate = r["as_of_date"] != DBNull.Value ? Convert.ToDateTime(r["as_of_date"]) : (DateTime?)null;
                    model.PrintUPIQrCode = Bool(r, "print_upi_qr");
                    model.PrintBankDetails = Bool(r, "print_bank_details");
                    model.AccountNumber = Str(r, "account_number");
                    model.IFSCCode = Str(r, "ifsc_code");
                    model.UPIID = Str(r, "upi_id");
                    model.BankName = Str(r, "bank_name");
                    model.AccountHolderName = Str(r, "account_holder_name");
                }
            }
            catch
            {
                // no bank configured - bank block is simply skipped
            }

            _cache.Set(key, model, CacheTtl);
            return model;
        }

        // =================================================================
        //  Defaults + reader helpers
        // =================================================================
        public static readonly Dictionary<int, string> DefaultTransactionTitles = new Dictionary<int, string>
        {
            { (int)TradeDocumentTypes.PurchaseOrder,   "Purchase Order" },
            { (int)TradeDocumentTypes.SalesOrder,      "Sale Order" },
            { (int)TradeDocumentTypes.DeliveryChallan, "Delivery Challan" },
            { (int)TradeDocumentTypes.PurchaseChallan, "Purchase Bill" },
            { (int)TradeDocumentTypes.SalesChallan,    "Tax Invoice" },
            { (int)TradeDocumentTypes.DebitNote,       "Debit Note" },
            { (int)TradeDocumentTypes.CreditNote,      "Credit Note" },
            { (int)TradeDocumentTypes.Estimation,      "Estimate / Quotation" },
            { (int)TradeDocumentTypes.PaymentIn,       "Payment Receipt" },
            { (int)TradeDocumentTypes.PaymentOut,      "Payment Voucher" }
        };

        public static string DefaultTitleFor(int docType)
        {
            return DefaultTransactionTitles.TryGetValue(docType, out var title) ? title : "Invoice";
        }

        private static void AddNullable(NpgsqlCommand cmd, string name, string value)
        {
            cmd.Parameters.AddWithValue(name, string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim());
        }

        private static bool HasColumn(System.Data.IDataRecord r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static string Str(System.Data.IDataRecord r, string name)
        {
            if (!HasColumn(r, name)) return null;
            var v = r[name];
            return v == DBNull.Value ? null : v.ToString();
        }

        private static bool Bool(System.Data.IDataRecord r, string name)
        {
            if (!HasColumn(r, name)) return false;
            var v = r[name];
            return v != DBNull.Value && Convert.ToBoolean(v);
        }

        private static int Int(System.Data.IDataRecord r, string name)
        {
            if (!HasColumn(r, name)) return 0;
            var v = r[name];
            return v == DBNull.Value ? 0 : Convert.ToInt32(v);
        }

        private static decimal Dec(System.Data.IDataRecord r, string name)
        {
            if (!HasColumn(r, name)) return 0m;
            var v = r[name];
            return v == DBNull.Value ? 0m : Convert.ToDecimal(v);
        }

        private static DateTime Dt(System.Data.IDataRecord r, string name)
        {
            if (!HasColumn(r, name)) return DateTime.MinValue;
            var v = r[name];
            return v == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(v);
        }
    }
}
