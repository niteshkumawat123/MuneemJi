using MUNEEMJI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Landscape Theme 1 and 2. Both are fully bordered grids with a centred
    /// title bar, a company band beside the invoice details, Bill To / Ship To,
    /// then the item grid. They differ below that: Theme 1 lays the totals out as
    /// two horizontal strips with the HSN grid beside the bank block, while
    /// Theme 2 runs the HSN grid on the left with the totals stacked on the right.
    /// </summary>
    public class QuestLandscapeDocument : QuestDocumentBase
    {
        private readonly bool _strips;   // Theme 1 uses the horizontal totals strips

        public QuestLandscapeDocument(QuestDocumentData data, IWebHostEnvironment env)
            : base(data, env)
        {
            _strips = string.Equals(Style.LayoutKey, "landscape1", StringComparison.OrdinalIgnoreCase);
        }

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

                    outer.Item().Border(Bw).BorderColor(Border).Column(box =>
                    {
                        // Title sits in its own full-width bordered bar.
                        box.Item().BorderBottom(Bw).BorderColor(Border)
                            .Element(PadCell).AlignCenter()
                            .Text(Data.DocumentTitle)
                            .FontSize(QuestPdfEngine.TitleSize(S.InvoiceTextSize)).Bold();

                        ComposeCompanyBand(box);
                        ComposePartyBand(box);

                        box.Item().Element(ComposeItemTable);

                        if (_strips)
                        {
                            ComposeTotalStrips(box);
                            ComposeTaxAndBank(box);
                            ComposeClosingRow(box, threeColumns: true);
                        }
                        else
                        {
                            ComposeTaxAndTotals(box);
                            ComposeClosingRow(box, threeColumns: false);
                            ComposeBankRow(box);
                        }
                    });
                });
            });
        }

        // =================================================================
        //  Company band + invoice details
        // =================================================================
        private void ComposeCompanyBand(ColumnDescriptor box)
        {
            box.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem(2.6f).Element(PadCell).Row(inner =>
                {
                    if (LogoBytes != null)
                    {
                        inner.ConstantItem(72f).AlignMiddle().Height(42f).Image(LogoBytes).FitArea();
                        inner.ConstantItem(8f);
                    }

                    inner.RelativeItem().Column(c =>
                    {
                        if (S.PrintCompanyName)
                        {
                            c.Item().Text(Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company Name"))
                                .FontSize(QuestPdfEngine.CompanyNameSize(S.CompanyNameTextSize)).Bold();
                        }

                        if (S.PrintAddress)
                        {
                            var address = Pick(S.AddressText, BuildCompanyAddress());
                            if (!string.IsNullOrWhiteSpace(address))
                                c.Item().Text(address).FontSize(Small);
                        }

                        c.Item().PaddingTop(1).Row(r =>
                        {
                            if (S.PrintPhone)
                            {
                                var phone = Pick(S.PhoneText, Data.Company?.PhoneNumber);
                                if (!string.IsNullOrWhiteSpace(phone))
                                    r.RelativeItem().Text("Phone: " + phone).FontSize(Small).Bold().FontColor(Accent);
                            }

                            if (S.PrintGstin)
                            {
                                var gstin = Pick(S.GstinText, Data.Company?.Gstin);
                                if (!string.IsNullOrWhiteSpace(gstin))
                                    r.RelativeItem().Text("GSTIN : " + gstin).FontSize(Small).Bold();
                            }
                        });

                        c.Item().PaddingTop(1).Row(r =>
                        {
                            if (S.PrintEmail)
                            {
                                var email = Pick(S.EmailText, Data.Company?.Email);
                                if (!string.IsNullOrWhiteSpace(email))
                                    r.RelativeItem().Text("Email: " + email).FontSize(Small).Bold().FontColor(Accent);
                            }

                            if (S.PrintState)
                            {
                                var state = BuildStateLine();
                                if (!string.IsNullOrWhiteSpace(state))
                                    r.RelativeItem().Text("State : " + state).FontSize(Small).Bold();
                            }
                        });
                    });
                });

                row.RelativeItem(1f).BorderLeft(Bw).BorderColor(Border).Element(PadCell).Column(c =>
                {
                    c.Item().Text(NumberLabel() + ": " + DocumentNumber()).FontSize(Small);
                    c.Item().PaddingTop(2).Text("Date: " + DocumentDate()).FontSize(Small);

                    if (Data.Bill.Time.HasValue && Data.Bill.Time.Value != TimeSpan.MinValue)
                        c.Item().PaddingTop(2).Text("Time: " + QuestPdfEngine.TimeOrDash(Data.Bill.Time)).FontSize(Small);

                    if (Data.Bill.DueDate != DateTime.MinValue)
                        c.Item().PaddingTop(2).Text("Due Date: " + QuestPdfEngine.DateOrDash(Data.Bill.DueDate)).FontSize(Small);
                });
            });
        }

        // =================================================================
        //  Bill To | Ship To
        // =================================================================
        private void ComposePartyBand(ColumnDescriptor box)
        {
            box.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell).Column(c =>
                {
                    c.Item().Text("Bill To:").FontSize(Small).Bold();
                    c.Item().PaddingTop(2).Text(PartyName()).FontSize(Small).FontColor(Accent);

                    var address = BillAddress();
                    if (!string.IsNullOrWhiteSpace(address))
                        c.Item().PaddingTop(2).Text(address).FontSize(Small);

                    var phone = PartyPhone();
                    if (!string.IsNullOrWhiteSpace(phone))
                        c.Item().PaddingTop(2).Text("Contact No.: " + phone).FontSize(Small).FontColor(Accent);

                    if (!string.IsNullOrWhiteSpace(Data.Party?.GSTIN))
                        c.Item().PaddingTop(2).Text("GSTIN: " + Data.Party.GSTIN).FontSize(Small);
                });

                row.RelativeItem().Element(PadCell).Column(c =>
                {
                    c.Item().Text("Ship To:").FontSize(Small).Bold();
                    c.Item().PaddingTop(2).Text(QuestPdfEngine.Dash(ShipAddress())).FontSize(Small);
                });
            });
        }

        // =================================================================
        //  Item grid
        // =================================================================
        private void ComposeItemTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3f);     // #
                    c.RelativeColumn(24f);    // Item name
                    c.RelativeColumn(11f);    // HSC/SAC
                    c.RelativeColumn(10f);    // Quantity
                    c.RelativeColumn(11f);    // Price/unit
                    c.RelativeColumn(12f);    // Discount
                    c.RelativeColumn(11f);    // GST
                    c.RelativeColumn(11f);    // Amount
                });

                table.Header(h =>
                {
                    Head(h, "#", "left");
                    Head(h, "Item name", "left");
                    Head(h, "HSC/SAC", "right");
                    Head(h, "Quantity", "right");
                    Head(h, "Price/unit", "right");
                    Head(h, "Discount", "right");
                    Head(h, "GST", "right");
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
                    Cell(table, Plain(item.PricePerUnit), "right");
                    Cell(table, Plain(item.DiscountAmount) + " (" + QuestPdfEngine.Percent(item.DiscountPercentage) + ")", "right");
                    Cell(table, Plain(item.TaxAmount) + " (" + QuestPdfEngine.Percent(item.TaxPercentage) + ")", "right");
                    Cell(table, Plain(item.TotalAmount ?? (lineTaxable + item.TaxAmount)), "right");
                }

                for (int blank = Data.Items.Count; blank < S.MinItemRows; blank++)
                    for (int c = 0; c < 8; c++)
                        table.Cell().Border(Bw).BorderColor(Border).MinHeight(13f).Padding(4).Text(string.Empty);

                table.Footer(f =>
                {
                    var totalFree = Data.Items.Sum(i => i.FreeQuantity ?? 0m);
                    var totalQty = QuestPdfEngine.Qty(Data.TotalQuantity)
                                   + (totalFree > 0 ? " + " + QuestPdfEngine.Qty(totalFree) : string.Empty);

                    Foot(f, string.Empty, "left");
                    Foot(f, "TOTAL", "left");
                    Foot(f, string.Empty, "right");
                    Foot(f, S.PrintTotalItemQuantity ? totalQty : string.Empty, "right");
                    Foot(f, string.Empty, "right");
                    Foot(f, Plain(Data.TotalDiscount), "right");
                    Foot(f, Plain(Data.TotalTax), "right");
                    Foot(f, Plain(Data.TotalTaxable + Data.TotalTax + Data.TotalCess), "right");
                });

                if (S.ExpandItemTable)
                    table.ExtendLastCellsToTableBottom();
            });
        }

        private void Head(TableCellDescriptor h, string text, string align)
        {
            h.Cell().Border(Bw).BorderColor(Border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold();
        }

        private void Cell(TableDescriptor t, string text, string align, Color? color = null, bool bold = false)
        {
            var span = t.Cell().Border(Bw).BorderColor(Border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small);

            if (bold) span.Bold();
            if (color.HasValue) span.FontColor(color.Value);
        }

        private void Foot(TableCellDescriptor f, string text, string align)
        {
            f.Cell().Border(Bw).BorderColor(Border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold();
        }

        // =================================================================
        //  Theme 1: horizontal totals strips
        // =================================================================
        private void ComposeTotalStrips(ColumnDescriptor box)
        {
            var bill = Data.Bill;

            box.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                StripCell(row, "Sub Total:", Plain(Data.TotalTaxable + Data.TotalDiscount), 1f, true);
                StripCell(row, "Discount" + PercentSuffix(EffectiveDiscountRate()) + ":", Plain(Data.TotalDiscount), 1.15f, true);
                StripCell(row, "Tax" + PercentSuffix(EffectiveTaxRate()) + ":", Plain(Data.TotalTax), 1f, true);
                StripCell(row, TcsTdsLabel() + PercentSuffix(bill.TdsTcsPercentage) + ":", Plain(bill.TdsTcsAmount), 1f, true);

                row.RelativeItem(2.2f).Element(PadCell).Text(t =>
                {
                    t.Span("Total: ").FontSize(Small).Bold();
                    t.Span(Money(Data.GrandTotal)).FontSize(Small).Bold();
                    t.Span(" (" + QuestPdfEngine.AmountInWords(Data.GrandTotal, S.AmountInWordsFormat) + ")")
                        .FontSize(Small).Bold();
                });
            });

            box.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                if (S.PrintReceivedAmount)
                    StripCell(row, "Received:", Plain(bill.paidReciveamount), 1f, false);

                if (S.PrintBalanceAmount)
                    StripCell(row, "Balance:", Plain(Data.GrandTotal - bill.paidReciveamount), 1f, false);

                if (S.PrintCurrentBalanceParty)
                    StripCell(row, "Current Balance:", Plain(Data.PartyCurrentBalance), 1.2f, false);

                if (S.PrintYouSaved)
                    StripCell(row, "You Saved:", Plain(Data.TotalDiscount), 1f, false);
            });
        }

        private void StripCell(RowDescriptor row, string label, string value, float weight, bool divider)
        {
            var cell = row.RelativeItem(weight);
            if (divider) cell = cell.BorderRight(Bw).BorderColor(Border);

            cell.Element(PadCell).Text(t =>
            {
                t.Span(label + " ").FontSize(Small);
                t.Span(value).FontSize(Small).Bold().FontColor(Accent);
            });
        }

        /// <summary>Theme 1: HSN grid on the left, payment mode and bank on the right.</summary>
        private void ComposeTaxAndBank(ColumnDescriptor box)
        {
            box.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem(1.4f).BorderRight(Bw).BorderColor(Border).Element(ComposeHsnGrid);

                row.RelativeItem(1f).Column(c =>
                {
                    if (S.PrintPaymentMode && !string.IsNullOrWhiteSpace(Data.Bill.PaymentType))
                    {
                        c.Item().BorderBottom(Bw).BorderColor(Border).Element(PadCell)
                            .Text("Payment Mode: " + Data.Bill.PaymentType.Trim()).FontSize(Small);
                    }

                    if (HasBank())
                        c.Item().Element(PadCell).Column(ComposeBankLines);
                });
            });
        }

        // =================================================================
        //  Theme 2: HSN grid beside the stacked totals
        // =================================================================
        private void ComposeTaxAndTotals(ColumnDescriptor box)
        {
            box.Item().BorderBottom(Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem(1.5f).BorderRight(Bw).BorderColor(Border).Column(c =>
                {
                    c.Item().Element(ComposeHsnGrid);

                    if (S.PrintPaymentMode && !string.IsNullOrWhiteSpace(Data.Bill.PaymentType))
                    {
                        c.Item().BorderTop(Bw).BorderColor(Border).Element(PadCell)
                            .Text("Payment Mode: " + Data.Bill.PaymentType.Trim()).FontSize(Small);
                    }
                });

                row.RelativeItem(1f).Column(c =>
                {
                    ComposeAmountRows(c, TotalLine);

                    c.Item().BorderBottom(0.4f).BorderColor(Border).Element(PadCell)
                        .Text("Invoice Amount In Words :").FontSize(Small);

                    c.Item().BorderBottom(0.4f).BorderColor(Border).Element(PadCell)
                        .Text(QuestPdfEngine.AmountInWords(Data.GrandTotal, S.AmountInWordsFormat))
                        .FontSize(Small).FontColor(Accent);
                });
            });
        }

        private void TotalLine(ColumnDescriptor col, string label, string value, bool emphasise)
        {
            var item = col.Item().BorderBottom(0.4f).BorderColor(Border);
            if (emphasise) item = item.Background(TotalBg);

            item.Element(PadCell).Row(r =>
            {
                var l = r.RelativeItem(1.5f).Text(label).FontSize(Small);
                r.ConstantItem(10f).Text(emphasise ? string.Empty : ":").FontSize(Small);
                var v = r.RelativeItem(1.1f).AlignRight().Text(value).FontSize(Small);

                if (emphasise)
                {
                    l.Bold().FontColor(TotalText);
                    v.Bold().FontColor(TotalText);
                }
            });
        }

        // =================================================================
        //  HSN grid
        // =================================================================
        private void ComposeHsnGrid(IContainer container)
        {
            bool split = Data.IsDomestic;

            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(12f);
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

                var cur = "(" + QuestPdfEngine.Rupee + ")";

                table.Header(h =>
                {
                    GridHead(h, "HSN/ SAC", rowSpan: 2);
                    GridHead(h, "Taxable amount" + cur, rowSpan: 2);

                    if (split)
                    {
                        GridHead(h, "CGST", colSpan: 2);
                        GridHead(h, "SGST", colSpan: 2);
                    }
                    else
                    {
                        GridHead(h, "IGST", colSpan: 2);
                    }

                    GridHead(h, "Total Tax Amount" + cur, rowSpan: 2);

                    GridHead(h, "Rate(%)");
                    GridHead(h, "Amount" + cur);
                    if (split)
                    {
                        GridHead(h, "Rate(%)");
                        GridHead(h, "Amount" + cur);
                    }
                });

                foreach (var g in HsnGroups())
                {
                    GridCell(table, g.Hsn, "left");
                    GridCell(table, Plain(g.Taxable), "right");

                    if (split)
                    {
                        GridCell(table, QuestPdfEngine.Percent(g.Rate / 2m), "right");
                        GridCell(table, Plain(g.TaxAmount / 2m), "right");
                        GridCell(table, QuestPdfEngine.Percent(g.Rate / 2m), "right");
                        GridCell(table, Plain(g.TaxAmount / 2m), "right");
                    }
                    else
                    {
                        GridCell(table, QuestPdfEngine.Percent(g.Rate), "right");
                        GridCell(table, Plain(g.TaxAmount), "right");
                    }

                    GridCell(table, Plain(g.TaxAmount + g.Cess), "right");
                }

                table.Footer(f =>
                {
                    GridFoot(f, "TOTAL", "left");
                    GridFoot(f, Plain(Data.TotalTaxable), "right");

                    if (split)
                    {
                        GridFoot(f, string.Empty, "right");
                        GridFoot(f, Plain(Data.TotalTax / 2m), "right");
                        GridFoot(f, string.Empty, "right");
                        GridFoot(f, Plain(Data.TotalTax / 2m), "right");
                    }
                    else
                    {
                        GridFoot(f, string.Empty, "right");
                        GridFoot(f, Plain(Data.TotalTax), "right");
                    }

                    GridFoot(f, Plain(Data.TotalTax + Data.TotalCess), "right");
                });
            });
        }

        private void GridHead(TableCellDescriptor h, string text, uint rowSpan = 1, uint colSpan = 1)
        {
            var cell = h.Cell();
            if (rowSpan > 1) cell = cell.RowSpan(rowSpan);
            if (colSpan > 1) cell = cell.ColumnSpan(colSpan);

            cell.Border(Bw).BorderColor(Border).Padding(3)
                .AlignCenter().AlignMiddle()
                .Text(text).FontSize(Tiny).Bold();
        }

        private void GridCell(TableDescriptor t, string text, string align)
        {
            t.Cell().Border(Bw).BorderColor(Border).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Tiny);
        }

        private void GridFoot(TableCellDescriptor f, string text, string align)
        {
            f.Cell().Border(Bw).BorderColor(Border).Padding(3)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Tiny).Bold();
        }

        // =================================================================
        //  Description | Terms | (Signature)
        // =================================================================
        private void ComposeClosingRow(ColumnDescriptor box, bool threeColumns)
        {
            var description = S.PrintDescription ? Data.Bill.Description : null;
            var terms = S.PrintTermsConditions ? S.DefaultTermsText : null;

            box.Item().BorderBottom(threeColumns ? 0f : Bw).BorderColor(Border).Row(row =>
            {
                row.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell).Column(c =>
                {
                    c.Item().Text("Description:").FontSize(Small).Bold();
                    c.Item().PaddingTop(2).Text(QuestPdfEngine.Dash(description)).FontSize(Small).FontColor(Accent);
                });

                var mid = row.RelativeItem();
                if (threeColumns) mid = mid.BorderRight(Bw).BorderColor(Border);

                mid.Element(PadCell).Column(c =>
                {
                    c.Item().Text("Terms & Conditions:").FontSize(Small).Bold();
                    c.Item().PaddingTop(2).Text(QuestPdfEngine.Dash(terms)).FontSize(Small).FontColor(Accent);
                });

                if (threeColumns)
                {
                    row.RelativeItem().Element(PadCell).Column(c =>
                    {
                        c.Item().Text("For: " + Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company") + ":")
                            .FontSize(Small).Bold();

                        if (SignatureBytes != null)
                            c.Item().PaddingTop(3).AlignCenter().Height(42f).Image(SignatureBytes).FitArea();
                        else
                            c.Item().Height(28f);

                        if (S.PrintSignatureText)
                        {
                            c.Item().PaddingTop(2).AlignCenter()
                                .Text(Pick(S.SignatureText, "Authorized Signatory")).FontSize(Small).Bold();
                        }
                    });
                }
            });
        }

        /// <summary>Theme 2 closes with Bank Details beside the signature.</summary>
        private void ComposeBankRow(ColumnDescriptor box)
        {
            bool bank = HasBank();
            bool signature = S.PrintSignatureText || SignatureBytes != null;
            if (!bank && !signature) return;

            box.Item().Row(row =>
            {
                row.RelativeItem().BorderRight(Bw).BorderColor(Border).Element(PadCell).Column(c =>
                {
                    c.Item().Text("Bank Details:").FontSize(Small).Bold();
                    c.Item().PaddingTop(2).Column(ComposeBankLines);
                });

                row.RelativeItem().Element(PadCell).Column(c =>
                {
                    c.Item().Text("For: " + Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company") + ":")
                        .FontSize(Small).Bold();

                    if (SignatureBytes != null)
                        c.Item().PaddingTop(3).AlignCenter().Height(42f).Image(SignatureBytes).FitArea();
                    else
                        c.Item().Height(28f);

                    if (S.PrintSignatureText)
                    {
                        c.Item().PaddingTop(2).AlignCenter()
                            .Text(Pick(S.SignatureText, "Authorized Signatory")).FontSize(Small).Bold();
                    }
                });
            });
        }
    }
}
