using MUNEEMJI.Models.Setting;

namespace MUNEEMJI.Services
{
    public interface IPrintSettingsService
    {
        /// <summary>Theme catalogue (public.print_themes), cached.</summary>
        List<PrintThemeModel> GetThemes();

        /// <summary>Themes for one tab ("Regular" / "Thermal").</summary>
        List<PrintThemeModel> GetThemes(string printerType);

        /// <summary>
        /// Settings for one company + printer type. Creates the row with defaults
        /// on first access so the caller never has to null-check.
        /// </summary>
        PrintSettingsModel GetSettings(int companyId, string printerType);

        /// <summary>Everything the Settings/Print screen needs.</summary>
        PrintSettingsViewModel GetViewModel(int companyId);

        /// <summary>Upsert one settings row and invalidate the cache.</summary>
        bool SaveSettings(PrintSettingsModel model, string updatedBy, out string message);

        /// <summary>Copy the theme colours + orientation onto the settings row.</summary>
        bool ResetToTheme(int companyId, string printerType, int themeId, string updatedBy, out string message);

        /// <summary>Per-document-type titles and copy labels.</summary>
        List<PrintTransactionNameModel> GetTransactionNames(int companyId);

        bool SaveTransactionNames(int companyId, List<PrintTransactionNameModel> rows, out string message);

        /// <summary>
        /// Item table columns for a document type. Falls back to the type 0
        /// ("all types") row set, then to PrintItemColumnCatalog defaults.
        /// </summary>
        List<PrintItemColumnModel> GetItemColumns(int companyId, int tradeDocumentTypesId);

        bool SaveItemColumns(int companyId, int tradeDocumentTypesId, List<PrintItemColumnModel> rows, out string message);

        /// <summary>
        /// Resolves the issuing company, its bank, print settings, transaction
        /// name and item columns for a document. Company comes from the document
        /// row itself, so tenants never print each other letterheads.
        /// </summary>
        Task<PdfCompanyContext> GetPdfContextAsync(int tradeDocumentId, int fallbackDocumentTypeId, string printerType = "Regular");

        /// <summary>Same as above but for a known company (used by the live preview).</summary>
        PdfCompanyContext GetPdfContextForCompany(int companyId, int tradeDocumentTypesId, string printerType = "Regular");

        /// <summary>Drop every cached entry for a company.</summary>
        void InvalidateCompany(int companyId);
    }
}
