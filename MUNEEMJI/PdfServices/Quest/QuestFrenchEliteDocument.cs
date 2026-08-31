using MUNEEMJI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// French Elite: a filled title block on the left with the logo opposite,
    /// the company name and a three-column contact strip beneath it, an accent
    /// rule, then Invoice / Bill To / Transportation in three columns, a filled
    /// item grid and the bank block beside the totals.
    /// </summary>
    public class QuestFrenchEliteDocument : QuestDocumentBase
    {
        public QuestFrenchEliteDocument(QuestDocumentData data, IWebHostEnvironment env)
            : base(data, env)
        {
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

                page.Content().Column(col =>
                {
                    if (!string.IsNullOrWhiteSpace(copyLabel))
                    {
                        col.Item().AlignRight().PaddingBottom(6)
                            .Text(copyLabel).FontSize(8f).Bold().FontColor(Accent);
                    }

                    ComposeTitleBlock(col);
                    ComposeCompanyStrip(col);

                    col.Item().PaddingVertical(6)
                        .LineHorizontal(0.9f).LineColor(Accent);

                    ComposeInfoColumns(col);

                    col.Item().PaddingTop(6).Element(ComposeItemTable);

                    ComposeFooterRow(col);
                });
            });
        }

        // =================================================================
        //  Filled title block, logo opposite
        // =================================================================
        private void ComposeTitleBlock(ColumnDescriptor col)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem(1.2f).Background(Accent).PaddingVertical(12).PaddingHorizontal(18)
                    .Text(Data.DocumentTitle.ToUpperInvariant())
                    .FontSize(QuestPdfEngine.TitleSize(S.InvoiceTextSize) + 6f)
                    .Bold().FontColor(AccentText);

                row.RelativeItem(1f).AlignRight().AlignMiddle().Column(c =>
                {
                    if (LogoBytes != null)
                        c.Item().AlignRight().Height(40f).Image(LogoBytes).FitArea();
                });
            });
        }

        // =================================================================
        //  Company name + three-column contact strip
        // =================================================================
        private void ComposeCompanyStrip(ColumnDescriptor col)
        {
            if (S.PrintCompanyName)
            {
                col.Item().PaddingTop(10)
                    .Text(Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company Name"))
                    .FontSize(QuestPdfEngine.CompanyNameSize(S.CompanyNameTextSize) - 2f)
                    .Bold().FontColor(Accent);
            }

            col.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    if (S.PrintAddress)
                    {
                        var address = Pick(S.AddressText, BuildCompanyAddress());
                        if (!string.IsNullOrWhiteSpace(address))
                        {
                            c.Item().Text("Address:").FontSize(Tiny).Bold().FontColor(Accent);
                            c.Item().PaddingTop(2).Text(address).FontSize(Small);
                        }
                    }
                });

                row.RelativeItem().Column(c =>
                {
                    if (S.PrintPhone)
                    {
                        var phone = Pick(S.PhoneText, Data.Company?.PhoneNumber);
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            c.Item().Text("Phone:").FontSize(Tiny).Bold().FontColor(Accent);
                            c.Item().PaddingTop(2).Text(phone).FontSize(Small);
                        }
                    }

                    if (S.PrintEmail)
                    {
                        var email = Pick(S.EmailText, Data.Company?.Email);
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            c.Item().PaddingTop(6).Text("Email:").FontSize(Tiny).Bold().FontColor(Accent);
                            c.Item().PaddingTop(2).Text(email).FontSize(Small);
                        }
                    }
                });

                row.RelativeItem().Column(c =>
                {
                    if (S.PrintGstin)
                    {
                        var gstin = Pick(S.GstinText, Data.Company?.Gstin);
                        if (!string.IsNullOrWhiteSpace(gstin))
                        {
                            c.Item().Text("GSTIN:").FontSize(Tiny).Bold().FontColor(Accent);
                            c.Item().PaddingTop(2).Text(gstin).FontSize(Small);
                        }
                    }

                    if (S.PrintState)
                    {
                        var state = BuildStateLine();
                        if (!string.IsNullOrWhiteSpace(state))
                        {
                            c.Item().PaddingTop(6).Text("State:").FontSize(Tiny).Bold().FontColor(Accent);
                            c.Item().PaddingTop(2).Text(state).FontSize(Small);
                        }
                    }
                });
            });
        }

        // =================================================================
        //  Invoice | Bill To | Transportation
        // =================================================================
        private void ComposeInfoColumns(ColumnDescriptor col)
        {
            var bill = Data.Bill;

            col.Item().Row(row =>
            {
                row.RelativeItem(1f).Column(c =>
                {
                    c.Item().Text(NumberLabel() + ": " + DocumentNumber())
                        .FontSize(Body).Bold().FontColor(Accent);

                    FieldPair(c, "Invoice Date:", DocumentDate());

                    if (bill.Time.HasValue && bill.Time.Value != TimeSpan.MinValue)
                        FieldPair(c, "Invoice Time:", QuestPdfEngine.TimeOrDash(bill.Time));

                    if (!string.IsNullOrWhiteSpace(bill.StateOfSupply))
                        FieldPair(c, "Place of Supply:", bill.StateOfSupply);

                    if (bill.PODate.HasValue && bill.PODate.Value != DateTime.MinValue)
                        FieldPair(c, "PO date:", QuestPdfEngine.DateOrDash(bill.PODate));
                });

                row.RelativeItem(1.2f).Column(c =>
                {
                    c.Item().Text("Bill To:").FontSize(Body).Bold().FontColor(Accent);
                    c.Item().PaddingTop(4).Text(PartyName()).FontSize(Small).Bold();

                    var address = BillAddress();
                    if (!string.IsNullOrWhiteSpace(address))
                        c.Item().PaddingTop(3).Text(address).FontSize(Small);

                    var phone = PartyPhone();
                    if (!string.IsNullOrWhiteSpace(phone))
                        FieldPair(c, "Contact Number:", phone);

                    if (!string.IsNullOrWhiteSpace(Data.Party?.GSTIN))
                        FieldPair(c, "GSTIN Number:", Data.Party.GSTIN);

                    if (!string.IsNullOrWhiteSpace(Data.Party?.StateName))
                        FieldPair(c, "State:", Data.Party.StateName);
                });

                row.RelativeItem(1.1f).Column(c =>
                {
                    c.Item().Text("Transportation Details:").FontSize(Body).Bold().FontColor(Accent);

                    if (!string.IsNullOrWhiteSpace(bill.TransportName))
                        FieldPair(c, "Transport Name:", bill.TransportName);

                    if (!string.IsNullOrWhiteSpace(bill.VehicleNumber))
                        FieldPair(c, "Vehicle Number:", bill.VehicleNumber);

                    if (bill.DeliveryDate.HasValue && bill.DeliveryDate.Value != DateTime.MinValue)
                        FieldPair(c, "Delivery Date:", QuestPdfEngine.DateOrDash(bill.DeliveryDate));
                });
            });
        }

        private void FieldPair(ColumnDescriptor c, string label, string value)
        {
            c.Item().PaddingTop(4).Row(r =>
            {
                r.RelativeItem(1.1f).Text(label).FontSize(Small);
                r.RelativeItem(1.1f).Text(QuestPdfEngine.Dash(value)).FontSize(Small);
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
                    c.RelativeColumn(3f);
                    c.RelativeColumn(22f);
                    c.RelativeColumn(10f);
                    c.RelativeColumn(10f);
                    c.RelativeColumn(11f);
                    c.RelativeColumn(12f);
                    c.RelativeColumn(12f);
                    c.RelativeColumn(13f);
                });

                table.Header(h =>
                {
                    Head(h, "#", "left");
                    Head(h, "Item name", "left");
                    Head(h, "HSN/ SAC", "left");
                    Head(h, "Quantity", "right");
                    Head(h, "Price/ unit", "right");
                    Head(h, "Discount", "right");
                    Head(h, "GST", "right");
                    Head(h, "Amount", "right");
                });

                int index = 0;
                foreach (var item in Data.Items)
                {
                    index++;

                    var free = item.FreeQuantity ?? 0m;
                    var qty = QuestPdfEngine.Qty(item.Quantity) + (free > 0 ? " + " + QuestPdfEngine.Qty(free) : string.Empty);
                    var lineTaxable = item.Quantity * item.PricePerUnit - item.DiscountAmount;

                    Cell(table, index.ToString(), "left");
                    Cell(table, QuestPdfEngine.Dash(item.Item), "left", true);
                    Cell(table, QuestPdfEngine.Dash(item.HSNCode), "left");
                    Cell(table, qty, "right");
                    Cell(table, Money(item.PricePerUnit), "right");
                    Cell(table, Money(item.DiscountAmount) + " (" + QuestPdfEngine.Percent(item.DiscountPercentage) + ")", "right");
                    Cell(table, Money(item.TaxAmount) + " (" + QuestPdfEngine.Percent(item.TaxPercentage) + ")", "right");
                    Cell(table, Money(item.TotalAmount ?? (lineTaxable + item.TaxAmount)), "right");
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
                    Foot(f, "Total", "left");
                    Foot(f, string.Empty, "left");
                    Foot(f, S.PrintTotalItemQuantity ? totalQty : string.Empty, "right");
                    Foot(f, string.Empty, "right");
                    Foot(f, Money(Data.TotalDiscount), "right");
                    Foot(f, Money(Data.TotalTax), "right");
                    Foot(f, Money(Data.TotalTaxable + Data.TotalTax + Data.TotalCess), "right");
                });

                if (S.ExpandItemTable)
                    table.ExtendLastCellsToTableBottom();
            });
        }

        private void Head(TableCellDescriptor h, string text, string align)
        {
            h.Cell().Background(Accent).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold().FontColor(AccentText);
        }

        private void Cell(TableDescriptor t, string text, string align, bool bold = false)
        {
            var span = t.Cell().Border(Bw).BorderColor(Border).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small);

            if (bold) span.Bold();
        }

        private void Foot(TableCellDescriptor f, string text, string align)
        {
            f.Cell().Background(Accent).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold().FontColor(AccentText);
        }

        // =================================================================
        //  Pay To | totals
        // =================================================================
        private void ComposeFooterRow(ColumnDescriptor col)
        {
            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem(1.2f).Column(c =>
                {
                    if (HasBank())
                    {
                        c.Item().Text("Pay To:").FontSize(Body).Bold().FontColor(Accent);
                        c.Item().PaddingTop(3).Column(ComposeBankLines);
                    }

                    c.Item().PaddingTop(6).Text("Invoice Amount In Words:").FontSize(Small).Bold().FontColor(Accent);
                    c.Item().PaddingTop(2)
                        .Text(QuestPdfEngine.AmountInWords(Data.GrandTotal, S.AmountInWordsFormat)).FontSize(Small);

                    if (S.PrintDescription && !string.IsNullOrWhiteSpace(Data.Bill.Description))
                    {
                        c.Item().PaddingTop(6).Text("Description:").FontSize(Small).Bold().FontColor(Accent);
                        c.Item().PaddingTop(2).Text(Data.Bill.Description.Trim()).FontSize(Small);
                    }

                    if (S.PrintTermsConditions && !string.IsNullOrWhiteSpace(S.DefaultTermsText))
                    {
                        c.Item().PaddingTop(6).Text("Terms and conditions:").FontSize(Small).Bold().FontColor(Accent);
                        c.Item().PaddingTop(2).Text(S.DefaultTermsText.Trim()).FontSize(Small);
                    }

                    if (S.PrintPaymentMode && !string.IsNullOrWhiteSpace(Data.Bill.PaymentType))
                    {
                        c.Item().PaddingTop(6).Text("Payment Mode: " + Data.Bill.PaymentType.Trim())
                            .FontSize(Small).Bold();
                    }
                });

                row.ConstantItem(10f);

                row.RelativeItem(1f).Column(c =>
                {
                    c.Item().Border(Bw).BorderColor(Border).Column(totals =>
                    {
                        ComposeAmountRows(totals, TotalLine);
                    });

                    c.Item().PaddingTop(8).Column(ComposeSignature);
                });
            });
        }

        private void TotalLine(ColumnDescriptor col, string label, string value, bool emphasise)
        {
            var item = col.Item().BorderBottom(0.4f).BorderColor(Border);
            if (emphasise) item = item.Background(Accent);

            item.Element(PadCell).Row(r =>
            {
                var l = r.RelativeItem(1.5f).Text(label).FontSize(Small);
                var v = r.RelativeItem(1f).AlignRight().Text(value).FontSize(Small);

                if (emphasise)
                {
                    l.Bold().FontColor(AccentText);
                    v.Bold().FontColor(AccentText);
                }
            });
        }
    }
}
