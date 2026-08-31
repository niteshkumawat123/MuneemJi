using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Everything the themed composers share: resolved colours, the copy list,
    /// number and date formatting, and the derived tax tables. Each theme then
    /// only has to describe its own arrangement of blocks.
    /// </summary>
    public abstract class QuestDocumentBase : IDocument
    {
        protected readonly QuestDocumentData Data;
        protected readonly IWebHostEnvironment Env;
        protected readonly PrintSettingsModel S;
        protected readonly QuestThemeStyle Style;

        protected readonly Color Accent;        // headings, filled bars
        protected readonly Color AccentText;    // text drawn on Accent
        protected readonly Color Border;
        protected readonly Color Shade;         // light tint of the accent
        protected readonly Color TotalBg;
        protected readonly Color TotalText;

        protected readonly byte[] LogoBytes;
        protected readonly byte[] SignatureBytes;

        protected const float Bw = 0.7f;
        protected const float Body = 8f;
        protected const float Small = 7.2f;
        protected const float Tiny = 6.5f;

        protected QuestDocumentBase(QuestDocumentData data, IWebHostEnvironment env)
        {
            Data = data ?? new QuestDocumentData();
            Env = env;
            S = Data.Settings;
            Style = QuestThemeStyle.For(S.EffectiveLayoutKey);

            Accent = QuestPdfEngine.ParseColor(S.EffectivePrimaryColor, "#C2185B");
            AccentText = QuestPdfEngine.ParseColor(
                QuestPdfEngine.ContrastHex(S.EffectivePrimaryColor), "#FFFFFF");
            Border = QuestPdfEngine.ParseColor(S.EffectiveBorderColor, "#A9A9A9");
            TotalBg = QuestPdfEngine.ParseColor(S.EffectiveTotalRowColor, "#FFF3CD");
            TotalText = QuestPdfEngine.ParseColor(
                QuestPdfEngine.ContrastHex(S.EffectiveTotalRowColor), "#000000");
            Shade = QuestPdfEngine.ParseColor(S.EffectiveHeaderBgColor, "#BBBBBB").WithAlpha((byte)55);

            if (S.PrintLogo)
                LogoBytes = QuestPdfEngine.ReadAsset(Env, Data.Company?.LogoPath);

            if (S.PrintSignatureImage)
                SignatureBytes = QuestPdfEngine.ReadAsset(Env, Data.Company?.SignaturePath);
        }

        public abstract void Compose(IDocumentContainer container);

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"{Data.DocumentTitle} {DocumentNumber()}",
                Author = Data.Company?.BusinessName ?? "MuneemJi",
                Subject = Data.DocumentTitle,
                Creator = "MuneemJi"
            };
        }

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        // =================================================================
        //  Page scaffolding
        // =================================================================
        protected string FontFamilyName()
        {
            return string.IsNullOrWhiteSpace(S.FontFamily) ? QuestPdfEngine.DefaultFontFamily : S.FontFamily.Trim();
        }

        protected void ApplyPageChrome(PageDescriptor page)
        {
            page.Size(QuestPdfEngine.ResolvePageSize(S.PaperSize, S.Orientation));

            page.MarginLeft((float)S.MarginLeft);
            page.MarginRight((float)S.MarginRight);
            page.MarginBottom((float)S.MarginBottom);
            page.MarginTop((float)S.ExtraSpaceTop + 12f);

            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x
                .FontFamily(FontFamilyName(), QuestPdfEngine.FallbackFontFamily)
                .FontSize(Body)
                .FontColor(Colors.Black));

            if (!string.IsNullOrWhiteSpace(S.WatermarkText))
            {
                page.Foreground().AlignCenter().AlignMiddle()
                    .Text(S.WatermarkText.Trim())
                    .FontSize(60).Bold().FontColor(Accent.WithAlpha((byte)26));
            }

            page.Footer().Element(ComposePageFooter);
        }

        protected void ComposePageFooter(IContainer container)
        {
            if (!S.PrintPageNumbers)
            {
                container.Height(0);
                return;
            }

            container.PaddingTop(4).AlignRight().Text(text =>
            {
                text.DefaultTextStyle(x => x.FontSize(Tiny).FontColor(Colors.Grey.Darken1));
                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
        }

        // =================================================================
        //  Copies
        // =================================================================
        protected List<string> CopyLabels()
        {
            var labels = new List<string>();

            if (!S.PrintOriginalDuplicate)
            {
                labels.Add(null);
                return labels;
            }

            var txn = Data.Context?.TransactionName;

            if (S.PrintCopyOriginal)
                labels.Add(Pick(txn?.LabelOriginal, S.LabelOriginal, "ORIGINAL FOR RECIPIENT"));
            if (S.PrintCopyDuplicate)
                labels.Add(Pick(txn?.LabelDuplicate, S.LabelDuplicate, "DUPLICATE FOR TRANSPORTER"));
            if (S.PrintCopyTriplicate)
                labels.Add(Pick(txn?.LabelTriplicate, S.LabelTriplicate, "TRIPLICATE FOR SUPPLIER"));

            if (labels.Count == 0) labels.Add(null);
            return labels;
        }

        // =================================================================
        //  Formatting
        // =================================================================
        protected static string Pick(params string[] candidates)
        {
            foreach (var c in candidates)
                if (!string.IsNullOrWhiteSpace(c)) return c.Trim();
            return string.Empty;
        }

        protected string Money(decimal value)
        {
            return QuestPdfEngine.Rupee + " " +
                   QuestPdfEngine.Money(value, S.PrintAmountWithDecimal, S.PrintAmountWithGrouping);
        }

        protected string Plain(decimal value)
        {
            return QuestPdfEngine.Money(value, S.PrintAmountWithDecimal, S.PrintAmountWithGrouping);
        }

        protected string DocumentNumber()
        {
            var bill = Data.Bill;
            if (!string.IsNullOrWhiteSpace(bill.BillNumber)) return bill.BillNumber;
            if (bill.InvoiceNumber.HasValue && bill.InvoiceNumber.Value > 0) return bill.InvoiceNumber.Value.ToString();
            if (!string.IsNullOrWhiteSpace(bill.OrderNo)) return bill.OrderNo;
            if (!string.IsNullOrWhiteSpace(bill.ChallanNo)) return bill.ChallanNo;
            return bill.Id > 0 ? bill.Id.ToString() : "-";
        }

        protected string NumberLabel()
        {
            var title = Data.DocumentTitle ?? string.Empty;
            if (title.IndexOf("Invoice", StringComparison.OrdinalIgnoreCase) >= 0) return "Invoice No.";
            if (title.IndexOf("Challan", StringComparison.OrdinalIgnoreCase) >= 0) return "Challan No.";
            if (title.IndexOf("Order", StringComparison.OrdinalIgnoreCase) >= 0) return "Order No.";
            if (title.IndexOf("Receipt", StringComparison.OrdinalIgnoreCase) >= 0) return "Receipt No.";
            if (title.IndexOf("Voucher", StringComparison.OrdinalIgnoreCase) >= 0) return "Voucher No.";
            if (title.IndexOf("Note", StringComparison.OrdinalIgnoreCase) >= 0) return "Note No.";
            if (title.IndexOf("Estimate", StringComparison.OrdinalIgnoreCase) >= 0) return "Estimate No.";
            return "No.";
        }

        protected string BuildCompanyAddress()
        {
            var c = Data.Company;
            if (c == null) return string.Empty;

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.Address)) parts.Add(c.Address.Trim());
            if (!string.IsNullOrWhiteSpace(c.statename)) parts.Add(c.statename.Trim());
            if (!string.IsNullOrWhiteSpace(c.Pincode)) parts.Add(c.Pincode.Trim());
            return string.Join(" , ", parts);
        }

        protected string BuildStateLine()
        {
            var c = Data.Company;
            if (c == null || string.IsNullOrWhiteSpace(c.statename)) return null;
            return string.IsNullOrWhiteSpace(c.statecode) ? c.statename : c.statecode + "-" + c.statename;
        }

        protected string ShipAddress()
        {
            return Pick(Data.Bill.ShippingAddress, Data.Party?.ShippingAddress, Data.Party?.BillingAddress);
        }

        protected string PartyName()
        {
            return Pick(Data.Bill.BillingName, Data.Party?.PartyName, "-");
        }

        protected string BillAddress()
        {
            return Pick(Data.Bill.BillingAddress, Data.Party?.BillingAddress);
        }

        protected string PartyPhone()
        {
            return Pick(Data.Party?.PhoneNumber, Data.Bill.PhoneNo);
        }

        protected string DocumentDate()
        {
            var bill = Data.Bill;
            return QuestPdfEngine.DateOrDash(bill.InvoiceDate != DateTime.MinValue ? bill.InvoiceDate : bill.BillDate);
        }

        // =================================================================
        //  Rates
        // =================================================================
        protected static string PercentSuffix(decimal percent)
        {
            return percent == 0m ? string.Empty : " (" + QuestPdfEngine.Percent(percent) + ")";
        }

        protected string TcsTdsLabel() => Data.Bill.TCSTDSType.ToString();

        /// <summary>
        /// Header-level rates are often left at 0 while the real rate lives on the
        /// lines, so fall back to the blended rate actually charged.
        /// </summary>
        protected decimal EffectiveTaxRate()
        {
            if (Data.Bill.TaxPercentage != 0m) return Data.Bill.TaxPercentage;
            if (Data.TotalTaxable == 0m) return 0m;

            var distinct = Data.Items.Where(i => i.TaxPercentage != 0m)
                                     .Select(i => i.TaxPercentage).Distinct().ToList();
            if (distinct.Count == 1) return distinct[0];

            return Math.Round(Data.TotalTax * 100m / Data.TotalTaxable, 2);
        }

        protected decimal EffectiveDiscountRate()
        {
            if (Data.Bill.DiscountPercent != 0m) return Data.Bill.DiscountPercent;

            var gross = Data.TotalTaxable + Data.TotalDiscount;
            if (gross == 0m || Data.TotalDiscount == 0m) return 0m;

            return Math.Round(Data.TotalDiscount * 100m / gross, 2);
        }

        // =================================================================
        //  Derived tax tables
        // =================================================================
        protected class HsnGroup
        {
            public string Hsn { get; set; }
            public decimal Rate { get; set; }
            public decimal Taxable { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal Cess { get; set; }
        }

        /// <summary>Grouped by HSN code and rate, as the HSN summary grid needs.</summary>
        protected List<HsnGroup> HsnGroups()
        {
            var map = new Dictionary<string, HsnGroup>();

            foreach (var item in Data.Items)
            {
                var hsn = string.IsNullOrWhiteSpace(item.HSNCode) ? "-" : item.HSNCode.Trim();
                var key = hsn + "|" + item.TaxPercentage;

                if (!map.TryGetValue(key, out var group))
                {
                    group = new HsnGroup { Hsn = hsn, Rate = item.TaxPercentage };
                    map[key] = group;
                }

                group.Taxable += item.Quantity * item.PricePerUnit - item.DiscountAmount;
                group.TaxAmount += item.TaxAmount;
                group.Cess += item.AddCessAmount ?? 0m;
            }

            return map.Values.OrderBy(x => x.Hsn).ThenBy(x => x.Rate).ToList();
        }

        protected class TaxTypeRow
        {
            public string Type { get; set; }        // CGST / SGST / IGST
            public decimal Taxable { get; set; }
            public decimal Rate { get; set; }       // half rate for CGST/SGST
            public decimal Amount { get; set; }
        }

        /// <summary>
        /// One row per tax component per rate, which is what the
        /// "Tax type | Taxable amount | Rate | Tax amount" block prints.
        /// </summary>
        protected List<TaxTypeRow> TaxTypeRows()
        {
            var rows = new List<TaxTypeRow>();

            foreach (var group in HsnGroups().GroupBy(g => g.Rate))
            {
                var taxable = group.Sum(g => g.Taxable);
                var amount = group.Sum(g => g.TaxAmount);
                var rate = group.Key;

                if (Data.IsDomestic)
                {
                    rows.Add(new TaxTypeRow { Type = "SGST", Taxable = taxable, Rate = rate / 2m, Amount = amount / 2m });
                    rows.Add(new TaxTypeRow { Type = "CGST", Taxable = taxable, Rate = rate / 2m, Amount = amount / 2m });
                }
                else
                {
                    rows.Add(new TaxTypeRow { Type = "IGST", Taxable = taxable, Rate = rate, Amount = amount });
                }
            }

            return rows;
        }

        // =================================================================
        //  Cell helpers
        // =================================================================
        protected static IContainer Align(IContainer container, string align)
        {
            switch ((align ?? "left").ToLowerInvariant())
            {
                case "right": return container.AlignRight();
                case "center": return container.AlignCenter();
                default: return container.AlignLeft();
            }
        }

        /// <summary>Filled caption bar in the accent colour.</summary>
        protected IContainer BarCell(IContainer c) =>
            c.Background(Accent).PaddingVertical(3).PaddingHorizontal(5);

        /// <summary>Light tinted caption cell.</summary>
        protected IContainer ShadeCell(IContainer c) =>
            c.Background(Shade).PaddingVertical(3).PaddingHorizontal(5);

        protected IContainer PadCell(IContainer c) =>
            c.PaddingVertical(3).PaddingHorizontal(5);

        /// <summary>Caption that respects the theme's filled-bar switch.</summary>
        protected void Caption(ColumnDescriptor col, string text, bool filled)
        {
            if (filled)
                col.Item().Element(BarCell).Text(text).FontSize(Small).Bold().FontColor(AccentText);
            else
                col.Item().Element(PadCell).Text(text).FontSize(Small).Bold().FontColor(Accent);
        }

        protected void CaptionCell(IContainer container, string text, bool filled)
        {
            if (filled)
                container.Element(BarCell).Text(text).FontSize(Small).Bold().FontColor(AccentText);
            else
                container.Element(PadCell).Text(text).FontSize(Small).Bold().FontColor(Accent);
        }

        protected void Rule(ColumnDescriptor col, bool accent)
        {
            col.Item().PaddingVertical(4)
                .LineHorizontal(accent ? 0.9f : 0.6f)
                .LineColor(accent ? Accent : Border);
        }

        // =================================================================
        //  Shared blocks used by more than one family
        // =================================================================

        /// <summary>Bank details block - QR placeholder plus the account lines.</summary>
        protected bool HasBank()
        {
            var b = Data.Context?.Bank;
            return S.PrintBankDetails && b != null &&
                   (!string.IsNullOrWhiteSpace(b.BankName)
                    || !string.IsNullOrWhiteSpace(b.AccountNumber)
                    || !string.IsNullOrWhiteSpace(b.IFSCCode));
        }

        protected void ComposeBankLines(ColumnDescriptor c)
        {
            var b = Data.Context?.Bank;
            if (b == null) return;

            if (!string.IsNullOrWhiteSpace(b.BankName))
                c.Item().Text("Bank Name: " + b.BankName).FontSize(Small);

            if (!string.IsNullOrWhiteSpace(b.AccountNumber))
                c.Item().PaddingTop(3).Text("Bank Account No.: " + b.AccountNumber).FontSize(Small);

            if (!string.IsNullOrWhiteSpace(b.IFSCCode))
                c.Item().PaddingTop(3).Text("Bank IFSC code: " + b.IFSCCode).FontSize(Small);

            if (S.PrintUpiQr && !string.IsNullOrWhiteSpace(b.UPIID))
                c.Item().PaddingTop(3).Text("UPI: " + b.UPIID).FontSize(Small);
        }

        /// <summary>"For : COMPANY" plus signature image and caption.</summary>
        protected void ComposeSignature(ColumnDescriptor c)
        {
            var company = Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company");

            c.Item().AlignCenter().Text("For : " + company).FontSize(Small);

            if (SignatureBytes != null)
                c.Item().PaddingTop(3).AlignCenter().Height(44f).Image(SignatureBytes).FitArea();
            else
                c.Item().Height(30f);

            if (S.PrintSignatureText)
            {
                c.Item().PaddingTop(3).AlignCenter()
                    .Text(Pick(S.SignatureText, "Authorized Signatory")).FontSize(Small).Bold();
            }
        }

        /// <summary>The label : value rows on the right of every theme.</summary>
        protected void ComposeAmountRows(ColumnDescriptor col, Action<ColumnDescriptor, string, string, bool> line)
        {
            var bill = Data.Bill;

            line(col, "Sub Total", Money(Data.TotalTaxable + Data.TotalDiscount), false);
            line(col, "Discount" + PercentSuffix(EffectiveDiscountRate()), Money(Data.TotalDiscount), false);

            if (S.PrintTaxDetails)
                line(col, "Tax" + PercentSuffix(EffectiveTaxRate()), Money(Data.TotalTax), false);

            line(col, TcsTdsLabel() + PercentSuffix(bill.TdsTcsPercentage), Money(bill.TdsTcsAmount), false);

            if (Data.TotalCess != 0) line(col, "Cess", Money(Data.TotalCess), false);
            if (bill.ShippingAmount != 0) line(col, "Shipping", Money(bill.ShippingAmount), false);
            if (bill.PackingAmount != 0) line(col, "Packing", Money(bill.PackingAmount), false);
            if (bill.AdjustmentAmount != 0) line(col, "Adjustment", Money(bill.AdjustmentAmount), false);
            if (bill.IsRoundOff && bill.RoundOffValue != 0) line(col, "Round Off", Money(bill.RoundOffValue), false);

            line(col, "Total", Money(Data.GrandTotal), true);

            if (S.PrintReceivedAmount) line(col, "Received", Money(bill.paidReciveamount), false);
            if (S.PrintBalanceAmount) line(col, "Balance", Money(Data.GrandTotal - bill.paidReciveamount), false);
            if (S.PrintCurrentBalanceParty) line(col, "Current Balance", Money(Data.PartyCurrentBalance), false);
            if (S.PrintYouSaved && Data.TotalDiscount > 0) line(col, "You Saved", Money(Data.TotalDiscount), true);
        }
    }
}
