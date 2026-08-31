using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// The Tally theme layout.
    /// Order on the page: copy caption, centred title, then one bordered block
    /// holding company header, Bill To / Invoice Details, Ship To, item grid,
    /// tax summary beside the totals, payment mode, description beside terms,
    /// and finally bank details beside the signature.
    /// </summary>
    public class QuestTallyDocument : IDocument
    {
        private readonly QuestDocumentData _data;
        private readonly IWebHostEnvironment _env;
        private readonly PrintSettingsModel _s;

        private readonly Color _primary;
        private readonly Color _border;
        private readonly Color _shade;
        private readonly Color _totalRowBg;
        private readonly Color _totalRowText;

        private readonly byte[] _logoBytes;
        private readonly byte[] _signatureBytes;

        private const float Bw = 0.7f;      // border width
        private const float Body = 8f;      // body text size
        private const float Small = 7.2f;   // dense table text

        public QuestTallyDocument(QuestDocumentData data, IWebHostEnvironment env)
        {
            _data = data ?? new QuestDocumentData();
            _env = env;
            _s = _data.Settings;

            _primary = QuestPdfEngine.ParseColor(_s.EffectivePrimaryColor, "#4E2A0A");
            _border = QuestPdfEngine.ParseColor(_s.EffectiveBorderColor, "#A9A9A9");
            _totalRowBg = QuestPdfEngine.ParseColor(_s.EffectiveTotalRowColor, "#FFF3CD");
            _totalRowText = QuestPdfEngine.ParseColor(
                QuestPdfEngine.ContrastHex(_s.EffectiveTotalRowColor), "#000000");

            // Section captions ("Bill To:", "Ship To:", ...) sit on a light tint of
            // the theme header colour so they follow whatever accent is picked.
            _shade = QuestPdfEngine.ParseColor(_s.EffectiveHeaderBgColor, "#BBBBBB").WithAlpha((byte)55);

            if (_s.PrintLogo)
                _logoBytes = QuestPdfEngine.ReadAsset(_env, _data.Company?.LogoPath);

            if (_s.PrintSignatureImage)
                _signatureBytes = QuestPdfEngine.ReadAsset(_env, _data.Company?.SignaturePath);
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"{_data.DocumentTitle} {DocumentNumber()}",
                Author = _data.Company?.BusinessName ?? "MuneemJi",
                Subject = _data.DocumentTitle,
                Creator = "MuneemJi"
            };
        }

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        // =================================================================
        //  Copies
        // =================================================================
        private List<string> CopyLabels()
        {
            var labels = new List<string>();

            if (!_s.PrintOriginalDuplicate)
            {
                labels.Add(null);
                return labels;
            }

            var txn = _data.Context?.TransactionName;

            if (_s.PrintCopyOriginal)
                labels.Add(Pick(txn?.LabelOriginal, _s.LabelOriginal, "ORIGINAL FOR RECIPIENT"));
            if (_s.PrintCopyDuplicate)
                labels.Add(Pick(txn?.LabelDuplicate, _s.LabelDuplicate, "DUPLICATE FOR TRANSPORTER"));
            if (_s.PrintCopyTriplicate)
                labels.Add(Pick(txn?.LabelTriplicate, _s.LabelTriplicate, "TRIPLICATE FOR SUPPLIER"));

            if (labels.Count == 0) labels.Add(null);
            return labels;
        }

        public void Compose(IDocumentContainer container)
        {
            foreach (var copyLabel in CopyLabels())
                ComposePage(container, copyLabel);
        }

        private void ComposePage(IDocumentContainer container, string copyLabel)
        {
            container.Page(page =>
            {
                page.Size(QuestPdfEngine.ResolvePageSize(_s.PaperSize, _s.Orientation));

                page.MarginLeft((float)_s.MarginLeft);
                page.MarginRight((float)_s.MarginRight);
                page.MarginBottom((float)_s.MarginBottom);
                page.MarginTop((float)_s.ExtraSpaceTop + 12f);

                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x
                    .FontFamily(FontFamilyName(), QuestPdfEngine.FallbackFontFamily)
                    .FontSize(Body)
                    .FontColor(Colors.Black));

                if (!string.IsNullOrWhiteSpace(_s.WatermarkText))
                {
                    page.Foreground().AlignCenter().AlignMiddle()
                        .Text(_s.WatermarkText.Trim())
                        .FontSize(60).Bold().FontColor(_primary.WithAlpha((byte)26));
                }

                page.Content().Column(col =>
                {
                    // Copy caption and title sit outside the bordered block.
                    if (!string.IsNullOrWhiteSpace(copyLabel))
                    {
                        col.Item().AlignRight().PaddingBottom(4)
                            .Text(copyLabel).FontSize(8f).Bold().FontColor(_primary);
                    }

                    col.Item().AlignCenter().PaddingBottom(5)
                        .Text(_data.DocumentTitle)
                        .FontSize(QuestPdfEngine.TitleSize(_s.InvoiceTextSize)).Bold();

                    col.Item().Border(Bw).BorderColor(_border).Column(box =>
                    {
                        box.Item().BorderBottom(Bw).BorderColor(_border).Element(ComposeCompanyBlock);

                        ComposeBillAndInvoice(box);
                        ComposeShipTo(box);

                        box.Item().Element(ComposeItemTable);
                        box.Item().Element(ComposeSummaryRow);

                        ComposePaymentMode(box);
                        ComposeDescriptionAndTerms(box);
                        ComposeBankAndSignature(box);
                    });
                });

                page.Footer().Element(ComposePageFooter);
            });
        }

        private string FontFamilyName()
        {
            return string.IsNullOrWhiteSpace(_s.FontFamily) ? QuestPdfEngine.DefaultFontFamily : _s.FontFamily.Trim();
        }

        // =================================================================
        //  Shared cell helpers
        // =================================================================
        private IContainer Caption(IContainer c) => c.Background(_shade).PaddingVertical(3).PaddingHorizontal(5);
        private IContainer Cell(IContainer c) => c.PaddingVertical(3).PaddingHorizontal(5);

        private static string Pick(params string[] candidates)
        {
            foreach (var c in candidates)
                if (!string.IsNullOrWhiteSpace(c)) return c.Trim();
            return string.Empty;
        }

        private string Money(decimal value)
        {
            return QuestPdfEngine.Rupee + " " +
                   QuestPdfEngine.Money(value, _s.PrintAmountWithDecimal, _s.PrintAmountWithGrouping);
        }

        private string Plain(decimal value)
        {
            return QuestPdfEngine.Money(value, _s.PrintAmountWithDecimal, _s.PrintAmountWithGrouping);
        }

        private string DocumentNumber()
        {
            var bill = _data.Bill;
            if (!string.IsNullOrWhiteSpace(bill.BillNumber)) return bill.BillNumber;
            if (bill.InvoiceNumber.HasValue && bill.InvoiceNumber.Value > 0) return bill.InvoiceNumber.Value.ToString();
            if (!string.IsNullOrWhiteSpace(bill.OrderNo)) return bill.OrderNo;
            if (!string.IsNullOrWhiteSpace(bill.ChallanNo)) return bill.ChallanNo;
            return bill.Id > 0 ? bill.Id.ToString() : "-";
        }

        // =================================================================
        //  1. Company header - logo left, details centred
        // =================================================================
        private void ComposeCompanyBlock(IContainer container)
        {
            container.Padding(5).Row(row =>
            {
                if (_logoBytes != null)
                {
                    row.ConstantItem(62f).AlignMiddle().AlignCenter()
                        .Height(46f).Image(_logoBytes).FitArea();
                }

                row.RelativeItem().Column(info =>
                {
                    if (_s.PrintCompanyName)
                    {
                        info.Item().AlignCenter()
                            .Text(Pick(_s.CompanyNameText, _data.Company?.BusinessName, "Company Name"))
                            .FontSize(QuestPdfEngine.CompanyNameSize(_s.CompanyNameTextSize)).Bold();
                    }

                    if (_s.PrintAddress)
                    {
                        var address = Pick(_s.AddressText, BuildCompanyAddress());
                        if (!string.IsNullOrWhiteSpace(address))
                            info.Item().AlignCenter().PaddingTop(1).Text(address).FontSize(Small).Bold();
                    }

                    // Phone | GSTIN, then Email | State - two balanced columns.
                    var phone = _s.PrintPhone ? Pick(_s.PhoneText, _data.Company?.PhoneNumber) : null;
                    var gstin = _s.PrintGstin ? Pick(_s.GstinText, _data.Company?.Gstin) : null;
                    var email = _s.PrintEmail ? Pick(_s.EmailText, _data.Company?.Email) : null;
                    var state = _s.PrintState ? BuildStateLine() : null;

                    if (!string.IsNullOrWhiteSpace(phone) || !string.IsNullOrWhiteSpace(gstin))
                        PairLine(info, string.IsNullOrWhiteSpace(phone) ? "" : "Phone: " + phone,
                                       string.IsNullOrWhiteSpace(gstin) ? "" : "GSTIN : " + gstin);

                    if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(state))
                        PairLine(info, string.IsNullOrWhiteSpace(email) ? "" : "Email: " + email,
                                       string.IsNullOrWhiteSpace(state) ? "" : "State : " + state);
                });

                // Balances the logo column so the details stay optically centred.
                if (_logoBytes != null)
                    row.ConstantItem(62f);
            });
        }

        private void PairLine(ColumnDescriptor col, string left, string right)
        {
            col.Item().PaddingTop(2).Row(r =>
            {
                r.RelativeItem().AlignRight().PaddingRight(10).Text(left).FontSize(Small).Bold();
                r.RelativeItem().AlignLeft().PaddingLeft(10).Text(right).FontSize(Small).Bold();
            });
        }

        private string BuildCompanyAddress()
        {
            var c = _data.Company;
            if (c == null) return string.Empty;

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.Address)) parts.Add(c.Address.Trim());
            if (!string.IsNullOrWhiteSpace(c.statename)) parts.Add(c.statename.Trim());
            if (!string.IsNullOrWhiteSpace(c.Pincode)) parts.Add(c.Pincode.Trim());
            return string.Join(" , ", parts);
        }

        private string BuildStateLine()
        {
            var c = _data.Company;
            if (c == null || string.IsNullOrWhiteSpace(c.statename)) return null;
            return string.IsNullOrWhiteSpace(c.statecode)
                ? c.statename
                : c.statecode + "-" + c.statename;
        }

        // =================================================================
        //  2. Bill To | Invoice Details
        // =================================================================
        private void ComposeBillAndInvoice(ColumnDescriptor box)
        {
            var bill = _data.Bill;
            var party = _data.Party;

            box.Item().BorderBottom(Bw).BorderColor(_border).Row(row =>
            {
                row.RelativeItem().Element(Caption).Text("Bill To:").FontSize(Small).Bold();
                row.RelativeItem().BorderLeft(Bw).BorderColor(_border)
                    .Element(Caption).Text("Invoice Details:").FontSize(Small).Bold();
            });

            box.Item().BorderBottom(Bw).BorderColor(_border).Row(row =>
            {
                row.RelativeItem().Element(Cell).Column(c =>
                {
                    c.Item().Text(Pick(bill.BillingName, party?.PartyName, "-")).FontSize(Small);

                    var address = Pick(bill.BillingAddress, party?.BillingAddress);
                    if (!string.IsNullOrWhiteSpace(address))
                        c.Item().PaddingTop(2).Text(address).FontSize(Small);

                    var phone = Pick(party?.PhoneNumber, bill.PhoneNo);
                    if (!string.IsNullOrWhiteSpace(phone))
                        c.Item().PaddingTop(2).Text("Contact No.: " + phone).FontSize(Small);

                    if (!string.IsNullOrWhiteSpace(party?.GSTIN))
                        c.Item().PaddingTop(2).Text("GSTIN: " + party.GSTIN).FontSize(Small);
                });

                row.RelativeItem().BorderLeft(Bw).BorderColor(_border).Element(Cell).Column(c =>
                {
                    c.Item().Text(NumberLabel() + ": " + DocumentNumber()).FontSize(Small);

                    c.Item().PaddingTop(2).Text("Date: " + QuestPdfEngine.DateOrDash(
                        bill.InvoiceDate != DateTime.MinValue ? bill.InvoiceDate : bill.BillDate)).FontSize(Small);

                    if (bill.Time.HasValue && bill.Time.Value != TimeSpan.MinValue)
                        c.Item().PaddingTop(2).Text("Time: " + QuestPdfEngine.TimeOrDash(bill.Time)).FontSize(Small);

                    if (bill.DueDate != DateTime.MinValue)
                        c.Item().PaddingTop(2).Text("Due Date: " + QuestPdfEngine.DateOrDash(bill.DueDate)).FontSize(Small);

                    if (!string.IsNullOrWhiteSpace(bill.PONo))
                        c.Item().PaddingTop(2).Text("PO No.: " + bill.PONo).FontSize(Small);
                });
            });
        }

        private string NumberLabel()
        {
            var title = _data.DocumentTitle ?? string.Empty;
            if (title.IndexOf("Invoice", StringComparison.OrdinalIgnoreCase) >= 0) return "Invoice No.";
            if (title.IndexOf("Challan", StringComparison.OrdinalIgnoreCase) >= 0) return "Challan No.";
            if (title.IndexOf("Order", StringComparison.OrdinalIgnoreCase) >= 0) return "Order No.";
            if (title.IndexOf("Receipt", StringComparison.OrdinalIgnoreCase) >= 0) return "Receipt No.";
            if (title.IndexOf("Voucher", StringComparison.OrdinalIgnoreCase) >= 0) return "Voucher No.";
            if (title.IndexOf("Note", StringComparison.OrdinalIgnoreCase) >= 0) return "Note No.";
            if (title.IndexOf("Estimate", StringComparison.OrdinalIgnoreCase) >= 0) return "Estimate No.";
            return "No.";
        }

        // =================================================================
        //  3. Ship To
        // =================================================================
        private void ComposeShipTo(ColumnDescriptor box)
        {
            var shipAddress = Pick(_data.Bill.ShippingAddress, _data.Party?.ShippingAddress);
            if (string.IsNullOrWhiteSpace(shipAddress)) return;

            box.Item().BorderBottom(Bw).BorderColor(_border)
                .Element(Caption).Text("Ship To:").FontSize(Small).Bold();

            box.Item().BorderBottom(Bw).BorderColor(_border)
                .Element(Cell).Text(shipAddress).FontSize(Small);
        }

        // =================================================================
        //  4. Item grid
        // =================================================================
        private void ComposeItemTable(IContainer container)
        {
            var items = _data.Items;

            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3.5f);    // #
                    c.RelativeColumn(22f);     // Item name
                    c.RelativeColumn(10f);     // HSC/SAC
                    c.RelativeColumn(10f);     // Quantity
                    c.RelativeColumn(11f);     // Price/unit
                    c.RelativeColumn(13f);     // Discount
                    c.RelativeColumn(13f);     // GST
                    c.RelativeColumn(12.5f);   // Amount
                });

                table.Header(h =>
                {
                    ItemHead(h, "#", "left");
                    ItemHead(h, "Item name", "left");
                    ItemHead(h, "HSC/SAC", "right");
                    ItemHead(h, "Quantity", "right");
                    ItemHead(h, "Price/unit", "right");
                    ItemHead(h, "Discount", "right");
                    ItemHead(h, "GST", "right");
                    ItemHead(h, "Amount", "right");
                });

                int index = 0;
                foreach (var item in items)
                {
                    index++;

                    var free = item.FreeQuantity ?? 0m;
                    var qty = QuestPdfEngine.Qty(item.Quantity)
                              + (free > 0 ? "+" + QuestPdfEngine.Qty(free) : string.Empty);

                    var lineGross = item.Quantity * item.PricePerUnit;
                    var lineTaxable = lineGross - item.DiscountAmount;

                    ItemCell(table, index.ToString(), "left", _primary);
                    ItemCell(table, QuestPdfEngine.Dash(item.Item), "left", null, bold: true);
                    ItemCell(table, QuestPdfEngine.Dash(item.HSNCode), "right");
                    ItemCell(table, qty, "right", _primary);
                    ItemCell(table, Money(item.PricePerUnit), "right");
                    ItemCell(table, Money(item.DiscountAmount) + " (" + QuestPdfEngine.Percent(item.DiscountPercentage) + ")", "right");
                    ItemCell(table, Money(item.TaxAmount) + " (" + QuestPdfEngine.Percent(item.TaxPercentage) + ")", "right");
                    ItemCell(table, Money(item.TotalAmount ?? (lineTaxable + item.TaxAmount)), "right");
                }

                // "Min No. of Rows" - keep the grid a fixed height on short bills.
                for (int blank = items.Count; blank < _s.MinItemRows; blank++)
                    for (int c = 0; c < 8; c++)
                        table.Cell().Border(Bw).BorderColor(_border).MinHeight(13f).Padding(3).Text(string.Empty);

                table.Footer(f =>
                {
                    var totalFree = _data.Items.Sum(i => i.FreeQuantity ?? 0m);
                    var totalQty = QuestPdfEngine.Qty(_data.TotalQuantity)
                                   + (totalFree > 0 ? " + " + QuestPdfEngine.Qty(totalFree) : string.Empty);

                    ItemFoot(f, string.Empty, "left");
                    ItemFoot(f, "TOTAL", "left");
                    ItemFoot(f, string.Empty, "right");
                    ItemFoot(f, _s.PrintTotalItemQuantity ? totalQty : string.Empty, "right");
                    ItemFoot(f, string.Empty, "right");
                    ItemFoot(f, Money(_data.TotalDiscount), "right");
                    ItemFoot(f, Money(_data.TotalTax), "right");
                    ItemFoot(f, Money(_data.TotalTaxable + _data.TotalTax + _data.TotalCess), "right");
                });

                if (_s.ExpandItemTable)
                    table.ExtendLastCellsToTableBottom();
            });
        }

        private void ItemHead(TableCellDescriptor h, string text, string align)
        {
            h.Cell().Border(Bw).BorderColor(_border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold();
        }

        private void ItemCell(TableDescriptor t, string text, string align, Color? color = null, bool bold = false)
        {
            var span = t.Cell().Border(Bw).BorderColor(_border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small);

            if (bold) span.Bold();
            if (color.HasValue) span.FontColor(color.Value);
        }

        private void ItemFoot(TableCellDescriptor f, string text, string align)
        {
            f.Cell().Border(Bw).BorderColor(_border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold();
        }

        private static IContainer Align(IContainer container, string align)
        {
            switch ((align ?? "left").ToLowerInvariant())
            {
                case "right": return container.AlignRight();
                case "center": return container.AlignCenter();
                default: return container.AlignLeft();
            }
        }

        // =================================================================
        //  5. Tax summary (left) beside totals (right)
        // =================================================================
        private void ComposeSummaryRow(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem(1.15f).Column(left =>
                {
                    if (_s.PrintTaxDetails)
                    {
                        left.Item().BorderRight(Bw).BorderBottom(Bw).BorderColor(_border)
                            .Element(Cell).Text("Tax Summary:").FontSize(Small);

                        left.Item().BorderRight(Bw).BorderColor(_border).Element(ComposeTaxSummaryTable);
                    }

                    // No Extend() here: inside a page column it would swallow the
                    // rest of the page and push the footer blocks onto page 2.
                    left.Item().BorderRight(Bw).BorderColor(_border);
                });

                row.RelativeItem(1f).Element(ComposeTotalsColumn);
            });
        }

        private void ComposeTaxSummaryTable(IContainer container)
        {
            bool split = _data.IsDomestic;

            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(11f);   // HSN/SAC
                    c.RelativeColumn(15f);   // Taxable amount
                    c.RelativeColumn(8f);    // rate
                    c.RelativeColumn(11f);   // amount
                    if (split)
                    {
                        c.RelativeColumn(8f);
                        c.RelativeColumn(11f);
                    }
                    c.RelativeColumn(15f);   // total tax
                });

                var currency = " (" + QuestPdfEngine.Rupee + ")";

                table.Header(h =>
                {
                    TaxHead(h, "HSN/ SAC", rowSpan: 2);
                    TaxHead(h, "Taxable amount" + currency, rowSpan: 2);

                    if (split)
                    {
                        TaxHead(h, "CGST", colSpan: 2);
                        TaxHead(h, "SGST", colSpan: 2);
                    }
                    else
                    {
                        TaxHead(h, "IGST", colSpan: 2);
                    }

                    TaxHead(h, "Total Tax Amount" + currency, rowSpan: 2);

                    TaxHead(h, "Rate(%)");
                    TaxHead(h, "Amount" + currency);
                    if (split)
                    {
                        TaxHead(h, "Rate(%)");
                        TaxHead(h, "Amount" + currency);
                    }
                });

                foreach (var group in HsnGroups())
                {
                    TaxCell(table, group.Hsn, "left");
                    TaxCell(table, Money(group.Taxable), "right");

                    if (split)
                    {
                        TaxCell(table, QuestPdfEngine.Percent(group.Rate / 2m), "right");
                        TaxCell(table, Money(group.TaxAmount / 2m), "right");
                        TaxCell(table, QuestPdfEngine.Percent(group.Rate / 2m), "right");
                        TaxCell(table, Money(group.TaxAmount / 2m), "right");
                    }
                    else
                    {
                        TaxCell(table, QuestPdfEngine.Percent(group.Rate), "right");
                        TaxCell(table, Money(group.TaxAmount), "right");
                    }

                    TaxCell(table, Money(group.TaxAmount + group.Cess), "right");
                }

                table.Footer(f =>
                {
                    TaxFoot(f, "Total", "left");
                    TaxFoot(f, Money(_data.TotalTaxable), "right");

                    if (split)
                    {
                        TaxFoot(f, string.Empty, "right");
                        TaxFoot(f, Money(_data.TotalTax / 2m), "right");
                        TaxFoot(f, string.Empty, "right");
                        TaxFoot(f, Money(_data.TotalTax / 2m), "right");
                    }
                    else
                    {
                        TaxFoot(f, string.Empty, "right");
                        TaxFoot(f, Money(_data.TotalTax), "right");
                    }

                    TaxFoot(f, Money(_data.TotalTax + _data.TotalCess), "right");
                });
            });
        }

        private class HsnGroup
        {
            public string Hsn { get; set; }
            public decimal Rate { get; set; }
            public decimal Taxable { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal Cess { get; set; }
        }

        /// <summary>Tax summary is grouped by HSN code and rate, as Tally prints it.</summary>
        private List<HsnGroup> HsnGroups()
        {
            var map = new Dictionary<string, HsnGroup>();

            foreach (var item in _data.Items)
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

        private void TaxHead(TableCellDescriptor h, string text, uint rowSpan = 1, uint colSpan = 1)
        {
            var cell = h.Cell();
            if (rowSpan > 1) cell = cell.RowSpan(rowSpan);
            if (colSpan > 1) cell = cell.ColumnSpan(colSpan);

            cell.Border(Bw).BorderColor(_border).Padding(3)
                .AlignCenter().AlignMiddle()
                .Text(text).FontSize(6.5f).Bold();
        }

        private void TaxCell(TableDescriptor t, string text, string align)
        {
            t.Cell().Border(Bw).BorderColor(_border).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(6.5f);
        }

        private void TaxFoot(TableCellDescriptor f, string text, string align)
        {
            f.Cell().Border(Bw).BorderColor(_border).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(6.5f).Bold();
        }

        // =================================================================
        //  6. Totals column
        // =================================================================
        private void ComposeTotalsColumn(IContainer container)
        {
            var bill = _data.Bill;

            container.Column(col =>
            {
                TotalLine(col, "Sub Total", Money(_data.TotalTaxable + _data.TotalDiscount));

                // Tally prints Discount, Tax and TCS/TDS on every invoice, each with
                // its rate, even when the amount is zero - so these are unconditional.
                TotalLine(col, "Discount" + PercentSuffix(EffectiveDiscountRate()), Money(_data.TotalDiscount));

                if (_s.PrintTaxDetails)
                    TotalLine(col, "Tax" + PercentSuffix(EffectiveTaxRate()), Money(_data.TotalTax));

                TotalLine(col, TcsTdsLabel() + PercentSuffix(bill.TdsTcsPercentage), Money(bill.TdsTcsAmount));

                if (_data.TotalCess != 0)
                    TotalLine(col, "Cess", Money(_data.TotalCess));

                if (bill.ShippingAmount != 0) TotalLine(col, "Shipping", Money(bill.ShippingAmount));
                if (bill.PackingAmount != 0) TotalLine(col, "Packing", Money(bill.PackingAmount));
                if (bill.AdjustmentAmount != 0) TotalLine(col, "Adjustment", Money(bill.AdjustmentAmount));


                if (bill.IsRoundOff && bill.RoundOffValue != 0)
                    TotalLine(col, "Round Off", Money(bill.RoundOffValue));

                TotalLine(col, "Total", Money(_data.GrandTotal), emphasise: true);

                col.Item().BorderBottom(Bw).BorderColor(_border)
                    .Element(Cell).Text("Invoice Amount In Words :").FontSize(Small);

                col.Item().BorderBottom(Bw).BorderColor(_border)
                    .Element(Cell)
                    .Text(QuestPdfEngine.AmountInWords(_data.GrandTotal, _s.AmountInWordsFormat))
                    .FontSize(Small);

                if (_s.PrintReceivedAmount)
                    TotalLine(col, "Received", Money(bill.paidReciveamount));

                if (_s.PrintBalanceAmount)
                    TotalLine(col, "Balance", Money(_data.GrandTotal - bill.paidReciveamount));

                if (_s.PrintCurrentBalanceParty)
                    TotalLine(col, "Current Balance", Money(_data.PartyCurrentBalance));

                if (_s.PrintYouSaved && _data.TotalDiscount > 0)
                    TotalLine(col, "You Saved", Money(_data.TotalDiscount), emphasise: true);
            });
        }

        private static string PercentSuffix(decimal percent)
        {
            return percent == 0m ? string.Empty : " (" + QuestPdfEngine.Percent(percent) + ")";
        }

        /// <summary>TCS or TDS, whichever the document is carrying.</summary>
        private string TcsTdsLabel()
        {
            return _data.Bill.TCSTDSType.ToString();
        }

        /// <summary>
        /// Header-level rates are often left at 0 while the real rate lives on the
        /// lines, so fall back to the blended rate actually charged.
        /// </summary>
        private decimal EffectiveTaxRate()
        {
            if (_data.Bill.TaxPercentage != 0m) return _data.Bill.TaxPercentage;
            if (_data.TotalTaxable == 0m) return 0m;

            var distinct = _data.Items
                .Where(i => i.TaxPercentage != 0m)
                .Select(i => i.TaxPercentage)
                .Distinct()
                .ToList();

            // A single rate across the bill prints as that rate; a mix prints blended.
            if (distinct.Count == 1) return distinct[0];

            return Math.Round(_data.TotalTax * 100m / _data.TotalTaxable, 2);
        }

        private decimal EffectiveDiscountRate()
        {
            if (_data.Bill.DiscountPercent != 0m) return _data.Bill.DiscountPercent;

            var gross = _data.TotalTaxable + _data.TotalDiscount;
            if (gross == 0m || _data.TotalDiscount == 0m) return 0m;

            return Math.Round(_data.TotalDiscount * 100m / gross, 2);
        }

        private void TotalLine(ColumnDescriptor col, string label, string value, bool emphasise = false)
        {
            var item = col.Item().BorderBottom(Bw).BorderColor(_border);
            if (emphasise) item = item.Background(_totalRowBg);

            item.Element(Cell).Row(r =>
            {
                var l = r.RelativeItem(1.5f).Text(label).FontSize(Small);
                r.ConstantItem(10f).Text(emphasise ? string.Empty : ":").FontSize(Small);
                var v = r.RelativeItem(1.2f).AlignRight().Text(value).FontSize(Small);

                if (emphasise)
                {
                    l.Bold().FontColor(_totalRowText);
                    v.Bold().FontColor(_totalRowText);
                }
            });
        }

        // =================================================================
        //  7. Payment mode
        // =================================================================
        private void ComposePaymentMode(ColumnDescriptor box)
        {
            if (!_s.PrintPaymentMode || string.IsNullOrWhiteSpace(_data.Bill.PaymentType)) return;

            box.Item().BorderTop(Bw).BorderBottom(Bw).BorderColor(_border)
                .Element(Caption).Text("Payment Mode:").FontSize(Small).Bold();

            box.Item().BorderBottom(Bw).BorderColor(_border)
                .Element(Cell).Text(_data.Bill.PaymentType.Trim()).FontSize(Small);
        }

        // =================================================================
        //  8. Description | Terms & Conditions
        // =================================================================
        private void ComposeDescriptionAndTerms(ColumnDescriptor box)
        {
            var description = _s.PrintDescription ? _data.Bill.Description : null;
            var terms = _s.PrintTermsConditions ? _s.DefaultTermsText : null;

            if (string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(terms)) return;

            box.Item().BorderTop(Bw).BorderBottom(Bw).BorderColor(_border).Row(row =>
            {
                row.RelativeItem().Element(Caption).Text("Description:").FontSize(Small).Bold();
                row.RelativeItem().BorderLeft(Bw).BorderColor(_border)
                    .Element(Caption).Text("Terms & Conditions:").FontSize(Small).Bold();
            });

            box.Item().BorderBottom(Bw).BorderColor(_border).Row(row =>
            {
                row.RelativeItem().Element(Cell)
                    .Text(QuestPdfEngine.Dash(description)).FontSize(Small);

                row.RelativeItem().BorderLeft(Bw).BorderColor(_border).Element(Cell)
                    .Text(QuestPdfEngine.Dash(terms)).FontSize(Small);
            });
        }

        // =================================================================
        //  9. Bank details | Signature
        // =================================================================
        private void ComposeBankAndSignature(ColumnDescriptor box)
        {
            var bank = _data.Context?.Bank;
            bool hasBank = _s.PrintBankDetails && bank != null &&
                           (!string.IsNullOrWhiteSpace(bank.BankName)
                            || !string.IsNullOrWhiteSpace(bank.AccountNumber)
                            || !string.IsNullOrWhiteSpace(bank.IFSCCode));

            bool hasSignature = _s.PrintSignatureText || _signatureBytes != null;
            if (!hasBank && !hasSignature) return;

            var forLine = "For: " + Pick(_s.CompanyNameText, _data.Company?.BusinessName, "Company") + ":";

            box.Item().BorderBottom(Bw).BorderColor(_border).Row(row =>
            {
                row.RelativeItem().Element(Caption).Text("Bank Details:").FontSize(Small).Bold();
                row.RelativeItem().BorderLeft(Bw).BorderColor(_border)
                    .Element(Caption).Text(forLine).FontSize(Small).Bold();
            });

            box.Item().Row(row =>
            {
                row.RelativeItem().Element(Cell).Row(bankRow =>
                {
                    bankRow.RelativeItem().PaddingLeft(4).Column(c =>
                    {
                        if (hasBank)
                        {
                            if (!string.IsNullOrWhiteSpace(bank.BankName))
                                c.Item().Text("Bank Name: " + bank.BankName).FontSize(Small).FontColor(_primary);

                            if (!string.IsNullOrWhiteSpace(bank.AccountNumber))
                                c.Item().PaddingTop(3).Text("Bank Account No.: " + bank.AccountNumber).FontSize(Small).FontColor(_primary);

                            if (!string.IsNullOrWhiteSpace(bank.IFSCCode))
                                c.Item().PaddingTop(3).Text("Bank IFSC code: " + bank.IFSCCode).FontSize(Small).FontColor(_primary);

                            if (_s.PrintUpiQr && !string.IsNullOrWhiteSpace(bank.UPIID))
                                c.Item().PaddingTop(3).Text("UPI: " + bank.UPIID).FontSize(Small).FontColor(_primary);
                        }
                        else
                        {
                            c.Item().Text(string.Empty);
                        }
                    });
                });

                row.RelativeItem().BorderLeft(Bw).BorderColor(_border).Element(Cell).Column(c =>
                {
                    if (_signatureBytes != null)
                        c.Item().AlignCenter().Height(46f).Image(_signatureBytes).FitArea();
                    else
                        c.Item().Height(34f);

                    if (_s.PrintSignatureText)
                    {
                        c.Item().PaddingTop(4).AlignCenter()
                            .Text(Pick(_s.SignatureText, "Authorized Signatory")).FontSize(Small).Bold();
                    }
                });
            });
        }

        // =================================================================
        //  Footer
        // =================================================================
        private void ComposePageFooter(IContainer container)
        {
            if (!_s.PrintPageNumbers)
            {
                container.Height(0);
                return;
            }

            container.PaddingTop(4).AlignRight().Text(text =>
            {
                text.DefaultTextStyle(x => x.FontSize(6.5f).FontColor(Colors.Grey.Darken1));
                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
        }
    }
}
