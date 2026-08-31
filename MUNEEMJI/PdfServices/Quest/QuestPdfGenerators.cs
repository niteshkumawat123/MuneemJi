using MUNEEMJI.Models;
using MUNEEMJI.Services;
using QuestPDF.Fluent;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Shared pipeline for every QuestPDF-backed generator:
    /// resolve company context -> load data -> render -> persist -> return web path.
    /// Each concrete class exposes the method name its legacy interface declares,
    /// so controllers and views need no changes.
    /// </summary>
    public abstract class QuestPdfGeneratorBase
    {
        protected readonly IPrintSettingsService PrintSettings;

        protected QuestPdfGeneratorBase(IPrintSettingsService printSettings)
        {
            PrintSettings = printSettings;
        }

        /// <summary>Document type this generator prints, for settings lookup.</summary>
        protected abstract int DocumentTypeId { get; }

        /// <summary>Title used when the company has not renamed the transaction.</summary>
        protected abstract string DefaultTitle { get; }

        /// <summary>File name prefix under wwwroot/DataContainer/GeneratedInvoices.</summary>
        protected abstract string FilePrefix { get; }

        /// <summary>Renders and persists, returning the web-relative path.</summary>
        protected async Task<string> GeneratePathAsync(int id, IWebHostEnvironment env)
        {
            var bytes = await RenderAsync(id, env, "Regular");

            var folder = Path.Combine(env.WebRootPath, "DataContainer", "GeneratedInvoices");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{FilePrefix}_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var fullPath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(fullPath, bytes);

            return $"/DataContainer/GeneratedInvoices/{fileName}";
        }

        /// <summary>
        /// Renders straight to memory. Used by the Settings &gt; Print live preview,
        /// which must never leave files behind on disk.
        /// </summary>
        public async Task<byte[]> RenderAsync(int id, IWebHostEnvironment env, string printerType)
        {
            QuestPdfEngine.EnsureInitialised(env);

            var context = await PrintSettings.GetPdfContextAsync(id, DocumentTypeId, printerType);

            var loader = new QuestPdfDataLoader();
            var data = await loader.LoadAsync(id, context, DefaultTitle);

            return Render(data, env);
        }

        /// <summary>
        /// Rasterises the document one image per page. Used by the theme
        /// regression harness so a layout change can be eyeballed, and by any
        /// caller that wants a thumbnail rather than a PDF.
        /// </summary>
        public static IEnumerable<byte[]> RenderImages(QuestDocumentData data, IWebHostEnvironment env, int dpi = 110)
        {
            return SelectDocument(data, env)
                .GenerateImages(new QuestPDF.Infrastructure.ImageGenerationSettings { RasterDpi = dpi });
        }

        /// <summary>
        /// Picks the renderer for the resolved settings. A receipt is a different
        /// document from an invoice, not a narrower one, so they do not share a layout.
        /// </summary>
        public static byte[] Render(QuestDocumentData data, IWebHostEnvironment env)
        {
            return SelectDocument(data, env).GeneratePdf();
        }

        private static QuestPDF.Infrastructure.IDocument SelectDocument(QuestDocumentData data, IWebHostEnvironment env)
        {
            var settings = data?.Settings;
            var isThermal = settings != null
                && (string.Equals(settings.PrinterType, "Thermal", StringComparison.OrdinalIgnoreCase)
                    || QuestPdfEngine.IsThermal(settings.PaperSize));

            if (isThermal)
                return new QuestThermalDocument(data, env);

            var layoutKey = settings?.EffectiveLayoutKey;

            // Unseeded or unrecognised layouts keep the original generic composer
            // rather than being forced into a themed one.
            if (!QuestThemeStyle.IsKnown(layoutKey))
                return new QuestInvoiceDocument(data, env);

            switch (QuestThemeStyle.For(layoutKey).Family)
            {
                case ThemeFamily.Tally:
                    return new QuestTallyDocument(data, env);

                case ThemeFamily.GstPlain:
                case ThemeFamily.GstBoxed:
                    return new QuestGstDocument(data, env);

                case ThemeFamily.Landscape:
                    return new QuestLandscapeDocument(data, env);

                case ThemeFamily.DoubleDivine:
                    return new QuestDoubleDivineDocument(data, env);

                case ThemeFamily.FrenchElite:
                    return new QuestFrenchEliteDocument(data, env);

                default:
                    return new QuestInvoiceDocument(data, env);
            }
        }
    }

    // =====================================================================
    //  Sales
    // =====================================================================
    public class QuestSalesInvoicesPdf : QuestPdfGeneratorBase, ISalesInvoicesPdf
    {
        public QuestSalesInvoicesPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.SalesChallan;
        protected override string DefaultTitle => "Tax Invoice";
        protected override string FilePrefix => "Invoice";

        public Task<string> GetContractPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestSaleOrderPdf : QuestPdfGeneratorBase, ISaleOrderPdf
    {
        public QuestSaleOrderPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.SalesOrder;
        protected override string DefaultTitle => "Sale Order";
        protected override string FilePrefix => "SaleOrder";

        public Task<string> GetSaleOrderPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestSaleReturnPdf : QuestPdfGeneratorBase, ISaleReturnPdf
    {
        public QuestSaleReturnPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.CreditNote;
        protected override string DefaultTitle => "Sale Return";
        protected override string FilePrefix => "SaleReturn";

        public Task<string> GetSaleReturnPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestDeliveryChallanPdf : QuestPdfGeneratorBase, IDeliveryChallanPdf
    {
        public QuestDeliveryChallanPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.DeliveryChallan;
        protected override string DefaultTitle => "Delivery Challan";
        protected override string FilePrefix => "DeliveryChallan";

        public Task<string> GetDeliveryChallanPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestEstimationQuotationPdf : QuestPdfGeneratorBase, IEstimationQuotationPdf
    {
        public QuestEstimationQuotationPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.Estimation;
        protected override string DefaultTitle => "Estimate / Quotation";
        protected override string FilePrefix => "Estimation";

        public Task<string> GetEstimationPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestCreditNotePdf : QuestPdfGeneratorBase, ICreditNotePdf
    {
        public QuestCreditNotePdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.CreditNote;
        protected override string DefaultTitle => "Credit Note";
        protected override string FilePrefix => "CreditNote";

        public Task<string> GetCreditNotePdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    // =====================================================================
    //  Purchases
    // =====================================================================
    public class QuestPurchaseBillPdf : QuestPdfGeneratorBase, IPurchaseBillPdf
    {
        public QuestPurchaseBillPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.PurchaseChallan;
        protected override string DefaultTitle => "Purchase Bill";
        protected override string FilePrefix => "PurchaseBill";

        public Task<string> GetPurchaseBillPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestPurchaseOrderPdf : QuestPdfGeneratorBase, IPurchaseOrderPdf
    {
        public QuestPurchaseOrderPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.PurchaseOrder;
        protected override string DefaultTitle => "Purchase Order";
        protected override string FilePrefix => "PurchaseOrder";

        public Task<string> GetPurchaseOrderPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestPurchaseReturnPdf : QuestPdfGeneratorBase, IPurchaseReturnPdf
    {
        public QuestPurchaseReturnPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.DebitNote;
        protected override string DefaultTitle => "Purchase Return";
        protected override string FilePrefix => "PurchaseReturn";

        public Task<string> GetPurchaseReturnPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestDrNotePdf : QuestPdfGeneratorBase, IDrNotePdf
    {
        public QuestDrNotePdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.DebitNote;
        protected override string DefaultTitle => "Debit Note";
        protected override string FilePrefix => "DebitNote";

        public Task<string> GetDrNotePdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    // =====================================================================
    //  Money in / out
    // =====================================================================
    public class QuestPaymentInPdf : QuestPdfGeneratorBase, IPaymentInPdf
    {
        public QuestPaymentInPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.PaymentIn;
        protected override string DefaultTitle => "Payment Receipt";
        protected override string FilePrefix => "PaymentIn";

        public Task<string> GetPaymentInPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestPaymentOutPdf : QuestPdfGeneratorBase, IPaymentOutPdf
    {
        public QuestPaymentOutPdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => (int)TradeDocumentTypes.PaymentOut;
        protected override string DefaultTitle => "Payment Voucher";
        protected override string FilePrefix => "PaymentOut";

        public Task<string> GetPaymentOutPdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestOtherIncomePdf : QuestPdfGeneratorBase, IOtherIncomePdf
    {
        public QuestOtherIncomePdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => 0;
        protected override string DefaultTitle => "Other Income";
        protected override string FilePrefix => "OtherIncome";

        public Task<string> GetOtherIncomePdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }

    public class QuestExpensePdf : QuestPdfGeneratorBase, IExpensePdf
    {
        public QuestExpensePdf(IPrintSettingsService printSettings) : base(printSettings) { }
        protected override int DocumentTypeId => 0;
        protected override string DefaultTitle => "Expense";
        protected override string FilePrefix => "Expense";

        public Task<string> GetExpensePdfById(int id, IWebHostEnvironment _env) => GeneratePathAsync(id, _env);
    }
}
