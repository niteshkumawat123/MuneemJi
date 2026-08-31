using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Renders a trade document as a PDF, driven entirely by the company
    /// PrintSettings row. Every toggle on Settings &gt; Print maps to a branch here.
    /// </summary>
    public class QuestInvoiceDocument : IDocument
    {
        private readonly QuestDocumentData _data;
        private readonly IWebHostEnvironment _env;
        private readonly PrintSettingsModel _s;

        private readonly Color _primary;
        private readonly Color _headerBg;
        private readonly Color _border;
        private readonly Color _totalRowBg;
        private readonly Color _totalRowText;
        private readonly Color _headerText;

        private readonly List<PrintItemColumnModel> _columns;
        private readonly byte[] _logoBytes;
        private readonly byte[] _signatureBytes;
        private readonly bool _thermal;

        private const float BorderWidth = 0.6f;

        public QuestInvoiceDocument(QuestDocumentData data, IWebHostEnvironment env)
        {
            _data = data ?? new QuestDocumentData();
            _env = env;
            _s = _data.Settings;

            _primary = QuestPdfEngine.ParseColor(_s.EffectivePrimaryColor, "#4E2A0A");
            _headerBg = QuestPdfEngine.ParseColor(_s.EffectiveHeaderBgColor, "#BBBBBB");
            _border = QuestPdfEngine.ParseColor(_s.EffectiveBorderColor, "#A9A9A9");
            _totalRowBg = QuestPdfEngine.ParseColor(_s.EffectiveTotalRowColor, "#FFF3CD");
            _headerText = QuestPdfEngine.ParseColor(_s.EffectiveHeaderTextColor, "#FFFFFF");

            // A picked accent can fill the total band solid, so its text has to
            // flip to whatever stays readable rather than staying the accent colour.
            _totalRowText = QuestPdfEngine.ParseColor(
                QuestPdfEngine.ContrastHex(_s.EffectiveTotalRowColor), "#000000");

            _thermal = QuestPdfEngine.IsThermal(_s.PaperSize);

            _columns = (_data.Context?.ItemColumns ?? new List<PrintItemColumnModel>())
                .Where(c => c.IsVisible)
                .OrderBy(c => c.SortOrder)
                .ToList();

            if (_columns.Count == 0)
            {
                _columns = PrintItemColumnCatalog.All
                    .Where(d => d.DefaultVisible)
                    .Select((d, i) => new PrintItemColumnModel
                    {
                        ColumnKey = d.Key,
                        HeaderText = d.DefaultHeader,
                        IsVisible = true,
                        SortOrder = i,
                        WidthPercent = d.DefaultWidth
                    })
                    .ToList();
            }

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
                labels.Add(null);   // one page, no copy caption
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

        private static string Pick(params string[] candidates)
        {
            foreach (var c in candidates)
                if (!string.IsNullOrWhiteSpace(c)) return c.Trim();
            return string.Empty;
        }

        // =================================================================
        //  Composition
        // =================================================================
        public void Compose(IDocumentContainer container)
        {
            foreach (var copyLabel in CopyLabels())
                ComposePage(container, copyLabel);
        }

        private void ComposePage(IDocumentContainer container, string copyLabel)
        {
            container.Page(page =>
            {
                // Thermal rolls have no fixed height - let the page grow with the content
                // instead of emitting a tall, mostly-blank page.
                var rollWidth = QuestPdfEngine.ThermalWidth(_s.PaperSize);
                if (rollWidth > 0f)
                    page.ContinuousSize(rollWidth);
                else
                    page.Size(QuestPdfEngine.ResolvePageSize(_s.PaperSize, _s.Orientation));

                page.MarginLeft((float)_s.MarginLeft);
                page.MarginRight((float)_s.MarginRight);
                page.MarginBottom((float)_s.MarginBottom);
                page.MarginTop((float)_s.ExtraSpaceTop + (_thermal ? 4f : 10f));

                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x
                    .FontFamily(FontFamilyName(), QuestPdfEngine.FallbackFontFamily)
                    .FontSize(_thermal ? 7f : 8f)
                    .FontColor(Colors.Black));

                if (!string.IsNullOrWhiteSpace(_s.WatermarkText))
                {
                    page.Foreground().AlignCenter().AlignMiddle()
                        .Text(_s.WatermarkText.Trim())
                        .FontSize(60).Bold().FontColor(_primary.WithAlpha((byte)26));
                }

                // "Print repeat header in all pages" - QuestPDF repeats page.Header()
                // on every page, so when the option is off the same block is emitted
                // once at the top of the content instead.
                if (_s.RepeatHeader)
                {
                    page.Header().Element(c => ComposeLetterhead(c, copyLabel));
                    page.Content().Element(ComposeBody);
                }
                else
                {
                    page.Content().Column(col =>
                    {
                        col.Item().Element(c => ComposeLetterhead(c, copyLabel));
                        col.Item().Element(ComposeBody);
                    });
                }

                page.Footer().Element(ComposePageFooter);
            });
        }

        private string FontFamilyName()
        {
            return string.IsNullOrWhiteSpace(_s.FontFamily) ? QuestPdfEngine.DefaultFontFamily : _s.FontFamily.Trim();
        }

        // =================================================================
        //  Letterhead (company block + title + copy caption)
        // =================================================================
        private void ComposeLetterhead(IContainer container, string copyLabel)
        {
            container.Column(col =>
            {
                if (!string.IsNullOrWhiteSpace(copyLabel))
                {
                    col.Item().AlignRight().PaddingBottom(2)
                        .Text(copyLabel).FontSize(7f).Bold().FontColor(_primary);
                }

                col.Item().Row(row =>
                {
                    if (_logoBytes != null)
                    {
                        row.ConstantItem(_thermal ? 40f : 60f)
                            .Height(_thermal ? 40f : 60f)
                            .AlignMiddle()
                            .Image(_logoBytes).FitArea();
                        row.ConstantItem(6f);
                    }

                    row.RelativeItem().Column(info =>
                    {
                        if (_s.PrintCompanyName)
                        {
                            var name = Pick(_s.CompanyNameText, _data.Company?.BusinessName, "Company Name");
                            info.Item().Text(name)
                                .FontSize(QuestPdfEngine.CompanyNameSize(_s.CompanyNameTextSize))
                                .Bold().FontColor(_primary);
                        }

                        if (_s.PrintAddress)
                        {
                            var address = Pick(_s.AddressText, BuildCompanyAddress());
                            if (!string.IsNullOrWhiteSpace(address))
                                info.Item().Text(address).FontSize(7.5f);
                        }

                        info.Item().Row(meta =>
                        {
                            var parts = new List<string>();

                            if (_s.PrintPhone)
                            {
                                var phone = Pick(_s.PhoneText, _data.Company?.PhoneNumber);
                                if (!string.IsNullOrWhiteSpace(phone)) parts.Add("Phone: " + phone);
                            }

                            if (_s.PrintEmail)
                            {
                                var email = Pick(_s.EmailText, _data.Company?.Email);
                                if (!string.IsNullOrWhiteSpace(email)) parts.Add("Email: " + email);
                            }

                            if (parts.Count > 0)
                                meta.RelativeItem().Text(string.Join("   |   ", parts)).FontSize(7.5f);
                        });

                        info.Item().Row(meta =>
                        {
                            var parts = new List<string>();

                            if (_s.PrintGstin)
                            {
                                var gstin = Pick(_s.GstinText, _data.Company?.Gstin);
                                if (!string.IsNullOrWhiteSpace(gstin)) parts.Add("GSTIN: " + gstin);
                            }

                            if (_s.PrintState && !string.IsNullOrWhiteSpace(_data.Company?.statename))
                                parts.Add("State: " + _data.Company.statename +
                                          (string.IsNullOrWhiteSpace(_data.Company.statecode) ? "" : " (" + _data.Company.statecode + ")"));

                            if (parts.Count > 0)
                                meta.RelativeItem().Text(string.Join("   |   ", parts)).FontSize(7.5f).Bold();
                        });
                    });
                });

                col.Item().PaddingTop(4).BorderBottom(BorderWidth).BorderColor(_border);

                col.Item().PaddingTop(4).AlignCenter()
                    .Text(_data.DocumentTitle)
                    .FontSize(QuestPdfEngine.TitleSize(_s.InvoiceTextSize))
                    .Bold().FontColor(_primary);

                col.Item().PaddingBottom(4);
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
            return string.Join(", ", parts);
        }

        // =================================================================
        //  Body
        // =================================================================
        private void ComposeBody(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Element(ComposePartyAndDocumentGrid);
                col.Item().PaddingTop(4).Element(ComposeItemTable);
                col.Item().PaddingTop(4).Element(ComposeSummarySection);
                col.Item().PaddingTop(4).Element(ComposeSignatureBlock);

                if (_s.PrintAcknowledgement)
                    col.Item().PaddingTop(8).Element(ComposeAcknowledgement);
            });
        }

        // -----------------------------------------------------------------
        //  Bill To / Ship To / document meta
        // -----------------------------------------------------------------
        private void ComposePartyAndDocumentGrid(IContainer container)
        {
            var bill = _data.Bill;
            var party = _data.Party;

            container.Border(BorderWidth).BorderColor(_border).Row(row =>
            {
                // ---- Bill To ----
                row.RelativeItem(_thermal ? 1f : 1.2f).Padding(4).Column(c =>
                {
                    c.Item().Text("Bill To").FontSize(7.5f).Bold().FontColor(_primary);

                    var billName = Pick(bill.BillingName, party?.PartyName, "-");
                    c.Item().Text(billName).Bold();

                    var billAddress = Pick(bill.BillingAddress, party?.BillingAddress);
                    if (!string.IsNullOrWhiteSpace(billAddress))
                        c.Item().Text(billAddress).FontSize(7.5f);

                    if (!string.IsNullOrWhiteSpace(party?.PhoneNumber))
                        c.Item().Text("Phone: " + party.PhoneNumber).FontSize(7.5f);

                    if (!string.IsNullOrWhiteSpace(party?.GSTIN))
                        c.Item().Text("GSTIN: " + party.GSTIN).FontSize(7.5f);

                    if (!string.IsNullOrWhiteSpace(party?.StateName))
                        c.Item().Text("State: " + party.StateName +
                                      (string.IsNullOrWhiteSpace(party.StateCode) ? "" : " (" + party.StateCode + ")"))
                            .FontSize(7.5f);
                });

                if (!_thermal)
                {
                    // ---- Ship To ----
                    row.RelativeItem(1.2f).BorderLeft(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                    {
                        c.Item().Text("Ship To").FontSize(7.5f).Bold().FontColor(_primary);

                        var shipAddress = Pick(bill.ShippingAddress, party?.ShippingAddress, party?.BillingAddress, "-");
                        c.Item().Text(shipAddress).FontSize(7.5f);

                        if (!string.IsNullOrWhiteSpace(bill.DeliveryLocation))
                            c.Item().Text("Delivery: " + bill.DeliveryLocation).FontSize(7.5f);

                        if (!string.IsNullOrWhiteSpace(bill.TransportName))
                            c.Item().Text("Transport: " + bill.TransportName).FontSize(7.5f);

                        if (!string.IsNullOrWhiteSpace(bill.VehicleNumber))
                            c.Item().Text("Vehicle: " + bill.VehicleNumber).FontSize(7.5f);

                        if (!string.IsNullOrWhiteSpace(bill.EWayBillNo))
                            c.Item().Text("E-Way Bill: " + bill.EWayBillNo).FontSize(7.5f);
                    });
                }

                // ---- Document meta ----
                row.RelativeItem(1f).BorderLeft(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                {
                    MetaLine(c, _data.DocumentTitle + " No.", DocumentNumber());
                    MetaLine(c, "Date", QuestPdfEngine.DateOrDash(bill.InvoiceDate != DateTime.MinValue ? bill.InvoiceDate : bill.BillDate));

                    if (bill.Time.HasValue && bill.Time.Value != TimeSpan.MinValue)
                        MetaLine(c, "Time", QuestPdfEngine.TimeOrDash(bill.Time));

                    if (bill.DueDate != DateTime.MinValue)
                        MetaLine(c, "Due Date", QuestPdfEngine.DateOrDash(bill.DueDate));

                    if (!string.IsNullOrWhiteSpace(bill.PONo))
                        MetaLine(c, "PO No.", bill.PONo);

                    if (bill.PODate.HasValue && bill.PODate.Value != DateTime.MinValue)
                        MetaLine(c, "PO Date", QuestPdfEngine.DateOrDash(bill.PODate));

                    if (!string.IsNullOrWhiteSpace(bill.ChallanNo))
                        MetaLine(c, "Challan No.", bill.ChallanNo);

                    if (_s.PrintPaymentMode && !string.IsNullOrWhiteSpace(bill.PaymentType))
                        MetaLine(c, "Payment Mode", bill.PaymentType);

                    if (!string.IsNullOrWhiteSpace(bill.StateOfSupply))
                        MetaLine(c, "Place of Supply", bill.StateOfSupply);
                });
            });
        }

        private static void MetaLine(ColumnDescriptor c, string label, string value)
        {
            c.Item().Row(r =>
            {
                r.RelativeItem(1.1f).Text(label).FontSize(7.5f);
                r.RelativeItem(1.4f).Text(QuestPdfEngine.Dash(value)).FontSize(7.5f).Bold();
            });
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

        // -----------------------------------------------------------------
        //  Item table
        // -----------------------------------------------------------------
        private void ComposeItemTable(IContainer container)
        {
            var items = _data.Items;
            bool dec = _s.PrintAmountWithDecimal;
            bool grp = _s.PrintAmountWithGrouping;

            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    foreach (var col in _columns)
                    {
                        var def = PrintItemColumnCatalog.Find(col.ColumnKey);
                        var width = col.WidthPercent > 0 ? (float)col.WidthPercent : (float)(def?.DefaultWidth ?? 8m);
                        cols.RelativeColumn(width);
                    }
                });

                // ---- Header row ----
                table.Header(header =>
                {
                    foreach (var col in _columns)
                    {
                        var def = PrintItemColumnCatalog.Find(col.ColumnKey);
                        var text = Pick(col.HeaderText, def?.DefaultHeader, col.ColumnKey);

                        header.Cell()
                            .Border(BorderWidth).BorderColor(_border)
                            .Background(_headerBg)
                            .Padding(3)
                            .Element(c => Align(c, def?.Align))
                            .Text(text).FontSize(7f).Bold().FontColor(_headerText);
                    }
                });

                // ---- Body rows ----
                int index = 0;
                foreach (var item in items)
                {
                    index++;
                    foreach (var col in _columns)
                    {
                        var def = PrintItemColumnCatalog.Find(col.ColumnKey);
                        var value = CellValue(col.ColumnKey, item, index, dec, grp);

                        table.Cell()
                            .Border(BorderWidth).BorderColor(_border)
                            .Padding(3)
                            .Element(c => Align(c, def?.Align))
                            .Text(value).FontSize(7f);
                    }
                }

                // ---- "Min No. of Rows": pad with empty rows ----
                for (int blank = items.Count; blank < _s.MinItemRows; blank++)
                {
                    foreach (var col in _columns)
                    {
                        table.Cell()
                            .Border(BorderWidth).BorderColor(_border)
                            .Padding(3)
                            .MinHeight(12f)
                            .Text(string.Empty);
                    }
                }

                // ---- Totals row ----
                table.Footer(footer =>
                {
                    foreach (var col in _columns)
                    {
                        var def = PrintItemColumnCatalog.Find(col.ColumnKey);
                        var value = FooterValue(col.ColumnKey, dec, grp);

                        footer.Cell()
                            .Border(BorderWidth).BorderColor(_border)
                            .Background(_totalRowBg)
                            .Padding(3)
                            .Element(c => Align(c, def?.Align))
                            .Text(value).FontSize(7f).Bold().FontColor(_totalRowText);
                    }
                });

                // "Expand table to print on whole page"
                if (_s.ExpandItemTable)
                    table.ExtendLastCellsToTableBottom();
            });
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

        private string CellValue(string key, PurchaseBillItem item, int index, bool dec, bool grp)
        {
            var lineGross = item.Quantity * item.PricePerUnit;
            var lineTaxable = lineGross - item.DiscountAmount;

            switch ((key ?? string.Empty).ToLowerInvariant())
            {
                case "srno": return index.ToString();
                case "itemcode": return QuestPdfEngine.Dash(item.ItemCode);
                case "itemname": return QuestPdfEngine.Dash(item.Item);
                case "hsn": return QuestPdfEngine.Dash(item.HSNCode);
                case "batchno": return QuestPdfEngine.Dash(item.batchno);
                case "serialno": return QuestPdfEngine.Dash(item.serialno);
                case "modelno": return QuestPdfEngine.Dash(item.modelno);
                case "expirydate": return QuestPdfEngine.DateOrDash(item.ExpiryDate);
                case "quantity": return QuestPdfEngine.Qty(item.Quantity);
                case "unit": return QuestPdfEngine.Dash(item.Unit);
                case "priceperunit": return QuestPdfEngine.Money(item.PricePerUnit, dec, grp);
                case "discountpct": return QuestPdfEngine.Percent(item.DiscountPercentage);
                case "discountamt": return QuestPdfEngine.Money(item.DiscountAmount, dec, grp);
                case "taxablevalue": return QuestPdfEngine.Money(lineTaxable, dec, grp);
                case "taxpct": return QuestPdfEngine.Percent(item.TaxPercentage);
                case "taxamt": return QuestPdfEngine.Money(item.TaxAmount, dec, grp);
                case "cess": return QuestPdfEngine.Money(item.AddCessAmount ?? 0m, dec, grp);
                case "amount": return QuestPdfEngine.Money(item.TotalAmount ?? (lineTaxable + item.TaxAmount), dec, grp);
                default: return string.Empty;
            }
        }

        private string FooterValue(string key, bool dec, bool grp)
        {
            switch ((key ?? string.Empty).ToLowerInvariant())
            {
                case "itemname": return "Total";
                case "quantity": return _s.PrintTotalItemQuantity ? QuestPdfEngine.Qty(_data.TotalQuantity) : string.Empty;
                case "discountamt": return QuestPdfEngine.Money(_data.TotalDiscount, dec, grp);
                case "taxablevalue": return QuestPdfEngine.Money(_data.TotalTaxable, dec, grp);
                case "taxamt": return QuestPdfEngine.Money(_data.TotalTax, dec, grp);
                case "cess": return QuestPdfEngine.Money(_data.TotalCess, dec, grp);
                case "amount": return QuestPdfEngine.Money(_data.TotalTaxable + _data.TotalTax + _data.TotalCess, dec, grp);
                default: return string.Empty;
            }
        }

        // -----------------------------------------------------------------
        //  Summary: words + tax table + terms | totals
        // -----------------------------------------------------------------
        private void ComposeSummarySection(IContainer container)
        {
            if (_thermal)
            {
                container.Column(col =>
                {
                    col.Item().Element(ComposeTotalsBlock);
                    col.Item().PaddingTop(4).Element(ComposeWordsAndTerms);
                });
                return;
            }

            container.Row(row =>
            {
                row.RelativeItem(1.35f).Element(ComposeWordsAndTerms);
                row.ConstantItem(6f);
                row.RelativeItem(1f).Element(ComposeTotalsBlock);
            });
        }

        private void ComposeWordsAndTerms(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                {
                    c.Item().Text("Amount In Words").FontSize(7f).Bold().FontColor(_primary);
                    c.Item().Text(QuestPdfEngine.AmountInWords(_data.GrandTotal, _s.AmountInWordsFormat))
                        .FontSize(7.5f).Bold();
                });

                if (_s.PrintTaxDetails && _data.TaxSummary.Count > 0)
                    col.Item().PaddingTop(4).Element(ComposeTaxSummaryTable);

                if (_s.PrintBankDetails && HasBank())
                    col.Item().PaddingTop(4).Element(ComposeBankBlock);

                if (_s.PrintDescription && !string.IsNullOrWhiteSpace(_data.Bill.Description))
                {
                    col.Item().PaddingTop(4).Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                    {
                        c.Item().Text("Description").FontSize(7f).Bold().FontColor(_primary);
                        c.Item().Text(_data.Bill.Description.Trim()).FontSize(7.5f);
                    });
                }

                if (_s.PrintTermsConditions && !string.IsNullOrWhiteSpace(_s.DefaultTermsText))
                {
                    col.Item().PaddingTop(4).Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                    {
                        c.Item().Text("Terms and Conditions").FontSize(7f).Bold().FontColor(_primary);
                        c.Item().Text(_s.DefaultTermsText.Trim()).FontSize(7f);
                    });
                }
            });
        }

        private void ComposeTaxSummaryTable(IContainer container)
        {
            bool dec = _s.PrintAmountWithDecimal;
            bool grp = _s.PrintAmountWithGrouping;
            bool split = _data.IsDomestic;   // CGST + SGST vs a single IGST column

            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(1.2f);   // rate
                    cols.RelativeColumn(1.6f);   // taxable
                    if (split)
                    {
                        cols.RelativeColumn(1.4f);   // CGST
                        cols.RelativeColumn(1.4f);   // SGST
                    }
                    else
                    {
                        cols.RelativeColumn(2.8f);   // IGST
                    }
                    cols.RelativeColumn(1.5f);   // total tax
                });

                table.Header(header =>
                {
                    HeaderCell(header, "Tax Rate", "center");
                    HeaderCell(header, "Taxable Value", "right");
                    if (split)
                    {
                        HeaderCell(header, "CGST", "right");
                        HeaderCell(header, "SGST", "right");
                    }
                    else
                    {
                        HeaderCell(header, "IGST", "right");
                    }
                    HeaderCell(header, "Total Tax", "right");
                });

                foreach (var t in _data.TaxSummary)
                {
                    BodyCell(table, QuestPdfEngine.Percent(t.Rate), "center");
                    BodyCell(table, QuestPdfEngine.Money(t.Taxable, dec, grp), "right");

                    if (split)
                    {
                        BodyCell(table, QuestPdfEngine.Money(t.TaxAmount / 2m, dec, grp), "right");
                        BodyCell(table, QuestPdfEngine.Money(t.TaxAmount / 2m, dec, grp), "right");
                    }
                    else
                    {
                        BodyCell(table, QuestPdfEngine.Money(t.TaxAmount, dec, grp), "right");
                    }

                    BodyCell(table, QuestPdfEngine.Money(t.TaxAmount + t.Cess, dec, grp), "right");
                }

                table.Footer(footer =>
                {
                    FooterCell(footer, "Total", "center");
                    FooterCell(footer, QuestPdfEngine.Money(_data.TotalTaxable, dec, grp), "right");

                    if (split)
                    {
                        FooterCell(footer, QuestPdfEngine.Money(_data.TotalTax / 2m, dec, grp), "right");
                        FooterCell(footer, QuestPdfEngine.Money(_data.TotalTax / 2m, dec, grp), "right");
                    }
                    else
                    {
                        FooterCell(footer, QuestPdfEngine.Money(_data.TotalTax, dec, grp), "right");
                    }

                    FooterCell(footer, QuestPdfEngine.Money(_data.TotalTax + _data.TotalCess, dec, grp), "right");
                });
            });
        }

        private void HeaderCell(TableCellDescriptor header, string text, string align)
        {
            header.Cell()
                .Border(BorderWidth).BorderColor(_border)
                .Background(_headerBg).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(7f).Bold().FontColor(_headerText);
        }

        private void BodyCell(TableDescriptor table, string text, string align)
        {
            table.Cell()
                .Border(BorderWidth).BorderColor(_border).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(7f);
        }

        private void FooterCell(TableCellDescriptor footer, string text, string align)
        {
            footer.Cell()
                .Border(BorderWidth).BorderColor(_border)
                .Background(_totalRowBg).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(7f).Bold().FontColor(_totalRowText);
        }

        private bool HasBank()
        {
            var b = _data.Context?.Bank;
            return b != null && (!string.IsNullOrWhiteSpace(b.BankName)
                                 || !string.IsNullOrWhiteSpace(b.AccountNumber)
                                 || !string.IsNullOrWhiteSpace(b.UPIID));
        }

        private void ComposeBankBlock(IContainer container)
        {
            var b = _data.Context.Bank;

            container.Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
            {
                c.Item().Text("Bank Details").FontSize(7f).Bold().FontColor(_primary);

                if (!string.IsNullOrWhiteSpace(b.BankName))
                    c.Item().Text("Bank: " + b.BankName).FontSize(7.5f);

                if (!string.IsNullOrWhiteSpace(b.AccountHolderName))
                    c.Item().Text("Account Name: " + b.AccountHolderName).FontSize(7.5f);

                if (!string.IsNullOrWhiteSpace(b.AccountNumber))
                    c.Item().Text("Account No.: " + b.AccountNumber).FontSize(7.5f);

                if (!string.IsNullOrWhiteSpace(b.IFSCCode))
                    c.Item().Text("IFSC: " + b.IFSCCode).FontSize(7.5f);

                if (_s.PrintUpiQr && !string.IsNullOrWhiteSpace(b.UPIID))
                    c.Item().Text("UPI: " + b.UPIID).FontSize(7.5f);
            });
        }

        // -----------------------------------------------------------------
        //  Totals block
        // -----------------------------------------------------------------
        private void ComposeTotalsBlock(IContainer container)
        {
            var bill = _data.Bill;
            bool dec = _s.PrintAmountWithDecimal;
            bool grp = _s.PrintAmountWithGrouping;

            container.Border(BorderWidth).BorderColor(_border).Column(col =>
            {
                TotalLine(col, "Sub Total", QuestPdfEngine.Money(_data.TotalTaxable, dec, grp), false);

                if (_data.TotalDiscount > 0)
                    TotalLine(col, "Discount", "- " + QuestPdfEngine.Money(_data.TotalDiscount, dec, grp), false);

                if (_s.PrintTaxDetails && _data.TotalTax != 0)
                {
                    if (_data.IsDomestic)
                    {
                        TotalLine(col, "CGST", QuestPdfEngine.Money(_data.TotalTax / 2m, dec, grp), false);
                        TotalLine(col, "SGST", QuestPdfEngine.Money(_data.TotalTax / 2m, dec, grp), false);
                    }
                    else
                    {
                        TotalLine(col, "IGST", QuestPdfEngine.Money(_data.TotalTax, dec, grp), false);
                    }
                }

                if (_data.TotalCess != 0)
                    TotalLine(col, "Cess", QuestPdfEngine.Money(_data.TotalCess, dec, grp), false);

                if (bill.ShippingAmount != 0)
                    TotalLine(col, "Shipping", QuestPdfEngine.Money(bill.ShippingAmount, dec, grp), false);

                if (bill.PackingAmount != 0)
                    TotalLine(col, "Packing", QuestPdfEngine.Money(bill.PackingAmount, dec, grp), false);

                if (bill.AdjustmentAmount != 0)
                    TotalLine(col, "Adjustment", QuestPdfEngine.Money(bill.AdjustmentAmount, dec, grp), false);

                if (bill.TdsTcsAmount != 0)
                    TotalLine(col, bill.TCSTDSType.ToString(), QuestPdfEngine.Money(bill.TdsTcsAmount, dec, grp), false);

                if (bill.IsRoundOff && bill.RoundOffValue != 0)
                    TotalLine(col, "Round Off", QuestPdfEngine.Money(bill.RoundOffValue, dec, grp), false);

                TotalLine(col, "Total", QuestPdfEngine.Money(_data.GrandTotal, dec, grp), true);

                if (_s.PrintReceivedAmount)
                    TotalLine(col, "Received", QuestPdfEngine.Money(bill.paidReciveamount, dec, grp), false);

                if (_s.PrintBalanceAmount)
                    TotalLine(col, "Balance", QuestPdfEngine.Money(_data.GrandTotal - bill.paidReciveamount, dec, grp), false);

                if (_s.PrintCurrentBalanceParty)
                    TotalLine(col, "Current Balance", QuestPdfEngine.Money(_data.PartyCurrentBalance, dec, grp), false);

                if (_s.PrintYouSaved && _data.TotalDiscount > 0)
                {
                    col.Item().Background(_totalRowBg).Padding(3).AlignCenter()
                        .Text("You Saved " + QuestPdfEngine.Money(_data.TotalDiscount, dec, grp))
                        .FontSize(7.5f).Bold().FontColor(_totalRowText);
                }
            });
        }

        private void TotalLine(ColumnDescriptor col, string label, string value, bool emphasise)
        {
            var item = col.Item();
            if (emphasise) item = item.Background(_totalRowBg);

            item.BorderBottom(BorderWidth).BorderColor(_border).Padding(3).Row(r =>
            {
                var left = r.RelativeItem(1.3f).Text(label).FontSize(emphasise ? 8.5f : 7.5f);
                if (emphasise) left.Bold().FontColor(_totalRowText);

                var right = r.RelativeItem(1f).AlignRight().Text(value).FontSize(emphasise ? 8.5f : 7.5f);
                if (emphasise) right.Bold().FontColor(_totalRowText);
            });
        }

        // -----------------------------------------------------------------
        //  Signature / received by / delivered by
        // -----------------------------------------------------------------
        private void ComposeSignatureBlock(IContainer container)
        {
            container.Row(row =>
            {
                if (_s.PrintReceivedBy)
                {
                    row.RelativeItem().Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                    {
                        c.Item().Text("Received By").FontSize(7f).Bold().FontColor(_primary);
                        c.Item().PaddingTop(18).Text("Name: ______________").FontSize(7f);
                        c.Item().PaddingTop(2).Text("Date: ______________").FontSize(7f);
                    });
                    row.ConstantItem(4f);
                }

                if (_s.PrintDeliveredBy)
                {
                    row.RelativeItem().Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                    {
                        c.Item().Text("Delivered By").FontSize(7f).Bold().FontColor(_primary);
                        c.Item().PaddingTop(18).Text("Name: ______________").FontSize(7f);
                        c.Item().PaddingTop(2).Text("Date: ______________").FontSize(7f);
                    });
                    row.ConstantItem(4f);
                }

                row.RelativeItem().Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                {
                    var companyName = Pick(_s.CompanyNameText, _data.Company?.BusinessName);
                    if (!string.IsNullOrWhiteSpace(companyName))
                        c.Item().AlignRight().Text("For " + companyName).FontSize(7f).Bold();

                    if (_signatureBytes != null)
                        c.Item().AlignRight().Height(34f).Image(_signatureBytes).FitArea();
                    else
                        c.Item().PaddingTop(24);

                    if (_s.PrintSignatureText)
                    {
                        c.Item().AlignRight()
                            .Text(Pick(_s.SignatureText, "Authorized Signatory"))
                            .FontSize(7f).Bold();
                    }
                });
            });
        }

        // -----------------------------------------------------------------
        //  Acknowledgement slip
        // -----------------------------------------------------------------
        private void ComposeAcknowledgement(IContainer container)
        {
            bool dec = _s.PrintAmountWithDecimal;
            bool grp = _s.PrintAmountWithGrouping;

            container.Column(col =>
            {
                col.Item().PaddingBottom(3).Text("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -")
                    .FontSize(7f).FontColor(_border);

                col.Item().Border(BorderWidth).BorderColor(_border).Padding(4).Column(c =>
                {
                    c.Item().AlignCenter().Text("ACKNOWLEDGEMENT").FontSize(8f).Bold().FontColor(_primary);

                    c.Item().PaddingTop(3).Row(r =>
                    {
                        r.RelativeItem().Column(x =>
                        {
                            x.Item().Text(Pick(_s.CompanyNameText, _data.Company?.BusinessName, "-")).FontSize(7.5f).Bold();
                            x.Item().Text(_data.DocumentTitle + " No.: " + DocumentNumber()).FontSize(7f);
                            x.Item().Text("Date: " + QuestPdfEngine.DateOrDash(
                                _data.Bill.InvoiceDate != DateTime.MinValue ? _data.Bill.InvoiceDate : _data.Bill.BillDate)).FontSize(7f);
                            x.Item().Text("Amount: " + QuestPdfEngine.Money(_data.GrandTotal, dec, grp)).FontSize(7f).Bold();
                        });

                        r.RelativeItem().AlignRight().AlignBottom().Column(x =>
                        {
                            x.Item().PaddingTop(20).Text("Receiver Signature").FontSize(7f).Bold();
                        });
                    });
                });
            });
        }

        // -----------------------------------------------------------------
        //  Page footer
        // -----------------------------------------------------------------
        private void ComposePageFooter(IContainer container)
        {
            container.PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("This is a computer generated document.").FontSize(6.5f).FontColor(Colors.Grey.Darken1);
                });

                if (_s.PrintPageNumbers)
                {
                    row.ConstantItem(90f).AlignRight().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(6.5f).FontColor(Colors.Grey.Darken1));
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                }
            });
        }
    }
}
