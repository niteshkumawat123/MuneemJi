using MUNEEMJI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Double Divine: a dark header slab with a lighter contact banner laid over
    /// it, the logo in a white card, the document title set large on the right,
    /// then a three-column Bill To / Transportation / Invoice band, a filled item
    /// grid, and a closing row of bank details beside the totals panel.
    /// </summary>
    public class QuestDoubleDivineDocument : QuestDocumentBase
    {
        private readonly Color _slab;      // dark header
        private readonly Color _bannerBg;  // lighter contact strip
        private readonly Color _slabText;

        public QuestDoubleDivineDocument(QuestDocumentData data, IWebHostEnvironment env)
            : base(data, env)
        {
            // The dark slab is a deepened accent; the banner a lightened one, so
            // the two-tone look follows whichever colour pair is picked.
            _slab = QuestPdfEngine.Shade(S.EffectivePrimaryColor, -0.45f);
            _bannerBg = QuestPdfEngine.Shade(S.EffectivePrimaryColor, 0.45f);
            _slabText = QuestPdfEngine.ParseColor("#FFFFFF", "#FFFFFF");
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
                        col.Item().AlignRight().PaddingBottom(4)
                            .Text(copyLabel).FontSize(8f).Bold().FontColor(Accent);
                    }

                    ComposeHeaderSlab(col);
                    ComposeTitleRow(col);
                    ComposePartyBand(col);

                    col.Item().PaddingTop(6).Element(ComposeItemTable);

                    ComposeFooterRow(col);
                });
            });
        }

        // =================================================================
        //  Header slab: logo card + contact banner
        // =================================================================
        private void ComposeHeaderSlab(ColumnDescriptor col)
        {
            col.Item().Background(_slab).Padding(10).Row(row =>
            {
                if (LogoBytes != null)
                {
                    row.ConstantItem(84f).Background(Colors.White).Padding(6)
                        .Height(52f).Image(LogoBytes).FitArea();
                    row.ConstantItem(10f);
                }

                row.RelativeItem().Background(_bannerBg).Padding(8).Row(banner =>
                {
                    if (S.PrintPhone)
                    {
                        var phone = Pick(S.PhoneText, Data.Company?.PhoneNumber);
                        if (!string.IsNullOrWhiteSpace(phone))
                            BannerField(banner, phone);
                    }

                    if (S.PrintEmail)
                    {
                        var email = Pick(S.EmailText, Data.Company?.Email);
                        if (!string.IsNullOrWhiteSpace(email))
                            BannerField(banner, email);
                    }

                    if (S.PrintAddress)
                    {
                        var address = Pick(S.AddressText, BuildCompanyAddress());
                        if (!string.IsNullOrWhiteSpace(address))
                            BannerField(banner, address);
                    }
                });
            });

            // Company name and identifiers sit under the slab, on the slab colour.
            col.Item().Background(_slab).PaddingHorizontal(10).PaddingBottom(10).Column(c =>
            {
                if (S.PrintCompanyName)
                {
                    c.Item().Text(Pick(S.CompanyNameText, Data.Company?.BusinessName, "Company Name"))
                        .FontSize(QuestPdfEngine.CompanyNameSize(S.CompanyNameTextSize))
                        .Bold().FontColor(_slabText);
                }

                if (S.PrintGstin)
                {
                    var gstin = Pick(S.GstinText, Data.Company?.Gstin);
                    if (!string.IsNullOrWhiteSpace(gstin))
                        c.Item().PaddingTop(2).Text("GSTIN : " + gstin).FontSize(Small).FontColor(_slabText);
                }

                if (S.PrintState)
                {
                    var state = BuildStateLine();
                    if (!string.IsNullOrWhiteSpace(state))
                        c.Item().PaddingTop(1).Text("State : " + state).FontSize(Small).FontColor(_slabText);
                }
            });
        }

        private void BannerField(RowDescriptor banner, string value)
        {
            banner.RelativeItem().PaddingRight(6)
                .Text(value).FontSize(Tiny).FontColor(_slabText);
        }

        // =================================================================
        //  Title
        // =================================================================
        private void ComposeTitleRow(ColumnDescriptor col)
        {
            col.Item().PaddingTop(8).PaddingBottom(4).AlignRight()
                .Text(Data.DocumentTitle)
                .FontSize(QuestPdfEngine.TitleSize(S.InvoiceTextSize) + 6f)
                .Bold().FontColor(_slab);
        }

        // =================================================================
        //  Bill To | Transportation | Invoice details
        // =================================================================
        private void ComposePartyBand(ColumnDescriptor col)
        {
            var bill = Data.Bill;

            col.Item().PaddingBottom(6).Row(row =>
            {
                row.RelativeItem(1.15f).Column(c =>
                {
                    c.Item().Text("Bill To:").FontSize(Body).Bold().FontColor(Accent);
                    c.Item().PaddingTop(3).Text(PartyName()).FontSize(Small).Bold();

                    var address = BillAddress();
                    if (!string.IsNullOrWhiteSpace(address))
                        c.Item().PaddingTop(2).Text(address).FontSize(Small);

                    var phone = PartyPhone();
                    if (!string.IsNullOrWhiteSpace(phone))
                        FieldPair(c, "Contact No.:", phone);

                    if (!string.IsNullOrWhiteSpace(Data.Party?.GSTIN))
                        FieldPair(c, "GSTIN Number:", Data.Party.GSTIN);

                    if (!string.IsNullOrWhiteSpace(Data.Party?.StateName))
                        FieldPair(c, "State:", Data.Party.StateName);
                });

                row.RelativeItem(1.15f).Column(c =>
                {
                    c.Item().Text("Transportation Details:").FontSize(Body).Bold().FontColor(Accent);

                    if (!string.IsNullOrWhiteSpace(bill.TransportName))
                        FieldPair(c, "Transport Name:", bill.TransportName);

                    if (!string.IsNullOrWhiteSpace(bill.VehicleNumber))
                        FieldPair(c, "Vehicle Number:", bill.VehicleNumber);

                    if (bill.DeliveryDate.HasValue && bill.DeliveryDate.Value != DateTime.MinValue)
                        FieldPair(c, "Delivery Date:", QuestPdfEngine.DateOrDash(bill.DeliveryDate));
                });

                row.RelativeItem(1f).Column(c =>
                {
                    FieldPair(c, NumberLabel() + ":", DocumentNumber(), first: true);
                    FieldPair(c, "Invoice Date:", DocumentDate());

                    if (bill.Time.HasValue && bill.Time.Value != TimeSpan.MinValue)
                        FieldPair(c, "Invoice Time:", QuestPdfEngine.TimeOrDash(bill.Time));

                    if (!string.IsNullOrWhiteSpace(bill.StateOfSupply))
                        FieldPair(c, "Place of Supply:", bill.StateOfSupply);

                    if (bill.PODate.HasValue && bill.PODate.Value != DateTime.MinValue)
                        FieldPair(c, "PO date:", QuestPdfEngine.DateOrDash(bill.PODate));
                });
            });
        }

        private void FieldPair(ColumnDescriptor c, string label, string value, bool first = false)
        {
            c.Item().PaddingTop(first ? 3 : 4).Row(r =>
            {
                r.RelativeItem(1.1f).Text(label).FontSize(Small).Bold();
                r.RelativeItem(1.2f).Text(QuestPdfEngine.Dash(value)).FontSize(Small).FontColor(Accent);
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
                    c.RelativeColumn(9f);
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

                    Cell(table, index.ToString(), "left", Accent);
                    Cell(table, QuestPdfEngine.Dash(item.Item), "left", null, true);
                    Cell(table, QuestPdfEngine.Dash(item.HSNCode), "left");
                    Cell(table, qty, "right", Accent);
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
            h.Cell().Background(_bannerBg).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold().FontColor(_slab);
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
            f.Cell().Background(_bannerBg).Padding(4)
                .Element(c => Align(c, align))
                .Text(text).FontSize(Small).Bold().FontColor(_slab);
        }

        // =================================================================
        //  Pay To | totals
        // =================================================================
        private void ComposeFooterRow(ColumnDescriptor col)
        {
            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem(1.25f).Column(c =>
                {
                    if (HasBank())
                    {
                        c.Item().Text("Pay To:").FontSize(Body).Bold().FontColor(Accent);
                        c.Item().PaddingTop(3).Column(ComposeBankLines);
                    }

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

                    c.Item().PaddingTop(6).Text("Invoice Amount In Words:").FontSize(Small).Bold().FontColor(Accent);
                    c.Item().PaddingTop(2)
                        .Text(QuestPdfEngine.AmountInWords(Data.GrandTotal, S.AmountInWordsFormat)).FontSize(Small);
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
            if (emphasise) item = item.Background(_bannerBg);

            item.Element(PadCell).Row(r =>
            {
                var l = r.RelativeItem(1.5f).Text(label).FontSize(Small);
                var v = r.RelativeItem(1f).AlignRight().Text(value).FontSize(Small);

                if (emphasise)
                {
                    l.Bold().FontColor(_slab);
                    v.Bold().FontColor(_slab);
                }
            });
        }
    }
}
