using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// Thermal / POS receipt layout: one narrow continuous column, dashed rules,
    /// stacked item blocks. Nothing here shares geometry with the A4 renderer -
    /// a receipt is a different document, not a narrower invoice.
    /// </summary>
    public class QuestThermalDocument : IDocument
    {
        private readonly QuestDocumentData _data;
        private readonly IWebHostEnvironment _env;
        private readonly PrintSettingsModel _s;

        private readonly Color _ink;
        private readonly Color _rule;
        private readonly byte[] _logoBytes;

        private readonly float _rollWidth;
        private readonly float _body;
        private readonly bool _bold;

        private static readonly float[] DashPattern = { 2f, 2f };

        public QuestThermalDocument(QuestDocumentData data, IWebHostEnvironment env)
        {
            _data = data ?? new QuestDocumentData();
            _env = env;
            _s = _data.Settings;

            _ink = QuestPdfEngine.ParseColor(_s.EffectivePrimaryColor, "#000000");
            _rule = QuestPdfEngine.ParseColor(_s.EffectiveBorderColor, "#000000");

            _rollWidth = QuestPdfEngine.ThermalWidth(_s.PaperSize, _s.CustomChars);
            if (_rollWidth <= 0f) _rollWidth = QuestPdfEngine.ThermalWidth("3 Inch");

            _body = QuestPdfEngine.ThermalBodyFontSize;
            _bold = _s.UseTextStyling;

            if (_s.PrintLogo)
                _logoBytes = QuestPdfEngine.ReadAsset(_env, _data.Company?.LogoPath);
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
        public void Compose(IDocumentContainer container)
        {
            var copies = _s.NumberOfCopies < 1 ? 1 : (_s.NumberOfCopies > 5 ? 5 : _s.NumberOfCopies);

            for (int copy = 0; copy < copies; copy++)
                ComposeReceipt(container);
        }

        private void ComposeReceipt(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // A roll has no fixed height - the page grows with the receipt.
                page.ContinuousSize(_rollWidth);

                page.MarginLeft((float)_s.MarginLeft);
                page.MarginRight((float)_s.MarginRight);
                page.MarginBottom((float)_s.MarginBottom);
                page.MarginTop((float)_s.ExtraSpaceTop + 6f);

                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x
                    .FontFamily(FontFamilyName(), QuestPdfEngine.FallbackFontFamily)
                    .FontSize(_body)
                    .FontColor(_ink)
                    .LineHeight(1.15f));

                page.Content().Column(col =>
                {
                    ComposeHeader(col);
                    Rule(col);

                    ComposeTitleAndParty(col);
                    Rule(col);

                    ComposeItemHeader(col);
                    ComposeItems(col);
                    Rule(col);

                    ComposeTotals(col);
                    Rule(col);

                    ComposeFooter(col);

                    // "Extra lines at the end" - blank feed before the tear-off.
                    for (int i = 0; i < Math.Min(_s.ExtraLinesEnd, 20); i++)
                        col.Item().Height(_body * 1.4f);
                });
            });
        }

        private string FontFamilyName()
        {
            return string.IsNullOrWhiteSpace(_s.FontFamily) ? QuestPdfEngine.DefaultFontFamily : _s.FontFamily.Trim();
        }

        // =================================================================
        //  Building blocks
        // =================================================================
        private void Rule(ColumnDescriptor col)
        {
            col.Item().PaddingVertical(3)
                .LineHorizontal(0.5f)
                .LineDashPattern(DashPattern)
                .LineColor(_rule);
        }

        private void Centered(ColumnDescriptor col, string text, float size, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var span = col.Item().AlignCenter().Text(text.Trim()).FontSize(size);
            if (bold && _bold) span.Bold();
        }

        private static string Pick(params string[] candidates)
        {
            foreach (var c in candidates)
                if (!string.IsNullOrWhiteSpace(c)) return c.Trim();
            return string.Empty;
        }

        private string Money(decimal value)
        {
            return QuestPdfEngine.Money(value, _s.PrintAmountWithDecimal, _s.PrintAmountWithGrouping);
        }

        // =================================================================
        //  Header - logo + company block, all centred
        // =================================================================
        private void ComposeHeader(ColumnDescriptor col)
        {
            if (_logoBytes != null)
            {
                col.Item().AlignCenter().Height(34f).PaddingBottom(3)
                    .Image(_logoBytes).FitArea();
            }

            if (_s.PrintCompanyName)
            {
                var name = Pick(_s.CompanyNameText, _data.Company?.BusinessName);
                Centered(col, name, CompanyNameSize(), true);
            }

            if (_s.PrintAddress)
                Centered(col, Pick(_s.AddressText, BuildCompanyAddress()), _body);

            if (_s.PrintState && _data.Company != null)
            {
                var code = _data.Company.statecode;
                var stateName = _data.Company.statename;
                if (!string.IsNullOrWhiteSpace(stateName))
                    Centered(col, "State: " + (string.IsNullOrWhiteSpace(code) ? "" : code + "-") + stateName, _body);
            }

            if (_s.PrintPhone)
            {
                var phone = Pick(_s.PhoneText, _data.Company?.PhoneNumber);
                if (!string.IsNullOrWhiteSpace(phone)) Centered(col, "Ph.No.: " + phone, _body);
            }

            if (_s.PrintEmail)
            {
                var email = Pick(_s.EmailText, _data.Company?.Email);
                if (!string.IsNullOrWhiteSpace(email)) Centered(col, "Email: " + email, _body);
            }

            if (_s.PrintGstin)
            {
                var gstin = Pick(_s.GstinText, _data.Company?.Gstin);
                if (!string.IsNullOrWhiteSpace(gstin)) Centered(col, "GSTIN: " + gstin, _body);
            }
        }

        private float CompanyNameSize()
        {
            // The A4 sizes are far too large for a 58-88 mm roll.
            switch ((_s.CompanyNameTextSize ?? "Medium").Trim().ToLowerInvariant())
            {
                case "small": return _body + 1f;
                case "medium": return _body + 2f;
                case "large": return _body + 3.5f;
                case "extra large": return _body + 5f;
                default: return _body + 2f;
            }
        }

        private float TitleSize()
        {
            switch ((_s.InvoiceTextSize ?? "Small").Trim().ToLowerInvariant())
            {
                case "small": return _body + 0.5f;
                case "medium": return _body + 1.5f;
                case "large": return _body + 3f;
                case "extra large": return _body + 4.5f;
                default: return _body + 0.5f;
            }
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
        //  Title + party + document meta
        // =================================================================
        private void ComposeTitleAndParty(ColumnDescriptor col)
        {
            var bill = _data.Bill;
            var party = _data.Party;

            Centered(col, _data.DocumentTitle, TitleSize(), true);

            var partyName = Pick(bill.BillingName, party?.PartyName);
            if (!string.IsNullOrWhiteSpace(partyName))
            {
                var span = col.Item().PaddingTop(2).Text(partyName).FontSize(_body);
                if (_bold) span.Bold();
            }

            var partyPhone = Pick(party?.PhoneNumber, bill.PhoneNo);
            SplitLine(col,
                string.IsNullOrWhiteSpace(partyPhone) ? "" : "Ph. No.: " + partyPhone,
                "Date: " + QuestPdfEngine.DateOrDash(bill.InvoiceDate != DateTime.MinValue ? bill.InvoiceDate : bill.BillDate));

            SplitLine(col, "Bill To:", NumberLabel() + ": " + DocumentNumber());

            var billAddress = Pick(bill.BillingAddress, party?.BillingAddress);
            if (!string.IsNullOrWhiteSpace(billAddress))
                col.Item().Text(billAddress).FontSize(_body);

            if (!string.IsNullOrWhiteSpace(party?.GSTIN))
                col.Item().Text("GSTIN: " + party.GSTIN).FontSize(_body);

            col.Item().Text("Place of Supply:").FontSize(_body);
            col.Item().Text(Pick(bill.StateOfSupply, party?.StateName, "-")).FontSize(_body);

            if (_s.PrintPaymentMode && !string.IsNullOrWhiteSpace(bill.PaymentType))
                col.Item().Text("Payment Mode: " + bill.PaymentType).FontSize(_body);
        }

        private void SplitLine(ColumnDescriptor col, string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left) && string.IsNullOrWhiteSpace(right)) return;

            col.Item().Row(row =>
            {
                row.RelativeItem(1.35f).Text(left ?? string.Empty).FontSize(_body);
                row.RelativeItem(1f).AlignRight().Text(right ?? string.Empty).FontSize(_body);
            });
        }

        /// <summary>
        /// A short caption for the document number. The full title would wrap on a
        /// 58 mm roll, so "Tax Invoice" becomes "Invoice No.".
        /// </summary>
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

        /// <summary>"(5%)" for a real rate, empty when the rate is zero or unknown.</summary>
        private static string RateSuffix(decimal percent)
        {
            return percent == 0m ? string.Empty : "(" + QuestPdfEngine.Percent(percent) + ")";
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
        //  Item block
        // =================================================================
        private const float SrNoWidth = 12f;

        private void ComposeItemHeader(ColumnDescriptor col)
        {
            col.Item().Row(row =>
            {
                if (_s.PrintItemSrNo)
                    row.ConstantItem(SrNoWidth).Text("#").FontSize(_body).Bold();

                var span = row.RelativeItem().Text(_s.PrintItemHsn ? "Item Name(HSN)" : "Item Name").FontSize(_body);
                if (_bold) span.Bold();
            });

            col.Item().Row(row =>
            {
                if (_s.PrintItemSrNo)
                    row.ConstantItem(SrNoWidth).Text(string.Empty);

                row.RelativeItem().Row(AmountColumns(
                    "Qty",
                    _s.PrintItemMrp ? "MRP" : null,
                    "Price",
                    "Amount",
                    header: true));
            });

            if (_s.PrintItemDescription)
            {
                col.Item().Row(row =>
                {
                    if (_s.PrintItemSrNo)
                        row.ConstantItem(SrNoWidth).Text(string.Empty);

                    var span = row.RelativeItem().Text("Description").FontSize(_body);
                    if (_bold) span.Bold();
                });
            }
        }

        /// <summary>Builds the Qty / MRP / Price / Amount strip used by header and rows.</summary>
        private Action<RowDescriptor> AmountColumns(string qty, string mrp, string price, string amount, bool header)
        {
            return row =>
            {
                var qtySpan = row.RelativeItem(1.3f).Text(qty ?? string.Empty).FontSize(_body);
                if (header && _bold) qtySpan.Bold();

                if (mrp != null)
                {
                    var mrpSpan = row.RelativeItem(1f).AlignRight().Text(mrp).FontSize(_body);
                    if (header && _bold) mrpSpan.Bold();
                }

                var priceSpan = row.RelativeItem(1f).AlignRight().Text(price ?? string.Empty).FontSize(_body);
                if (header && _bold) priceSpan.Bold();

                var amountSpan = row.RelativeItem(1.25f).AlignRight().Text(amount ?? string.Empty).FontSize(_body);
                if (header && _bold) amountSpan.Bold();
            };
        }

        private void ComposeItems(ColumnDescriptor col)
        {
            int index = 0;

            foreach (var item in _data.Items)
            {
                index++;
                var idx = index;

                col.Item().PaddingTop(2).Row(row =>
                {
                    if (_s.PrintItemSrNo)
                        row.ConstantItem(SrNoWidth).Text(idx.ToString()).FontSize(_body);

                    row.RelativeItem().Column(line => ComposeItemLines(line, item));
                });
            }
        }

        private void ComposeItemLines(ColumnDescriptor line, PurchaseBillItem item)
        {
            // 1. Name (HSN)
            var name = QuestPdfEngine.Dash(item.Item);
            if (_s.PrintItemHsn && !string.IsNullOrWhiteSpace(item.HSNCode))
                name += "(" + item.HSNCode.Trim() + ")";
            line.Item().Text(name).FontSize(_body);

            // 2. Qty [+ free] unit | MRP | Price | Amount
            var free = item.FreeQuantity ?? 0m;
            var qtyText = QuestPdfEngine.Qty(item.Quantity)
                          + (free > 0 ? " + " + QuestPdfEngine.Qty(free) : string.Empty)
                          + (_s.PrintItemUom && !string.IsNullOrWhiteSpace(item.Unit) ? item.Unit.Trim() : string.Empty);

            var lineTaxable = item.Quantity * item.PricePerUnit - item.DiscountAmount;

            line.Item().Row(AmountColumns(
                qtyText,
                _s.PrintItemMrp ? Money(item.MRP ?? item.PricePerUnit) : null,
                Money(item.PricePerUnit),
                Money(item.Quantity * item.PricePerUnit),
                header: false));

            // 3. Description
            if (_s.PrintItemDescription && !string.IsNullOrWhiteSpace(item.Description))
                line.Item().Text(item.Description.Trim()).FontSize(_body).Italic();

            // 4. Additional item details, joined onto one wrapped line
            var extras = new List<string>();
            if (_s.PrintItemBatchNo && !string.IsNullOrWhiteSpace(item.batchno)) extras.Add("Batch No.: " + item.batchno.Trim());
            if (_s.PrintItemSerialNo && !string.IsNullOrWhiteSpace(item.serialno)) extras.Add("Serial No.: " + item.serialno.Trim());
            if (_s.PrintItemModelNo && !string.IsNullOrWhiteSpace(item.modelno)) extras.Add("Model No.: " + item.modelno.Trim());
            if (_s.PrintItemExpDate && item.ExpiryDate.HasValue && item.ExpiryDate.Value != DateTime.MinValue)
                extras.Add("Exp. Date: " + item.ExpiryDate.Value.ToString("MM/yyyy"));
            if (_s.PrintItemMfgDate && item.ManufacturingDate.HasValue && item.ManufacturingDate.Value != DateTime.MinValue)
                extras.Add("Mfg. Date: " + item.ManufacturingDate.Value.ToString("dd/MM/yyyy"));
            if (_s.PrintItemSize && !string.IsNullOrWhiteSpace(item.Size)) extras.Add("Size: " + item.Size.Trim());

            if (extras.Count > 0)
                line.Item().Text(string.Join(", ", extras)).FontSize(_body);

            // 5. Per-line discount / tax / final
            if (item.DiscountAmount != 0)
                ValueLine(line, "Disc." + RateSuffix(item.DiscountPercentage), Money(-item.DiscountAmount));

            if (_s.PrintTaxDetails && item.TaxAmount != 0)
                ValueLine(line, "Tax" + RateSuffix(item.TaxPercentage), Money(item.TaxAmount));

            ValueLine(line, "Final amount", Money(item.TotalAmount ?? (lineTaxable + item.TaxAmount)));
        }

        /// <summary>label ......... : ......... value</summary>
        private void ValueLine(ColumnDescriptor col, string label, string value, bool emphasise = false)
        {
            col.Item().Row(row =>
            {
                var l = row.RelativeItem(1.6f).Text(label).FontSize(_body);
                if (emphasise && _bold) l.Bold();

                row.ConstantItem(6f).Text(emphasise ? string.Empty : ":").FontSize(_body);

                var v = row.RelativeItem(1.4f).AlignRight().Text(value).FontSize(_body);
                if (emphasise && _bold) v.Bold();
            });
        }

        // =================================================================
        //  Totals
        // =================================================================
        private void ComposeTotals(ColumnDescriptor col)
        {
            var bill = _data.Bill;

            if (_s.PrintTotalItemQuantity)
            {
                var totalFree = _data.Items.Sum(i => i.FreeQuantity ?? 0m);
                var qtyLabel = "Qty: " + QuestPdfEngine.Qty(_data.TotalQuantity)
                               + (totalFree > 0 ? " + " + QuestPdfEngine.Qty(totalFree) : string.Empty);

                col.Item().Row(row =>
                {
                    row.RelativeItem(1.6f).Text(qtyLabel).FontSize(_body);
                    row.ConstantItem(6f).Text(string.Empty);
                    row.RelativeItem(1.4f).AlignRight()
                        .Text(Money(_data.TotalTaxable + _data.TotalDiscount)).FontSize(_body);
                });
            }

            if (_data.TotalDiscount != 0)
                ValueLine(col, "Disc." + RateSuffix(bill.DiscountPercent), Money(-_data.TotalDiscount));

            if (_s.PrintTaxDetails && _data.TotalTax != 0)
                ValueLine(col, "Tax" + RateSuffix(bill.TaxPercentage), Money(_data.TotalTax));

            if (_data.TotalCess != 0)
                ValueLine(col, "Cess", Money(_data.TotalCess));

            if (bill.ShippingAmount != 0) ValueLine(col, "Shipping", Money(bill.ShippingAmount));
            if (bill.PackingAmount != 0) ValueLine(col, "Packing", Money(bill.PackingAmount));
            if (bill.AdjustmentAmount != 0) ValueLine(col, "Adjustment", Money(bill.AdjustmentAmount));
            if (bill.TdsTcsAmount != 0) ValueLine(col, bill.TCSTDSType.ToString(), Money(bill.TdsTcsAmount));
            if (bill.IsRoundOff && bill.RoundOffValue != 0) ValueLine(col, "Round Off", Money(bill.RoundOffValue));

            ValueLine(col, "Total", Money(_data.GrandTotal), emphasise: true);

            if (_s.PrintReceivedAmount)
                ValueLine(col, "Received", Money(bill.paidReciveamount), emphasise: true);

            if (_s.PrintBalanceAmount)
                ValueLine(col, "Balance", Money(_data.GrandTotal - bill.paidReciveamount));

            if (_s.PrintCurrentBalanceParty)
                ValueLine(col, "Current Balance", Money(_data.PartyCurrentBalance));
        }

        // =================================================================
        //  Footer
        // =================================================================
        private void ComposeFooter(ColumnDescriptor col)
        {
            var bill = _data.Bill;

            if (_s.PrintYouSaved && _data.TotalDiscount > 0)
                Centered(col, "You Saved " + Money(_data.TotalDiscount), _body, true);

            col.Item().PaddingTop(2).Text(
                    QuestPdfEngine.AmountInWords(_data.GrandTotal, _s.AmountInWordsFormat))
                .FontSize(_body);

            if (_s.PrintTaxDetails && _data.TaxSummary.Count > 0)
            {
                Rule(col);
                col.Item().Row(row =>
                {
                    row.RelativeItem(1f).Text("Tax%").FontSize(_body);
                    row.RelativeItem(1.4f).AlignRight().Text("Taxable").FontSize(_body);
                    row.RelativeItem(1.2f).AlignRight().Text(_data.IsDomestic ? "CGST+SGST" : "IGST").FontSize(_body);
                });

                foreach (var t in _data.TaxSummary)
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(1f).Text(QuestPdfEngine.Percent(t.Rate)).FontSize(_body);
                        row.RelativeItem(1.4f).AlignRight().Text(Money(t.Taxable)).FontSize(_body);
                        row.RelativeItem(1.2f).AlignRight().Text(Money(t.TaxAmount)).FontSize(_body);
                    });
                }
            }

            if (bill.DueDate != DateTime.MinValue && _data.GrandTotal - bill.paidReciveamount > 0)
            {
                var days = (int)Math.Ceiling((bill.DueDate.Date - DateTime.Today).TotalDays);
                if (days > 0)
                {
                    Rule(col);
                    Centered(col, $"Balance to be paid in {days} day{(days == 1 ? "" : "s")}", _body, true);
                }
            }

            if (_s.PrintDescription && !string.IsNullOrWhiteSpace(bill.Description))
            {
                Rule(col);
                col.Item().Text(bill.Description.Trim()).FontSize(_body);
            }

            if (_s.PrintTermsConditions && !string.IsNullOrWhiteSpace(_s.DefaultTermsText))
            {
                Rule(col);
                col.Item().Text("Terms and Conditions").FontSize(_body).Bold();
                col.Item().Text(_s.DefaultTermsText.Trim()).FontSize(_body);
            }

            if (_s.PrintSignatureText)
            {
                col.Item().PaddingTop(10).AlignRight()
                    .Text(Pick(_s.SignatureText, "Authorized Signatory")).FontSize(_body);
            }

            if (_s.PrintAcknowledgement)
            {
                Rule(col);
                Centered(col, "ACKNOWLEDGEMENT", _body, true);
                col.Item().PaddingTop(12).Text("Receiver Signature").FontSize(_body);
            }
        }
    }
}
