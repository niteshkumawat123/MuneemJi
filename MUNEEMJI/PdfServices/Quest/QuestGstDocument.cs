using MUNEEMJI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Renders the GST Theme 1-6 and Theme 1-4 families. They share a skeleton -
    /// company header, centred title, three-column party band, item grid, then a
    /// tax block beside the amounts - and differ in the switches carried by
    /// <see cref="QuestThemeStyle"/>: filled bars vs open rules, logo side,
    /// banner header, split GST columns and the order of the closing blocks.
    /// </summary>
    public class QuestGstDocument : QuestDocumentBase
    {
        public QuestGstDocument(QuestDocumentData data, IWebHostEnvironment env)
            : base(data, env)
        {
        }

        private bool Boxed => Style.Family == ThemeFamily.GstBoxed;

        public override void Compose(IDocumentContainer container)
        {
            foreach (var copyLabel in CopyLabels())
                ComposePage(container, copyLabel);
        }

        private void ComposePage(IDocumentContainer container, string copyLabel)
        {
            container.Page(page =>
            {
                ApplyPageChrome(page);

                page.Content().Column(outer =>
                {
                    if (!string.IsNullOrWhiteSpace(copyLabel))
                    {
                        outer.Item().AlignRight().PaddingBottom(3)
                            .Text(copyLabel).FontSize(8f).Bold().FontColor(Accent);
                    }

                    // The boxed family centres the title above the border; the open
                    // family puts it between two rules inside the flow.
                    if (Boxed)
                    {
                        outer.Item().AlignCenter().PaddingBottom(4)
                            .Text(Data.DocumentTitle).FontSize(9.5f).Bold();

                        outer.Item().Border(Bw).BorderColor(Border).Column(ComposeBody);
                    }
                    else
                    {
                        outer.Item().Column(ComposeBody);
                    }
                });
            });
        }

        private void ComposeBody(ColumnDescriptor col)
        {
            // GST Theme 3 draws the company block inside its detail grid, so the
            // standalone header would duplicate it.
            if (!Style.InvoiceDetailGrid)
                ComposeHeader(col);

            if (!Boxed)
            {
                if (Style.RuleUnderHeader) Rule(col, Style.AccentRules);

                col.Item().AlignCenter().PaddingVertical(2)
                    .Text(Data.DocumentTitle)
                    .FontSize(QuestPdfEngine.TitleSize(S.InvoiceTextSize)).Bold().FontColor(Accent);

                if (Style.RuleUnderTitle) Rule(col, Style.AccentRules);
                else col.Item().PaddingBottom(6);
            }

            if (Style.InvoiceDetailGrid)
                ComposeInvoiceDetailGrid(col);
            else
                ComposePartyBand(col);

            col.Item().Element(ComposeItemTable);

            if (Style.WordsBeforeTaxBlock)
            {
                ComposeWordsAndAmounts(col);
                ComposePaymentModeBlock(col);
                ComposeTaxBlockRow(col);
            }
            else
            {
                ComposeTaxAndAmounts(col);
                ComposeWordsAndDescription(col);
                ComposePaymentModeBlock(col);
            }

            ComposeClosingRow(col);
        }

        // =================================================================
        //  Header
        // =================================================================
        private void ComposeHeader(ColumnDescriptor col)
        {
            if (Style.BannerHeader)
            {
                col.Item().Background(Accent).Padding(8).Element(c => ComposeHeaderRow(c, true));
                return;
            }

            col.Item().Padding(Boxed ? 6f : 2f).Element(c => ComposeHeaderRow(c, false));
        }

        private void ComposeHeaderRow(IContainer container, bool onBanner)
        {
            var ink = onBanner ? AccentText : Colors.Black;
            var muted = onBanner ? AccentText : Accent;

            container.Row(row =>
            {
                if (Style.Logo == LogoSide.Left)
                {
                    ComposeLogoCell(row, onBanner);
                    row.ConstantItem(8f);
                }

                row.RelativeItem().Column(info =>
                {
                    var align = Style.CompanyTextRight ? "right" : "left";

                    if (S.PrintCompanyName)
                    {
                        Align(info.Item(), align)
                            .Text(Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company Name"))
                            .FontSize(QuestPdfEngine.CompanyNameSize(S.CompanyNameTextSize))
                            .Bold().FontColor(ink);
                    }

                    if (S.PrintAddress)
                    {
                        var address = Pick(S.AddressText, BuildCompanyAddress());
                        if (!string.IsNullOrWhiteSpace(address))
                            Align(info.Item().PaddingTop(1), align).Text(address).FontSize(Small).FontColor(ink);
                    }

                    if (S.PrintPhone)
                    {
                        var phone = Pick(S.PhoneText, Data.Company?.PhoneNumber);
                        if (!string.IsNullOrWhiteSpace(phone))
                            Align(info.Item().PaddingTop(1), align).Text("Ph. no.: " + phone).FontSize(Small).FontColor(ink);
                    }

                    if (S.PrintEmail)
                    {
                        var email = Pick(S.EmailText, Data.Company?.Email);
                        if (!string.IsNullOrWhiteSpace(email))
                            Align(info.Item().PaddingTop(1), align).Text("Email: " + email).FontSize(Small).FontColor(ink);
                    }

                    if (S.PrintGstin)
                    {
                        var gstin = Pick(S.GstinText, Data.Company?.Gstin);
                        if (!string.IsNullOrWhiteSpace(gstin))
                            Align(info.Item().PaddingTop(1), align).Text("GSTIN : " + gstin).FontSize(Small).Bold().FontColor(muted);
                    }

                    if (S.PrintState)
                    {
                        var state = BuildStateLine();
                        if (!string.IsNullOrWhiteSpace(state))
                            Align(info.Item().PaddingTop(1), align).Text("State : " + state).FontSize(Small).Bold().FontColor(muted);
                    }
                });

                if (Style.Logo == LogoSide.Right)
                {
                    row.ConstantItem(8f);
                    ComposeLogoCell(row, onBanner);
                }
            });
        }

        private void ComposeLogoCell(RowDescriptor row, bool onBanner)
        {
            if (LogoBytes == null)
            {
                row.ConstantItem(0f);
                return;
            }

            var cell = row.ConstantItem(74f).AlignMiddle();

            // A banner needs the logo on a white card so a dark mark stays visible.
            if (Style.LogoCard && onBanner)
                cell.Background(Colors.White).Padding(5).Height(46f).Image(LogoBytes).FitArea();
            else
                cell.Height(46f).Image(LogoBytes).FitArea();
        }

        // =================================================================
        //  Party band - Bill To | Shipping To | Invoice Details
        // =================================================================
        private void ComposePartyBand(ColumnDescriptor col)
        {
            var filled = Style.FilledPartyBars;

            if (filled)
            {
                col.Item().BorderVertical(Boxed ? Bw : 0f).BorderColor(Border).Row(row =>
                {
                    CaptionCell(row.RelativeItem(1.25f), "Bill To:", true);
                    CaptionCell(row.RelativeItem(1.1f), "Shipping To", true);
                    CaptionCell(row.RelativeItem(1f).AlignRight(), "Invoice Details", true);
                });
            }
            else
            {
                col.Item().PaddingTop(2).Row(row =>
                {
                    row.RelativeItem(1.25f).Element(PadCell).Text("Bill To:").FontSize(Small).Bold();
                    row.RelativeItem(1.1f).Element(PadCell).Text("Shipping To").FontSize(Small).Bold();
                    row.RelativeItem(1f).Element(PadCell).AlignRight().Text("Invoice Details").FontSize(Small).Bold();
                });
            }

            var band = col.Item();
            if (Boxed) band = band.BorderBottom(Bw).BorderColor(Border);

            band.Row(row =>
            {
                // ---- Bill To ----
                var left = row.RelativeItem(1.25f);
                if (Boxed) left = left.BorderRight(Bw).BorderColor(Border);

                left.Element(PadCell).Column(c =>
                {
                    if (Style.InlinePartyName)
                    {
                        c.Item().Text("Bill To: " + PartyName()).FontSize(Small).Bold();
                    }
                    else
                    {
                        c.Item().Text(PartyName()).FontSize(Small).Bold();
                    }

                    var address = BillAddress();
                    if (!string.IsNullOrWhiteSpace(address))
                        c.Item().PaddingTop(2).Text(address).FontSize(Small);

                    var phone = PartyPhone();
                    if (!string.IsNullOrWhiteSpace(phone))
                        c.Item().PaddingTop(2).Text("Contact No.: " + phone).FontSize(Small);

                    if (!string.IsNullOrWhiteSpace(Data.Party?.GSTIN))
                        c.Item().PaddingTop(2).Text("GSTIN Number: " + Data.Party.GSTIN).FontSize(Small);
                });

                // ---- Shipping To ----
                var mid = row.RelativeItem(1.1f);
                if (Boxed) mid = mid.BorderRight(Bw).BorderColor(Border);

                mid.Element(PadCell).Column(c =>
                {
                    c.Item().Text(QuestPdfEngine.Dash(ShipAddress())).FontSize(Small);

                    if (!string.IsNullOrWhiteSpace(Data.Bill.TransportName))
                        c.Item().PaddingTop(2).Text("Transport: " + Data.Bill.TransportName).FontSize(Small);

                    if (!string.IsNullOrWhiteSpace(Data.Bill.VehicleNumber))
                        c.Item().PaddingTop(2).Text("Vehicle: " + Data.Bill.VehicleNumber).FontSize(Small);
                });

                // ---- Invoice Details ----
                row.RelativeItem(1f).Element(PadCell).Column(c =>
                {
                    DetailLine(c, NumberLabel() + ": " + DocumentNumber());
                    DetailLine(c, "Date: " + DocumentDate());

                    if (Data.Bill.Time.HasValue && Data.Bill.Time.Value != TimeSpan.MinValue)
                        DetailLine(c, "Time: " + QuestPdfEngine.TimeOrDash(Data.Bill.Time));

                    if (Data.Bill.DueDate != DateTime.MinValue)
                        DetailLine(c, "Due Date: " + QuestPdfEngine.DateOrDash(Data.Bill.DueDate));

                    if (!string.IsNullOrWhiteSpace(Data.Bill.PONo))
                        DetailLine(c, "PO No.: " + Data.Bill.PONo);
                });
            });
        }

        private void DetailLine(ColumnDescriptor c, string text)
        {
            c.Item().PaddingTop(2).AlignRight().Text(text).FontSize(Small);
        }

        /// <summary>GST Theme 3 prints shipping fields as a two-column label/value grid.</summary>
        private void ComposeInvoiceDetailGrid(ColumnDescriptor col)
        {
            var bill = Data.Bill;

            var pairs = new List<(string label, string value)>
            {
                (NumberLabel(), DocumentNumber()),
                ("Date", DocumentDate()),
                ("Due Date", QuestPdfEngine.DateOrDash(bill.DueDate)),
                ("Transport Name", QuestPdfEngine.Dash(bill.TransportName)),
                ("Vehicle Number", QuestPdfEngine.Dash(bill.VehicleNumber)),
                ("Delivery Date", QuestPdfEngine.DateOrDash(bill.DeliveryDate)),
                ("Delivery Location", QuestPdfEngine.Dash(bill.DeliveryLocation))
            };

            col.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem(1f).BorderRight(Bw).BorderColor(Border)
                    .Element(c => ComposeHeaderRow(c, false));

                row.RelativeItem(1.15f).Column(grid =>
                {
                    for (int i = 0; i < pairs.Count; i += 2)
                    {
                        var a = pairs[i];
                        var b = i + 1 < pairs.Count ? pairs[i + 1] : (label: (string)null, value: (string)null);

                        grid.Item().BorderBottom(Bw).BorderColor(Border).Row(r =>
                        {
                            r.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell).Column(c =>
                            {
                                c.Item().Text(a.label).FontSize(Tiny);
                                c.Item().Text(QuestPdfEngine.Dash(a.value)).FontSize(Small).Bold();
                            });

                            if (b.label != null)
                            {
                                r.RelativeItem().Element(PadCell).Column(c =>
                                {
                                    c.Item().Text(b.label).FontSize(Tiny);
                                    c.Item().Text(QuestPdfEngine.Dash(b.value)).FontSize(Small).Bold();
                                });
                            }
                            else
                            {
                                r.RelativeItem();
                            }
                        });
                    }
                });
            });

            // Bill To / Ship To underneath, as two columns
            col.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell).Column(c =>
                {
                    c.Item().Text("Bill To").FontSize(Small);
                    c.Item().PaddingTop(2).Text(PartyName()).FontSize(Small).Bold();

                    var address = BillAddress();
                    if (!string.IsNullOrWhiteSpace(address))
                        c.Item().PaddingTop(2).Text(address).FontSize(Small);

                    var phone = PartyPhone();
                    if (!string.IsNullOrWhiteSpace(phone))
                        c.Item().PaddingTop(2).Text("Contact No.: " + phone).FontSize(Small);
                });

                row.RelativeItem().Element(PadCell).Column(c =>
                {
                    c.Item().Text("Ship To").FontSize(Small);
                    c.Item().PaddingTop(2).Text(QuestPdfEngine.Dash(ShipAddress())).FontSize(Small);
                });
            });
        }

        // =================================================================
        //  Item grid
        // =================================================================
        private void ComposeItemTable(IContainer container)
        {
            bool split = Style.SplitGstColumns;

            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3.5f);     // #
                    c.RelativeColumn(20f);      // Item name
                    c.RelativeColumn(10f);      // HSC/SAC
                    c.RelativeColumn(9f);       // Quantity
                    c.RelativeColumn(10f);      // Price/unit
                    c.RelativeColumn(12f);      // Discount
                    if (split)
                    {
                        c.RelativeColumn(10f);  // CGST
                        c.RelativeColumn(10f);  // SGST
                    }
                    else
                    {
                        c.RelativeColumn(12f);  // GST
                    }
                    c.RelativeColumn(12f);      // Amount
                });

                table.Header(h =>
                {
                    Head(h, "#", "left");
                    Head(h, "Item name", "left");
                    Head(h, "HSC/SAC", "right");
                    Head(h, "Quantity", "right");
                    Head(h, "Price/unit", "right");
                    Head(h, "Discount", "right");
                    if (split)
                    {
                        Head(h, Data.IsDomestic ? "CGST" : "IGST", "right");
                        Head(h, Data.IsDomestic ? "SGST" : "Cess", "right");
                    }
                    else
                    {
                        Head(h, "GST", "right");
                    }
                    Head(h, "Amount", "right");
                });

                int index = 0;
                foreach (var item in Data.Items)
                {
                    index++;

                    var free = item.FreeQuantity ?? 0m;
                    var qty = QuestPdfEngine.Qty(item.Quantity) + (free > 0 ? "+" + QuestPdfEngine.Qty(free) : string.Empty);
                    var lineTaxable = item.Quantity * item.PricePerUnit - item.DiscountAmount;

                    Cell(table, index.ToString(), "left", Accent);
                    Cell(table, QuestPdfEngine.Dash(item.Item), "left", null, true);
                    Cell(table, QuestPdfEngine.Dash(item.HSNCode), "right");
                    Cell(table, qty, "right", Accent);
                    Cell(table, Money(item.PricePerUnit), "right");
                    Cell(table, Money(item.DiscountAmount) + " (" + QuestPdfEngine.Percent(item.DiscountPercentage) + ")", "right");

                    if (split)
                    {
                        var half = item.TaxAmount / 2m;
                        var halfRate = item.TaxPercentage / 2m;

                        if (Data.IsDomestic)
                        {
                            Cell(table, Money(half) + Environment.NewLine + "(" + QuestPdfEngine.Percent(halfRate) + ")", "right");
                            Cell(table, Money(half) + Environment.NewLine + "(" + QuestPdfEngine.Percent(halfRate) + ")", "right");
                        }
                        else
                        {
                            Cell(table, Money(item.TaxAmount) + Environment.NewLine + "(" + QuestPdfEngine.Percent(item.TaxPercentage) + ")", "right");
                            Cell(table, Money(item.AddCessAmount ?? 0m), "right");
                        }
                    }
                    else
                    {
                        Cell(table, Money(item.TaxAmount) + Environment.NewLine + "(" + QuestPdfEngine.Percent(item.TaxPercentage) + ")", "right");
                    }

                    Cell(table, Money(item.TotalAmount ?? (lineTaxable + item.TaxAmount)), "right");
                }

                for (int blank = Data.Items.Count; blank < S.MinItemRows; blank++)
                {
                    int columns = split ? 9 : 8;
                    for (int c = 0; c < columns; c++)
                        BlankCell(table);
                }

                table.Footer(f =>
                {
                    var totalFree = Data.Items.Sum(i => i.FreeQuantity ?? 0m);
                    var totalQty = QuestPdfEngine.Qty(Data.TotalQuantity)
                                   + (totalFree > 0 ? " + " + QuestPdfEngine.Qty(totalFree) : string.Empty);

                    Foot(f, string.Empty, "left");
                    Foot(f, "Total", "left");
                    Foot(f, string.Empty, "right");
                    Foot(f, S.PrintTotalItemQuantity ? totalQty : string.Empty, "right");
                    Foot(f, string.Empty, "right");
                    Foot(f, Money(Data.TotalDiscount), "right");

                    if (split)
                    {
                        Foot(f, Money(Data.IsDomestic ? Data.TotalTax / 2m : Data.TotalTax), "right");
                        Foot(f, Money(Data.IsDomestic ? Data.TotalTax / 2m : Data.TotalCess), "right");
                    }
                    else
                    {
                        Foot(f, Money(Data.TotalTax), "right");
                    }

                    Foot(f, Money(Data.TotalTaxable + Data.TotalTax + Data.TotalCess), "right");
                });

                if (S.ExpandItemTable)
                    table.ExtendLastCellsToTableBottom();
            });
        }

        private void Head(TableCellDescriptor h, string text, string align)
        {
            var cell = h.Cell();

            if (Style.FilledItemHeader)
            {
                cell.Background(Accent).PaddingVertical(4).PaddingHorizontal(4)
                    .Element(c => Align(c, align))
                    .Text(text).FontSize(Small).Bold().FontColor(AccentText);
            }
            else
            {
                cell.Border(Bw).BorderColor(Border).PaddingVertical(4).PaddingHorizontal(4)
                    .Element(c => Align(c, align))
                    .Text(text).FontSize(Small).Bold();
            }
        }

        private void Cell(TableDescriptor t, string text, string align, Color? color = null, bool bold = false)
        {
            IContainer cell = t.Cell();
            if (Style.BorderedItemTable) cell = cell.Border(Bw).BorderColor(Border);
            else cell = cell.BorderBottom(0.4f).BorderColor(Border);

            var span = cell.PaddingVertical(4).PaddingHorizontal(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small);

            if (bold) span.Bold();
            if (color.HasValue) span.FontColor(color.Value);
        }

        private void BlankCell(TableDescriptor t)
        {
            IContainer cell = t.Cell();
            if (Style.BorderedItemTable) cell = cell.Border(Bw).BorderColor(Border);
            else cell = cell.BorderBottom(0.4f).BorderColor(Border);

            cell.MinHeight(13f).Padding(4).Text(string.Empty);
        }

        private void Foot(TableCellDescriptor f, string text, string align)
        {
            IContainer cell = f.Cell();
            if (Style.BorderedItemTable) cell = cell.Border(Bw).BorderColor(Border);
            else cell = cell.BorderTop(Bw).BorderBottom(Bw).BorderColor(Border);

            cell.PaddingVertical(4).PaddingHorizontal(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold();
        }

        // =================================================================
        //  Tax block beside the amounts
        // =================================================================
        private void ComposeTaxAndAmounts(ColumnDescriptor col)
        {
            col.Item().PaddingTop(Boxed ? 0f : 8f).Row(row =>
            {
                var left = row.RelativeItem(1.05f);
                if (Boxed) left = left.BorderRight(Bw).BorderColor(Border);
                left.Element(ComposeTaxBlock);

                row.RelativeItem(1f).Element(ComposeAmountsBlock);
            });
        }

        private void ComposeTaxBlockRow(ColumnDescriptor col)
        {
            col.Item().PaddingTop(Boxed ? 0f : 8f).Row(row =>
            {
                var left = row.RelativeItem(1.05f);
                if (Boxed) left = left.BorderRight(Bw).BorderColor(Border);
                left.Element(ComposeTaxBlock);

                row.RelativeItem(1f).Element(c => c.Column(inner =>
                {
                    Caption(inner, "Description", Style.FilledLowerBars);
                    inner.Item().Element(PadCell)
                        .Text(QuestPdfEngine.Dash(S.PrintDescription ? Data.Bill.Description : null))
                        .FontSize(Small);
                }));
            });
        }

        private void ComposeTaxBlock(IContainer container)
        {
            switch (Style.TaxBlock)
            {
                case TaxBlockKind.HsnGrid:
                    container.Element(ComposeHsnGrid);
                    break;
                case TaxBlockKind.TaxDetails:
                    container.Element(ComposeTaxDetailsStrip);
                    break;
                case TaxBlockKind.FoldedIntoTotals:
                    // No separate tax table: the rates are already listed in the
                    // amounts column, so this slot carries the description.
                    container.Column(c =>
                    {
                        Caption(c, "Description", Style.FilledLowerBars);
                        c.Item().Element(PadCell)
                            .Text(QuestPdfEngine.Dash(S.PrintDescription ? Data.Bill.Description : null))
                            .FontSize(Small);
                    });
                    break;
                default:
                    container.Element(ComposeTaxTypeTable);
                    break;
            }
        }

        /// <summary>Tax type | Taxable amount | Rate | Tax amount.</summary>
        private void ComposeTaxTypeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(10f);
                    c.RelativeColumn(14f);
                    c.RelativeColumn(8f);
                    c.RelativeColumn(12f);
                });

                table.Header(h =>
                {
                    TaxHead(h, "Tax type", "left");
                    TaxHead(h, "Taxable amount", "right");
                    TaxHead(h, "Rate", "right");
                    TaxHead(h, "Tax amount", "right");
                });

                foreach (var r in TaxTypeRows())
                {
                    TaxCell(table, r.Type, "left");
                    TaxCell(table, Money(r.Taxable), "right");
                    TaxCell(table, QuestPdfEngine.Percent(r.Rate), "right");
                    TaxCell(table, Money(r.Amount), "right");
                }
            });
        }

        /// <summary>HSN/SAC grid with CGST and SGST sub-columns.</summary>
        private void ComposeHsnGrid(IContainer container)
        {
            bool split = Data.IsDomestic;

            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(11f);
                    c.RelativeColumn(14f);
                    c.RelativeColumn(8f);
                    c.RelativeColumn(11f);
                    if (split)
                    {
                        c.RelativeColumn(8f);
                        c.RelativeColumn(11f);
                    }
                    c.RelativeColumn(14f);
                });

                var cur = " (" + QuestPdfEngine.Rupee + ")";

                table.Header(h =>
                {
                    TaxHead(h, "HSN/ SAC", "center", rowSpan: 2);
                    TaxHead(h, "Taxable amount" + cur, "center", rowSpan: 2);

                    if (split)
                    {
                        TaxHead(h, "CGST", "center", colSpan: 2);
                        TaxHead(h, "SGST", "center", colSpan: 2);
                    }
                    else
                    {
                        TaxHead(h, "IGST", "center", colSpan: 2);
                    }

                    TaxHead(h, "Total Tax Amount" + cur, "center", rowSpan: 2);

                    TaxHead(h, "Rate(%)", "center");
                    TaxHead(h, "Amount" + cur, "center");
                    if (split)
                    {
                        TaxHead(h, "Rate(%)", "center");
                        TaxHead(h, "Amount" + cur, "center");
                    }
                });

                foreach (var g in HsnGroups())
                {
                    TaxCell(table, g.Hsn, "left");
                    TaxCell(table, Money(g.Taxable), "right");

                    if (split)
                    {
                        TaxCell(table, QuestPdfEngine.Percent(g.Rate / 2m), "right");
                        TaxCell(table, Money(g.TaxAmount / 2m), "right");
                        TaxCell(table, QuestPdfEngine.Percent(g.Rate / 2m), "right");
                        TaxCell(table, Money(g.TaxAmount / 2m), "right");
                    }
                    else
                    {
                        TaxCell(table, QuestPdfEngine.Percent(g.Rate), "right");
                        TaxCell(table, Money(g.TaxAmount), "right");
                    }

                    TaxCell(table, Money(g.TaxAmount + g.Cess), "right");
                }

                table.Footer(f =>
                {
                    TaxFoot(f, "Total", "left");
                    TaxFoot(f, Money(Data.TotalTaxable), "right");

                    if (split)
                    {
                        TaxFoot(f, string.Empty, "right");
                        TaxFoot(f, Money(Data.TotalTax / 2m), "right");
                        TaxFoot(f, string.Empty, "right");
                        TaxFoot(f, Money(Data.TotalTax / 2m), "right");
                    }
                    else
                    {
                        TaxFoot(f, string.Empty, "right");
                        TaxFoot(f, Money(Data.TotalTax), "right");
                    }

                    TaxFoot(f, Money(Data.TotalTax + Data.TotalCess), "right");
                });
            });
        }

        /// <summary>GST Theme 6 - one rate column per rate, CGST and SGST as rows.</summary>
        private void ComposeTaxDetailsStrip(IContainer container)
        {
            var rates = HsnGroups().GroupBy(g => g.Rate).OrderBy(g => g.Key).ToList();

            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(12f);
                    foreach (var _ in rates) c.RelativeColumn(10f);
                });

                table.Header(h =>
                {
                    TaxHead(h, "Tax details", "left");
                    foreach (var r in rates)
                        TaxHead(h, QuestPdfEngine.Percent(Data.IsDomestic ? r.Key / 2m : r.Key), "right");
                });

                var components = Data.IsDomestic
                    ? new[] { "CGST", "SGST" }
                    : new[] { "IGST" };

                foreach (var component in components)
                {
                    TaxCell(table, component, "left");
                    foreach (var r in rates)
                    {
                        var amount = r.Sum(g => g.TaxAmount);
                        TaxCell(table, Money(Data.IsDomestic ? amount / 2m : amount), "right");
                    }
                }
            });
        }

        private void TaxHead(TableCellDescriptor h, string text, string align, uint rowSpan = 1, uint colSpan = 1)
        {
            var cell = h.Cell();
            if (rowSpan > 1) cell = cell.RowSpan(rowSpan);
            if (colSpan > 1) cell = cell.ColumnSpan(colSpan);

            if (Style.FilledLowerBars)
            {
                cell.Background(Accent).Border(Bw).BorderColor(Border).Padding(4)
                    .Element(c => Align(c, align)).AlignMiddle()
                    .Text(text).FontSize(Tiny).Bold().FontColor(AccentText);
            }
            else
            {
                cell.Border(Bw).BorderColor(Border).Padding(4)
                    .Element(c => Align(c, align)).AlignMiddle()
                    .Text(text).FontSize(Tiny).Bold();
            }
        }

        private void TaxCell(TableDescriptor t, string text, string align)
        {
            t.Cell().Border(Bw).BorderColor(Border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Tiny);
        }

        private void TaxFoot(TableCellDescriptor f, string text, string align)
        {
            f.Cell().Border(Bw).BorderColor(Border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Tiny).Bold();
        }

        // =================================================================
        //  Amounts
        // =================================================================
        private void ComposeAmountsBlock(IContainer container)
        {
            container.Column(col =>
            {
                Caption(col, "Amounts", Style.FilledLowerBars || Style.FilledTotalsCaption);
                ComposeAmountRows(col, AmountLine);
            });
        }

        private void AmountLine(ColumnDescriptor col, string label, string value, bool emphasise)
        {
            var item = col.Item().BorderBottom(0.4f).BorderColor(Border);
            if (emphasise) item = item.Background(TotalBg);

            item.Element(PadCell).Row(r =>
            {
                var l = r.RelativeItem(1.6f).Text(label).FontSize(Small);
                var v = r.RelativeItem(1f).AlignRight().Text(value).FontSize(Small);

                if (emphasise)
                {
                    l.Bold().FontColor(TotalText);
                    v.Bold().FontColor(TotalText);
                }
            });
        }

        // =================================================================
        //  Words / description / payment mode
        // =================================================================
        private void ComposeWordsAndDescription(ColumnDescriptor col)
        {
            col.Item().PaddingTop(Boxed ? 0f : 6f).Row(row =>
            {
                var left = row.RelativeItem();
                if (Boxed) left = left.BorderRight(Bw).BorderColor(Border);

                left.Column(c =>
                {
                    Caption(c, "Invoice Amount In Words", Style.FilledLowerBars);
                    c.Item().Element(PadCell)
                        .Text(QuestPdfEngine.AmountInWords(Data.GrandTotal, S.AmountInWordsFormat))
                        .FontSize(Small);
                });

                row.RelativeItem().Column(c =>
                {
                    // The folded layout already printed the description beside the
                    // amounts, so this half carries the terms instead.
                    if (Style.TaxBlock == TaxBlockKind.FoldedIntoTotals)
                    {
                        Caption(c, "Terms and conditions", Style.FilledLowerBars);
                        c.Item().Element(PadCell)
                            .Text(QuestPdfEngine.Dash(S.PrintTermsConditions ? S.DefaultTermsText : null))
                            .FontSize(Small);
                    }
                    else
                    {
                        Caption(c, "Description", Style.FilledLowerBars);
                        c.Item().Element(PadCell)
                            .Text(QuestPdfEngine.Dash(S.PrintDescription ? Data.Bill.Description : null))
                            .FontSize(Small);
                    }
                });
            });
        }

        private void ComposeWordsAndAmounts(ColumnDescriptor col)
        {
            col.Item().PaddingTop(Boxed ? 0f : 6f).Row(row =>
            {
                var left = row.RelativeItem();
                if (Boxed) left = left.BorderRight(Bw).BorderColor(Border);

                left.Column(c =>
                {
                    Caption(c, "Invoice Amount In Words", Style.FilledLowerBars);
                    c.Item().Element(PadCell).AlignCenter()
                        .Text(QuestPdfEngine.AmountInWords(Data.GrandTotal, S.AmountInWordsFormat))
                        .FontSize(Small);
                });

                row.RelativeItem().Element(ComposeAmountsBlock);
            });
        }

        private void ComposePaymentModeBlock(ColumnDescriptor col)
        {
            if (!S.PrintPaymentMode || string.IsNullOrWhiteSpace(Data.Bill.PaymentType)) return;

            col.Item().Column(c =>
            {
                Caption(c, "Payment Mode", Style.FilledLowerBars);
                c.Item().Element(PadCell).AlignCenter()
                    .Text(Data.Bill.PaymentType.Trim()).FontSize(Small);
            });
        }

        // =================================================================
        //  Bank | Terms | Signature
        // =================================================================
        private void ComposeClosingRow(ColumnDescriptor col)
        {
            bool bank = HasBank();
            bool terms = S.PrintTermsConditions && !string.IsNullOrWhiteSpace(S.DefaultTermsText);
            bool signature = S.PrintSignatureText || SignatureBytes != null;

            if (!bank && !terms && !signature) return;

            var forLine = "For : " + Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company");

            col.Item().BorderTop(Bw).BorderColor(Border).Row(row =>
            {
                CaptionCell(row.RelativeItem().BorderRight(Bw).BorderColor(Border), "Bank Details", Style.FilledLowerBars);
                CaptionCell(row.RelativeItem().BorderRight(Bw).BorderColor(Border), "Terms and conditions", Style.FilledLowerBars);
                CaptionCell(row.RelativeItem(), forLine, Style.FilledLowerBars);
            });

            col.Item().Row(row =>
            {
                row.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell)
                    .Column(ComposeBankLines);

                row.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell)
                    .Text(QuestPdfEngine.Dash(terms ? S.DefaultTermsText : null)).FontSize(Small);

                row.RelativeItem().Element(PadCell).Column(c =>
                {
                    if (SignatureBytes != null)
                        c.Item().AlignCenter().Height(44f).Image(SignatureBytes).FitArea();
                    else
                        c.Item().Height(30f);

                    if (S.PrintSignatureText)
                    {
                        c.Item().PaddingTop(3).AlignCenter()
                            .Text(Pick(S.SignatureText, "Authorized Signatory")).FontSize(Small).Bold();
                    }
                });
            });
        }
    }
}
