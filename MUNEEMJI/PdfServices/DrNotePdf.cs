using iTextSharp.text;
using iTextSharp.text.pdf;
using Insight.Database;
using MUNEEMJI.Models;
using MUNEEMJI.Models.BankAccount;
using MUNEEMJI.PdfServices.Common;
using Npgsql;
using System.Data;
using System.Text;

namespace MUNEEMJI.PdfServices
{
    public interface IDrNotePdf
    {
        Task<string> GetDrNotePdfById(int id, IWebHostEnvironment _env);
    }

    public class DrNotePdf : IDrNotePdf
    {
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public DrNotePdf() { }

        public async Task<string> GetDrNotePdfById(int id, IWebHostEnvironment _env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string FontPath = Path.Combine(_env.WebRootPath, "DataContainer", "Font");
            string ImagePath = Path.Combine(_env.WebRootPath, "DataContainer", "Images");
            BusinessProfileModel companydetail = new BusinessProfileModel();
            PurchaseBill Bill = new PurchaseBill();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var Id = 1;
                using (var cmd = new NpgsqlCommand($"SELECT bp.*,sts.name,sts.code FROM business_profiles as bp left join states as sts on bp.state_id = sts.id WHERE businessesid = {Id}", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        companydetail.Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0;
                        companydetail.BusinessName = reader["business_name"] != DBNull.Value ? reader["business_name"].ToString() : string.Empty;
                        companydetail.PhoneNumber = reader["phone_number"] != DBNull.Value ? reader["phone_number"].ToString() : string.Empty;
                        companydetail.Gstin = reader["gstin"] != DBNull.Value ? reader["gstin"].ToString() : string.Empty;
                        companydetail.Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : string.Empty;
                        companydetail.BusinessTypeId = reader["business_type_id"] != DBNull.Value ? Convert.ToInt32(reader["business_type_id"]) : 0;
                        companydetail.BusinessCategoryId = reader["business_category_id"] != DBNull.Value ? Convert.ToInt32(reader["business_category_id"]) : 0;
                        companydetail.StateId = reader["state_id"] != DBNull.Value ? Convert.ToInt32(reader["state_id"]) : 0;
                        companydetail.Pincode = reader["pincode"] != DBNull.Value ? reader["pincode"].ToString() : string.Empty;
                        companydetail.Address = reader["address"] != DBNull.Value ? reader["address"].ToString() : string.Empty;
                        companydetail.LogoPath = reader["logo_path"] != DBNull.Value ? reader["logo_path"].ToString() : string.Empty;
                        companydetail.SignaturePath = reader["signature_path"] != DBNull.Value ? reader["signature_path"].ToString() : string.Empty;
                        companydetail.statecode = reader["code"] != DBNull.Value ? reader["code"].ToString() : string.Empty;
                        companydetail.statename = reader["name"] != DBNull.Value ? reader["name"].ToString() : string.Empty;
                    }
                }
            }

            Bill = await GetBillByIdForPdf(id) ?? new PurchaseBill();
            var partydetail = PartDetailForPdfById(Bill.PartyId);
            var BankDetail = GetBankForPdf();
            bool isDomestic = partydetail != null && Bill.StateId == partydetail.StateId;

            if (!FontFactory.IsRegistered("ARIAL"))
                FontFactory.Register(Path.Combine(FontPath, "ARIAL.ttf"), "ARIAL");

            BaseFont bfArial = BaseFont.CreateFont(Path.Combine(FontPath, "ARIAL.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            BaseFont bfRupee = BaseFont.CreateFont(Path.Combine(FontPath, "arial_with_rupee.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            Font fNormal = new Font(bfArial, 8f, Font.NORMAL);
            Font fBold = new Font(bfArial, 8f, Font.BOLD);
            Font fSmall = new Font(bfArial, 7f, Font.NORMAL);
            Font fSmallBold = new Font(bfArial, 7f, Font.BOLD);
            Font fLargeBold = new Font(bfArial, 11f, Font.BOLD);
            Font fTitle = new Font(bfArial, 13f, Font.BOLD);
            Font fRupee = new Font(bfRupee, 8f, Font.NORMAL);
            Font fRupeeBold = new Font(bfRupee, 8f, Font.BOLD);
            Font fRupeeSmall = new Font(bfRupee, 7f, Font.NORMAL);

            BaseColor grayBg = new BaseColor(187, 187, 187);
            BaseColor borderClr = new BaseColor(169, 169, 169);
            BaseColor darkBrown = new BaseColor(78, 42, 10);
            BaseColor totalRowBg = new BaseColor(255, 243, 205);

            Document doc = new Document(PageSize.A4, 36, 36, 88, 65);
            try
            {
                MemoryStream stream = new MemoryStream();
                using (PdfWriter wri = PdfWriter.GetInstance(doc, stream))
                {
                    wri.CloseStream = false;
                    wri.PageEvent = new SalesInvoicesPdf.PdfPageEvents(companydetail, _env);

                    doc.Open();
                    doc.NewPage();

                    // ===== SECTION 1: "Debit Note" Title =====
                    var titlePhrase = new Phrase();
                    titlePhrase.Add(new Chunk("Debit Note", new Font(bfArial, 14f, Font.BOLD, darkBrown)));
                    var titlePara = new Paragraph(titlePhrase);
                    titlePara.Alignment = Element.ALIGN_CENTER;
                    titlePara.SpacingAfter = 8f;
                    doc.Add(titlePara);

                    // ===== SECTION 2: Info Grid (Bill To / Ship To / Transport / Invoice Details) =====
                    PdfPTable infoGrid = new PdfPTable(4);
                    infoGrid.WidthPercentage = 100;
                    infoGrid.SetWidths(new float[] { 25f, 25f, 25f, 25f });

                    // Header row
                    AddHeaderCell(infoGrid, "Bill To", fSmallBold, grayBg, borderClr);
                    AddHeaderCell(infoGrid, "Ship To", fSmallBold, grayBg, borderClr);
                    AddHeaderCell(infoGrid, "Transportation Details", fSmallBold, grayBg, borderClr);
                    AddHeaderCell(infoGrid, "Invoice Details", fSmallBold, grayBg, borderClr);

                    // Bill To content
                    var billToPhrase = new Phrase(12f);
                    billToPhrase.Add(new Chunk((partydetail?.PartyName ?? "N/A") + "\n", fBold));
                    billToPhrase.Add(new Chunk((partydetail?.BillingAddress ?? "") + "\n", fSmall));
                    billToPhrase.Add(new Chunk("State: " + (partydetail?.StateCode ?? "") + "-" + (partydetail?.StateName ?? ""), fSmall));
                    AddContentCell(infoGrid, billToPhrase, borderClr);

                    // Ship To content
                    string shipAddr = !string.IsNullOrEmpty(Bill.ShippingAddress) ? Bill.ShippingAddress : (partydetail?.ShippingAddress ?? "");
                    var shipToPhrase = new Phrase(12f);
                    shipToPhrase.Add(new Chunk(shipAddr, fSmall));
                    AddContentCell(infoGrid, shipToPhrase, borderClr);

                    // Transportation Details content
                    string deliveryDateStr = Bill.DeliveryDate.HasValue && Bill.DeliveryDate.Value != DateTime.MinValue
                        ? Bill.DeliveryDate.Value.ToString("dd-MM-yyyy") : "";
                    var transportPhrase = new Phrase(12f);
                    transportPhrase.Add(new Chunk("Transport Name: " + (Bill.TransportName ?? "") + "\n", fSmall));
                    transportPhrase.Add(new Chunk("Vehicle Number: " + (Bill.VehicleNumber ?? "") + "\n", fSmall));
                    transportPhrase.Add(new Chunk("Delivery Date: " + deliveryDateStr + "\n", fSmall));
                    transportPhrase.Add(new Chunk("Delivery Location: " + (Bill.DeliveryLocation ?? "") + "\n", fSmall));
                    transportPhrase.Add(new Chunk("Field 5: " + (Bill.Field5 ?? "") + "\n", fSmall));
                    transportPhrase.Add(new Chunk("Field 6: " + (Bill.Field6 ?? ""), fSmall));
                    AddContentCell(infoGrid, transportPhrase, borderClr);

                    // Invoice Details content
                    string invDateStr = Bill.InvoiceDate.HasValue && Bill.InvoiceDate.Value != DateTime.MinValue
                        ? Bill.InvoiceDate.Value.ToString("dd-MM-yyyy") : "";
                    string timeStr = Bill.Time.HasValue && Bill.Time.Value != TimeSpan.MinValue
                        ? Bill.Time.Value.ToString(@"hh\:mm") + " " + (Bill.Time.Value.Hours >= 12 ? "PM" : "AM") : "";
                    var invPhrase = new Phrase(12f);
                    invPhrase.Add(new Chunk("Invoice No. : " + (Bill.InvoiceNumber?.ToString() ?? "") + "\n", fSmall));
                    invPhrase.Add(new Chunk("Date : " + invDateStr + "\n", fSmall));
                    invPhrase.Add(new Chunk("Time : " + timeStr + "\n", fSmall));
                    invPhrase.Add(new Chunk("Place of supply: " + (Bill.StateOfSupply ?? ""), fSmall));
                    var invCell = new PdfPCell(invPhrase);
                    invCell.BorderColor = borderClr;
                    invCell.Padding = 5f;
                    invCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    invCell.VerticalAlignment = Element.ALIGN_TOP;
                    infoGrid.AddCell(invCell);

                    doc.Add(infoGrid);

                    // ===== SECTION 3: Items Table =====
                    var validItems = Bill.BillItems?.Where(x => x.PricePerUnit > 0).ToList() ?? new List<PurchaseBillItem>();

                    float[] itemWidths = { 3f, 10f, 6f, 6f, 5f, 4f, 7f, 7f, 7f, 8f, 7f, 7f };
                    PdfPTable itemsTable = new PdfPTable(itemWidths);
                    itemsTable.WidthPercentage = 100;
                    itemsTable.SpacingBefore = 5f;

                    string[] headers = { "#", "Item name", "Item\nCode", "HSN/\nSAC", "Quantity", "Unit", "Price/\nUnit", "Taxable\nPrice/\nUnit", "Taxable\namount", "GST", "Final\nRate", "Amount" };
                    foreach (var h in headers)
                    {
                        var hCell = new PdfPCell(new Phrase(h, fSmallBold));
                        hCell.BackgroundColor = darkBrown;
                        hCell.BorderColor = darkBrown;
                        hCell.Padding = 4f;
                        hCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        hCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        // white text on dark brown
                        hCell.Phrase = new Phrase(h, new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE));
                        itemsTable.AddCell(hCell);
                    }

                    decimal totalQty = 0, totalTaxableAmt = 0, totalGstAmt = 0, totalAmount = 0;
                    int rowNum = 1;
                    foreach (var item in validItems)
                    {
                        decimal taxableAmt = item.PricePerUnit * item.Quantity - item.DiscountAmount;
                        decimal taxPerUnit = item.Quantity != 0 ? item.TaxAmount / item.Quantity : 0;
                        decimal finalRate = item.PricePerUnit + taxPerUnit;

                        totalQty += item.Quantity;
                        totalTaxableAmt += taxableAmt;
                        totalGstAmt += item.TaxAmount;
                        totalAmount += item.TotalAmount ?? 0;

                        AddItemCell(itemsTable, rowNum.ToString(), fSmall, borderClr, Element.ALIGN_CENTER);
                        AddItemCell(itemsTable, item.Item ?? "", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, item.ItemCode ?? "", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, item.HSNCode ?? "", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, item.Quantity.ToString("0.##"), fSmall, borderClr, Element.ALIGN_CENTER);
                        AddItemCell(itemsTable, item.Unit ?? "", fSmall, borderClr, Element.ALIGN_CENTER);
                        AddRupeeCell(itemsTable, item.PricePerUnit.ToString("0.00"), fRupeeSmall, borderClr);
                        AddRupeeCell(itemsTable, item.PricePerUnit.ToString("0.00"), fRupeeSmall, borderClr);
                        AddRupeeCell(itemsTable, taxableAmt.ToString("0.00"), fRupeeSmall, borderClr);
                        // GST cell: amount + percentage
                        var gstPhrase = new Phrase();
                        gstPhrase.Add(new Chunk("\u20B9 " + item.TaxAmount.ToString("0.00") + "\n", fRupeeSmall));
                        gstPhrase.Add(new Chunk("(" + item.TaxPercentage.ToString("0.##") + "%)", fSmall));
                        var gstCell = new PdfPCell(gstPhrase);
                        gstCell.BorderColor = borderClr;
                        gstCell.Padding = 3f;
                        gstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(gstCell);

                        AddRupeeCell(itemsTable, finalRate.ToString("0.00"), fRupeeSmall, borderClr);
                        AddRupeeCell(itemsTable, (item.TotalAmount ?? 0).ToString("0.00"), fRupeeSmall, borderClr);
                        rowNum++;
                    }

                    // Total row
                    var totLabelCell = new PdfPCell(new Phrase("Total", fBold));
                    totLabelCell.Colspan = 4;
                    totLabelCell.BackgroundColor = darkBrown;
                    totLabelCell.BorderColor = darkBrown;
                    totLabelCell.Padding = 4f;
                    totLabelCell.Phrase = new Phrase("Total", new Font(bfArial, 8f, Font.BOLD, BaseColor.WHITE));
                    itemsTable.AddCell(totLabelCell);

                    var totQtyCell = new PdfPCell(new Phrase(totalQty.ToString("0.##"), new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE)));
                    totQtyCell.BackgroundColor = darkBrown;
                    totQtyCell.BorderColor = darkBrown;
                    totQtyCell.Padding = 4f;
                    totQtyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    itemsTable.AddCell(totQtyCell);

                    // blank cells for Unit, Price/Unit, Taxable Price/Unit
                    for (int i = 0; i < 3; i++)
                    {
                        var blankCell = new PdfPCell(new Phrase("", fSmall));
                        blankCell.BackgroundColor = darkBrown;
                        blankCell.BorderColor = darkBrown;
                        blankCell.Padding = 4f;
                        itemsTable.AddCell(blankCell);
                    }

                    var totTaxableCell = new PdfPCell(new Phrase("\u20B9 " + totalTaxableAmt.ToString("0.00"), new Font(bfRupee, 7f, Font.BOLD, BaseColor.WHITE)));
                    totTaxableCell.BackgroundColor = darkBrown;
                    totTaxableCell.BorderColor = darkBrown;
                    totTaxableCell.Padding = 4f;
                    totTaxableCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totTaxableCell);

                    var totGstCell = new PdfPCell(new Phrase("\u20B9 " + totalGstAmt.ToString("0.00"), new Font(bfRupee, 7f, Font.BOLD, BaseColor.WHITE)));
                    totGstCell.BackgroundColor = darkBrown;
                    totGstCell.BorderColor = darkBrown;
                    totGstCell.Padding = 4f;
                    totGstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totGstCell);

                    // blank for Final Rate
                    var blankFR = new PdfPCell(new Phrase("", fSmall));
                    blankFR.BackgroundColor = darkBrown;
                    blankFR.BorderColor = darkBrown;
                    blankFR.Padding = 4f;
                    itemsTable.AddCell(blankFR);

                    var totAmtCell = new PdfPCell(new Phrase("\u20B9 " + totalAmount.ToString("0.00"), new Font(bfRupee, 7f, Font.BOLD, BaseColor.WHITE)));
                    totAmtCell.BackgroundColor = darkBrown;
                    totAmtCell.BorderColor = darkBrown;
                    totAmtCell.Padding = 4f;
                    totAmtCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totAmtCell);

                    doc.Add(itemsTable);

                    // ===== SECTION 4: Summary + Words/Terms (two-column layout) =====
                    PdfPTable summaryOuter = new PdfPTable(2);
                    summaryOuter.WidthPercentage = 100;
                    summaryOuter.SetWidths(new float[] { 50f, 50f });
                    summaryOuter.SpacingBefore = 5f;
                    summaryOuter.KeepTogether = true;

                    // LEFT: Invoice Amount In Words + Terms
                    var leftPhrase = new Phrase();
                    leftPhrase.Add(new Chunk("Invoice Amount In Words\n", fBold));
                    leftPhrase.Add(new Chunk(ConfigControls.ConvertAmountToWords(Bill.FinalAmount) + "\n\n", fSmall));
                    leftPhrase.Add(new Chunk("Terms and Conditions\n", fBold));
                    leftPhrase.Add(new Chunk(Bill.Description ?? "", fSmall));
                    var leftCell = new PdfPCell(leftPhrase);
                    leftCell.Border = Rectangle.NO_BORDER;
                    leftCell.Padding = 5f;
                    summaryOuter.AddCell(leftCell);

                    // RIGHT: Summary rows
                    PdfPTable summaryTbl = new PdfPTable(2);
                    summaryTbl.SetWidths(new float[] { 55f, 45f });

                    decimal subTotal = totalTaxableAmt;
                    AddSummaryRow(summaryTbl, "Sub Total", "\u20B9 " + subTotal.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);

                    if (isDomestic)
                    {
                        decimal halfTax = Bill.TaxAmount / 2;
                        string taxRate = Bill.TaxPercentage > 0 ? (Bill.TaxPercentage / 2).ToString("0.##") : "0";
                        AddSummaryRow(summaryTbl, "SGST@" + taxRate + "%", "\u20B9 " + halfTax.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                        AddSummaryRow(summaryTbl, "CGST@" + taxRate + "%", "\u20B9 " + halfTax.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    }
                    else
                    {
                        string taxRate = Bill.TaxPercentage > 0 ? Bill.TaxPercentage.ToString("0.##") : "0";
                        AddSummaryRow(summaryTbl, "IGST@" + taxRate + "%", "\u20B9 " + Bill.TaxAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    }

                    if (Bill.DiscountAmount > 0)
                        AddSummaryRow(summaryTbl, "Discount (" + Bill.DiscountPercent.ToString("0.##") + "%)", "\u20B9 " + Bill.DiscountAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    if (Bill.ShippingAmount != 0)
                        AddSummaryRow(summaryTbl, "Shipping:", "\u20B9 " + Bill.ShippingAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    if (Bill.PackingAmount != 0)
                        AddSummaryRow(summaryTbl, "Packaging:", "\u20B9 " + Bill.PackingAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    if (Bill.AdjustmentAmount != 0)
                        AddSummaryRow(summaryTbl, "Adjustment:", "\u20B9 " + Bill.AdjustmentAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    if (Bill.RoundOffValue != 0)
                        AddSummaryRow(summaryTbl, "Round off", "\u20B9 " + Bill.RoundOffValue.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);

                    // Total row (highlighted)
                    var totLbl = new PdfPCell(new Phrase("Total", fBold));
                    totLbl.BackgroundColor = totalRowBg;
                    totLbl.BorderColor = borderClr;
                    totLbl.Padding = 4f;
                    summaryTbl.AddCell(totLbl);
                    var totVal = new PdfPCell(new Phrase("\u20B9 " + Bill.FinalAmount.ToString("0.00"), fRupeeBold));
                    totVal.BackgroundColor = totalRowBg;
                    totVal.BorderColor = borderClr;
                    totVal.Padding = 4f;
                    totVal.HorizontalAlignment = Element.ALIGN_RIGHT;
                    summaryTbl.AddCell(totVal);

                    AddSummaryRow(summaryTbl, "Received", "\u20B9 " + Bill.paidReciveamount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    AddSummaryRow(summaryTbl, "Balance", "\u20B9 " + (Bill.FinalAmount - Bill.paidReciveamount).ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    AddSummaryRow(summaryTbl, "Payment mode", Bill.PaymentType ?? "", fSmall, fSmall, borderClr, false);
                    AddSummaryRow(summaryTbl, "Previous Balance", "\u20B9 " + Bill.ReturnNo.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    AddSummaryRow(summaryTbl, "Current Balance", "\u20B9 " + Bill.ReturnNo.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                    AddSummaryRow(summaryTbl, "", "", fSmall, fSmall, borderClr, false); // spacer
                    AddSummaryRow(summaryTbl, "You Saved", "\u20B9 " + Bill.DiscountAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);

                    var rightCell = new PdfPCell(summaryTbl);
                    rightCell.Border = Rectangle.NO_BORDER;
                    rightCell.Padding = 0f;
                    summaryOuter.AddCell(rightCell);

                    doc.Add(summaryOuter);

                    // Force bank details + acknowledgement onto a new page
                    doc.NewPage();

                    // ===== SECTION 5: Bank Details + Authorized Signatory =====
                    PdfPTable bankSection = new PdfPTable(2);
                    bankSection.WidthPercentage = 100;
                    bankSection.SetWidths(new float[] { 55f, 45f });
                    bankSection.SpacingBefore = 10f;

                    // Bank header
                    var payToHeader = new PdfPCell(new Phrase("Pay To:", fBold));
                    payToHeader.BackgroundColor = grayBg;
                    payToHeader.BorderColor = borderClr;
                    payToHeader.Padding = 4f;
                    bankSection.AddCell(payToHeader);

                    var forHeader = new PdfPCell(new Phrase("For : " + (companydetail.BusinessName ?? ""), fBold));
                    forHeader.BackgroundColor = grayBg;
                    forHeader.BorderColor = borderClr;
                    forHeader.Padding = 4f;
                    forHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bankSection.AddCell(forHeader);

                    // Bank details content
                    var bankPhrase = new Phrase(14f);
                    bankPhrase.Add(new Chunk("Bank Name : " + (BankDetail?.BankName ?? "N/A") + "\n", fSmall));
                    bankPhrase.Add(new Chunk("Bank Account No. : " + (BankDetail?.AccountNumber ?? "N/A") + "\n", fSmall));
                    bankPhrase.Add(new Chunk("Bank IFSC code : " + (BankDetail?.IFSCCode ?? "N/A") + "\n", fSmall));
                    bankPhrase.Add(new Chunk("Account holder's name : " + (BankDetail?.AccountDisplayName ?? "N/A"), fSmall));
                    var bankCell = new PdfPCell(bankPhrase);
                    bankCell.BorderColor = borderClr;
                    bankCell.Padding = 5f;
                    bankCell.MinimumHeight = 60f;
                    bankSection.AddCell(bankCell);

                    // Signature cell
                    var sigPhrase = new Phrase();
                    PdfPCell sigCell;
                    string sigPath = companydetail.SignaturePath;
                    if (!string.IsNullOrEmpty(sigPath))
                    {
                        string fullSigPath = sigPath.StartsWith("/") ? Path.Combine(_env.WebRootPath, sigPath.TrimStart('/')) : sigPath;
                        if (File.Exists(fullSigPath))
                        {
                            try
                            {
                                iTextSharp.text.Image sigImg = iTextSharp.text.Image.GetInstance(fullSigPath);
                                sigImg.ScaleToFit(100f, 40f);
                                sigCell = new PdfPCell();
                                sigCell.AddElement(sigImg);
                                var authLabel = new Paragraph("Authorized Signatory", fBold);
                                authLabel.Alignment = Element.ALIGN_RIGHT;
                                sigCell.AddElement(authLabel);
                            }
                            catch
                            {
                                sigPhrase.Add(new Chunk("\n\n\nAuthorized Signatory", fBold));
                                sigCell = new PdfPCell(sigPhrase);
                            }
                        }
                        else
                        {
                            sigPhrase.Add(new Chunk("\n\n\nAuthorized Signatory", fBold));
                            sigCell = new PdfPCell(sigPhrase);
                        }
                    }
                    else
                    {
                        sigPhrase.Add(new Chunk("\n\n\nAuthorized Signatory", fBold));
                        sigCell = new PdfPCell(sigPhrase);
                    }
                    sigCell.BorderColor = borderClr;
                    sigCell.Padding = 5f;
                    sigCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    sigCell.MinimumHeight = 60f;
                    bankSection.AddCell(sigCell);

                    doc.Add(bankSection);

                    // ===== SECTION 6: Dashed Separator =====
                    PdfPTable dashTable = new PdfPTable(1);
                    dashTable.WidthPercentage = 100;
                    dashTable.SpacingBefore = 10f;
                    var dashCell = new PdfPCell(new Phrase(" "));
                    dashCell.Border = Rectangle.NO_BORDER;
                    dashCell.BorderWidthBottom = 1f;
                    dashCell.BorderColorBottom = BaseColor.BLACK;
                    dashCell.Padding = 2f;
                    dashCell.CellEvent = new DashedBorderEvent();
                    dashTable.AddCell(dashCell);
                    doc.Add(dashTable);

                    // ===== SECTION 7: Acknowledgement Slip =====
                    PdfPTable ackSection = new PdfPTable(1);
                    ackSection.WidthPercentage = 100;
                    ackSection.SpacingBefore = 5f;

                    // Acknowledgement heading
                    var ackHeading = new PdfPCell(new Phrase("Acknowledgement", fBold));
                    ackHeading.Border = Rectangle.NO_BORDER;
                    ackHeading.HorizontalAlignment = Element.ALIGN_CENTER;
                    ackHeading.PaddingBottom = 3f;
                    ackSection.AddCell(ackHeading);

                    // Company name
                    var ackCompany = new PdfPCell(new Phrase(companydetail.BusinessName ?? "", fLargeBold));
                    ackCompany.Border = Rectangle.NO_BORDER;
                    ackCompany.HorizontalAlignment = Element.ALIGN_CENTER;
                    ackCompany.PaddingBottom = 8f;
                    ackSection.AddCell(ackCompany);

                    // Two columns: Invoice To + Invoice Details + Receiver Seal
                    PdfPTable ackInner = new PdfPTable(3);
                    ackInner.SetWidths(new float[] { 35f, 35f, 30f });

                    // Invoice To
                    var invToPhrase = new Phrase(14f);
                    invToPhrase.Add(new Chunk("Invoice To:\n", fBold));
                    invToPhrase.Add(new Chunk((partydetail?.PartyName ?? "") + "\n", fBold));
                    invToPhrase.Add(new Chunk(partydetail?.BillingAddress ?? "", fSmall));
                    var invToCell = new PdfPCell(invToPhrase);
                    invToCell.Border = Rectangle.NO_BORDER;
                    invToCell.Padding = 4f;
                    ackInner.AddCell(invToCell);

                    // Invoice Details
                    var invDetPhrase = new Phrase(14f);
                    invDetPhrase.Add(new Chunk("Invoice Details:\n", fBold));
                    invDetPhrase.Add(new Chunk("Invoice No. : " + (Bill.InvoiceNumber?.ToString() ?? "") + "\n", fSmall));
                    invDetPhrase.Add(new Chunk("Invoice date : " + invDateStr + "\n", fSmall));
                    invDetPhrase.Add(new Chunk("Invoice Amount : \u20B9 " + Bill.FinalAmount.ToString("0.00"), fRupeeSmall));
                    var invDetCell = new PdfPCell(invDetPhrase);
                    invDetCell.Border = Rectangle.NO_BORDER;
                    invDetCell.Padding = 4f;
                    ackInner.AddCell(invDetCell);

                    // Receiver Seal
                    var recPhrase = new Phrase(14f);
                    recPhrase.Add(new Chunk("\n\n---------------------\nReceiver's Seal & Sign", fSmall));
                    var recCell = new PdfPCell(recPhrase);
                    recCell.Border = Rectangle.NO_BORDER;
                    recCell.Padding = 4f;
                    recCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    ackInner.AddCell(recCell);

                    var ackInnerCell = new PdfPCell(ackInner);
                    ackInnerCell.Border = Rectangle.NO_BORDER;
                    ackSection.AddCell(ackInnerCell);

                    doc.Add(ackSection);

                    doc.Close();

                    byte[] bytes = stream.ToArray();

                    // Save PDF to wwwroot
                    string pdfFolderPath = Path.Combine(_env.WebRootPath, "DataContainer", "GeneratedInvoices");
                    if (!Directory.Exists(pdfFolderPath))
                        Directory.CreateDirectory(pdfFolderPath);

                    string fileName = $"DrNote_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    string fullFilePath = Path.Combine(pdfFolderPath, fileName);
                    await File.WriteAllBytesAsync(fullFilePath, bytes);

                    return $"/DataContainer/GeneratedInvoices/{fileName}";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try { if (doc.IsOpen()) doc.Close(); } catch { }
            }

            return string.Empty;
        }

        // ===== Helper methods for PDF cell creation =====
        private static void AddHeaderCell(PdfPTable table, string text, Font font, BaseColor bg, BaseColor border)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = bg;
            cell.BorderColor = border;
            cell.Padding = 4f;
            table.AddCell(cell);
        }

        private static void AddContentCell(PdfPTable table, Phrase phrase, BaseColor border)
        {
            var cell = new PdfPCell(phrase);
            cell.BorderColor = border;
            cell.Padding = 5f;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);
        }

        private static void AddItemCell(PdfPTable table, string text, Font font, BaseColor border, int align)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.BorderColor = border;
            cell.Padding = 3f;
            cell.HorizontalAlignment = align;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);
        }

        private static void AddRupeeCell(PdfPTable table, string amount, Font font, BaseColor border)
        {
            var cell = new PdfPCell(new Phrase("\u20B9 " + amount, font));
            cell.BorderColor = border;
            cell.Padding = 3f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);
        }

        private static void AddSummaryRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont, BaseColor border, bool highlight)
        {
            var lbl = new PdfPCell(new Phrase(label, labelFont));
            lbl.BorderColor = border;
            lbl.Padding = 3f;
            if (highlight) lbl.BackgroundColor = new BaseColor(255, 243, 205);
            table.AddCell(lbl);

            var val = new PdfPCell(new Phrase(value, valueFont));
            val.BorderColor = border;
            val.Padding = 3f;
            val.HorizontalAlignment = Element.ALIGN_RIGHT;
            if (highlight) val.BackgroundColor = new BaseColor(255, 243, 205);
            table.AddCell(val);
        }

        // ===== Dashed border cell event =====
        private class DashedBorderEvent : IPdfPCellEvent
        {
            public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
            {
                PdfContentByte canvas = canvases[PdfPTable.LINECANVAS];
                canvas.SaveState();
                canvas.SetLineDash(3, 3);
                canvas.MoveTo(position.Left, position.Bottom);
                canvas.LineTo(position.Right, position.Bottom);
                canvas.Stroke();
                canvas.RestoreState();
            }
        }

        // ===== Data-fetching methods (UNCHANGED) =====

        public async Task<PurchaseBill?> GetBillByIdForPdf(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var billQuery = @"
        SELECT 
            id, bill_number, bill_date, stateid, state_of_supply, phone_no, po_no, po_date,
            eway_bill_no, transport_name, delivery_location, vehicle_number, delivery_date,
            payment_type, description, image_path, round_off, total, paidreciveamount,
            created_date, tradedocumenttypesid, partyid, orderstatusid, duedate, orderno,
            orderdate, challanno, challandate, iscredit, billingname, billingaddress,
            shippingaddress, invoicenumber, invoicedate, ""time"", paymenttermid, field5,
            field6, documentpath, noofcopi, discount_percent, discount_amount, tax_percentage,
            tax_amount, shipping_amount, packing_amount, adjustment_amount, tdstcs_percentage,
            tdstcs_amount, isroundoff, final_amount, tcstdstype, isreceive, returnno
        FROM tradedocuments
        WHERE id = @Id";

            using var billCommand = new NpgsqlCommand(billQuery, connection);
            billCommand.Parameters.AddWithValue("@Id", id);

            using var billReader = await billCommand.ExecuteReaderAsync();

            if (!await billReader.ReadAsync())
                return null;
            int timeOrdinal = billReader.GetOrdinal("time");

            var bill = new PurchaseBill
            {
                Id = billReader.GetInt32("id"),
                BillNumber = billReader.IsDBNull("bill_number") ? "" : billReader.GetString("bill_number"),
                BillDate = billReader.IsDBNull("bill_date") ? DateTime.MinValue : billReader.GetDateTime("bill_date"),
                StateId = billReader.IsDBNull("stateid") ? 0 : billReader.GetInt32("stateid"),
                StateOfSupply = billReader.IsDBNull("state_of_supply") ? "" : billReader.GetString("state_of_supply"),
                PhoneNo = billReader.IsDBNull("phone_no") ? "" : billReader.GetString("phone_no"),
                PONo = billReader.IsDBNull("po_no") ? "" : billReader.GetString("po_no"),
                PODate = billReader.IsDBNull("po_date") ? DateTime.MinValue : billReader.GetDateTime("po_date"),
                EWayBillNo = billReader.IsDBNull("eway_bill_no") ? "" : billReader.GetString("eway_bill_no"),
                TransportName = billReader.IsDBNull("transport_name") ? "" : billReader.GetString("transport_name"),
                DeliveryLocation = billReader.IsDBNull("delivery_location") ? "" : billReader.GetString("delivery_location"),
                VehicleNumber = billReader.IsDBNull("vehicle_number") ? "" : billReader.GetString("vehicle_number"),
                DeliveryDate = billReader.IsDBNull("delivery_date") ? DateTime.MinValue : billReader.GetDateTime("delivery_date"),
                PaymentType = billReader.IsDBNull("payment_type") ? "" : billReader.GetString("payment_type"),
                Description = billReader.IsDBNull("description") ? "" : billReader.GetString("description"),
                ImagePath = billReader.IsDBNull("image_path") ? "" : billReader.GetString("image_path"),
                RoundOffValue = billReader.IsDBNull("round_off") ? 0 : billReader.GetDecimal("round_off"),
                Total = billReader.IsDBNull("total") ? 0 : billReader.GetDecimal("total"),
                paidReciveamount = billReader.IsDBNull("paidreciveamount") ? 0 : billReader.GetDecimal("paidreciveamount"),
                CreatedDate = billReader.IsDBNull("created_date") ? DateTime.MinValue : billReader.GetDateTime("created_date"),
                PartyId = billReader.IsDBNull("partyid") ? 0 : billReader.GetInt32("partyid"),
                DueDate = billReader.IsDBNull("duedate") ? DateTime.MinValue : billReader.GetDateTime("duedate"),
                OrderNo = billReader.IsDBNull("orderno") ? "" : billReader.GetString("orderno"),
                OrderDate = billReader.IsDBNull("orderdate") ? DateTime.MinValue : billReader.GetDateTime("orderdate"),
                ChallanNo = billReader.IsDBNull("challanno") ? "" : billReader.GetString("challanno"),
                Challandate = billReader.IsDBNull("challandate") ? DateTime.MinValue : billReader.GetDateTime("challandate"),
                IsCredit = billReader.IsDBNull("iscredit") ? false : billReader.GetBoolean("iscredit"),
                BillingName = billReader.IsDBNull("billingname") ? "" : billReader.GetString("billingname"),
                BillingAddress = billReader.IsDBNull("billingaddress") ? "" : billReader.GetString("billingaddress"),
                ShippingAddress = billReader.IsDBNull("shippingaddress") ? "" : billReader.GetString("shippingaddress"),
                InvoiceNumber = billReader.IsDBNull("invoicenumber") ? 0 : billReader.GetInt32("invoicenumber"),
                InvoiceDate = billReader.IsDBNull("invoicedate") ? DateTime.MinValue : billReader.GetDateTime("invoicedate"),
                PaymentTermId = billReader.IsDBNull("paymenttermid") ? 0 : billReader.GetInt32("paymenttermid"),
                Field5 = billReader.IsDBNull("field5") ? "" : billReader.GetString("field5"),
                Field6 = billReader.IsDBNull("field6") ? "" : billReader.GetString("field6"),
                DocumentPath = billReader.IsDBNull("documentpath") ? "" : billReader.GetString("documentpath"),
                NoOfCopi = billReader.IsDBNull("noofcopi") ? 0 : billReader.GetInt32("noofcopi"),
                DiscountPercent = billReader.IsDBNull("discount_percent") ? 0 : billReader.GetDecimal("discount_percent"),
                DiscountAmount = billReader.IsDBNull("discount_amount") ? 0 : billReader.GetDecimal("discount_amount"),
                TaxPercentage = billReader.IsDBNull("tax_percentage") ? 0 : billReader.GetDecimal("tax_percentage"),
                TaxAmount = billReader.IsDBNull("tax_amount") ? 0 : billReader.GetDecimal("tax_amount"),
                ShippingAmount = billReader.IsDBNull("shipping_amount") ? 0 : billReader.GetDecimal("shipping_amount"),
                PackingAmount = billReader.IsDBNull("packing_amount") ? 0 : billReader.GetDecimal("packing_amount"),
                AdjustmentAmount = billReader.IsDBNull("adjustment_amount") ? 0 : billReader.GetDecimal("adjustment_amount"),
                TdsTcsPercentage = billReader.IsDBNull("tdstcs_percentage") ? 0 : billReader.GetDecimal("tdstcs_percentage"),
                TdsTcsAmount = billReader.IsDBNull("tdstcs_amount") ? 0 : billReader.GetDecimal("tdstcs_amount"),
                IsRoundOff = billReader.IsDBNull("isroundoff") ? false : billReader.GetBoolean("isroundoff"),
                FinalAmount = billReader.IsDBNull("final_amount") ? 0 : billReader.GetDecimal("final_amount"),
                IsReceive = billReader.IsDBNull("isreceive") ? false : billReader.GetBoolean("isreceive"),
                ReturnNo = billReader.IsDBNull("returnno") ? 0 : billReader.GetDecimal("returnno"),
                Time = billReader.IsDBNull(timeOrdinal) ? TimeSpan.MinValue : billReader.GetTimeSpan(timeOrdinal)
            };

            await billReader.CloseAsync();

            var itemsQuery = @"
             SELECT 
                        td.id,
                        td.tradedocumentsid,
                        td.itemid,
                        td.categoryid,
                        td.serialno,
                        td.batchno,
                        td.modelno,
                        td.expirydate,
                        td.mfgdate,
                        td.item,
                        td.quantity,
                        td.unit,
                        td.price_per_unit,
                        td.discount_percentage,
                        td.discount_amount,
                        td.created_on,
                        td.tax_amount,
                        td.tax_percentage,
                        td.total_amount,
                        td.itemcode,
                        td.addcessamount,
                        bi.item_name as itemname
                    FROM tradedocumentitems AS td 
                    left join billitem as bi on td.itemid = bi.id
                                WHERE tradedocumentsid = @BillId";

            using var itemsCommand = new NpgsqlCommand(itemsQuery, connection);
            itemsCommand.Parameters.AddWithValue("@BillId", id);

            using var itemsReader = await itemsCommand.ExecuteReaderAsync();

            while (await itemsReader.ReadAsync())
            {
                bill.BillItems.Add(new PurchaseBillItem
                {
                    Id = itemsReader.GetInt32("id"),
                    BillId = itemsReader.GetInt32("tradedocumentsid"),
                    ItemId = itemsReader.IsDBNull("itemid") ? 0 : itemsReader.GetInt32("itemid"),
                    categoryid = itemsReader.IsDBNull("categoryid") ? 0 : itemsReader.GetInt32("categoryid"),
                    serialno = itemsReader.IsDBNull("serialno") ? "" : itemsReader.GetString("serialno"),
                    batchno = itemsReader.IsDBNull("batchno") ? "" : itemsReader.GetString("batchno"),
                    modelno = itemsReader.IsDBNull("modelno") ? "" : itemsReader.GetString("modelno"),
                    ExpiryDate = itemsReader.IsDBNull("expirydate") ? DateTime.MinValue : itemsReader.GetDateTime("expirydate"),
                    Quantity = itemsReader.IsDBNull("quantity") ? 0 : itemsReader.GetDecimal("quantity"),
                    Unit = itemsReader.IsDBNull("unit") ? "" : itemsReader.GetString("unit"),
                    PricePerUnit = itemsReader.IsDBNull("price_per_unit") ? 0 : itemsReader.GetDecimal("price_per_unit"),
                    DiscountPercentage = itemsReader.IsDBNull("discount_percentage") ? 0 : itemsReader.GetDecimal("discount_percentage"),
                    DiscountAmount = itemsReader.IsDBNull("discount_amount") ? 0 : itemsReader.GetDecimal("discount_amount"),
                    CreatedOn = itemsReader.IsDBNull("created_on") ? DateTime.MinValue : itemsReader.GetDateTime("created_on"),
                    TaxAmount = itemsReader.IsDBNull("tax_amount") ? 0 : itemsReader.GetDecimal("tax_amount"),
                    TaxPercentage = itemsReader.IsDBNull("tax_percentage") ? 0 : itemsReader.GetDecimal("tax_percentage"),
                    TotalAmount = itemsReader.IsDBNull("total_amount") ? 0 : itemsReader.GetDecimal("total_amount"),
                    ItemCode = itemsReader.IsDBNull("itemcode") ? "" : itemsReader.GetString("itemcode"),
                    AddCessAmount = itemsReader.IsDBNull("addcessamount") ? 0 : itemsReader.GetDecimal("addcessamount"),
                    Item = itemsReader.IsDBNull("itemname") ? "" : itemsReader.GetString("itemname"),
                });
            }

            return bill;
        }

        public PartyModel PartDetailForPdfById(int id)
        {
            PartyModel model = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT ps.* , ss.name , ss.code  FROM parties as ps left join states as ss on ss.id = ps.stateid  WHERE ps.id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new PartyModel
                            {
                                Id = id,
                                PartyName = reader["party_name"].ToString(),
                                GSTIN = reader["gstin"].ToString(),
                                PhoneNumber = reader["phone_number"].ToString(),
                                GSTType = reader["gst_type"].ToString(),
                                State = reader["state"].ToString(),
                                Email = reader["email"].ToString(),
                                BillingAddress = reader["billing_address"].ToString(),
                                ShippingAddress = reader["shipping_address"].ToString(),
                                IsShippingDisabled = Convert.ToBoolean(reader["is_shipping_disabled"]),
                                OpeningBalance = reader["opening_balance"] != DBNull.Value ? Convert.ToDecimal(reader["opening_balance"]) : (decimal?)null,
                                AsOfDate = reader["as_of_date"] != DBNull.Value ? Convert.ToDateTime(reader["as_of_date"]) : (DateTime?)null,
                                HasCustomCreditLimit = Convert.ToBoolean(reader["has_custom_credit_limit"]),
                                CreditLimit = reader["credit_limit"] != DBNull.Value ? Convert.ToDecimal(reader["credit_limit"]) : (decimal?)null,
                                AdditionalField1Enabled = Convert.ToBoolean(reader["additional_field1_enabled"]),
                                AdditionalField1Value = reader["additional_field1_value"]?.ToString(),
                                AdditionalField2Enabled = Convert.ToBoolean(reader["additional_field2_enabled"]),
                                AdditionalField2Value = reader["additional_field2_value"]?.ToString(),
                                AdditionalField3Enabled = Convert.ToBoolean(reader["additional_field3_enabled"]),
                                AdditionalField3Value = reader["additional_field3_value"]?.ToString(),
                                AdditionalField4Enabled = Convert.ToBoolean(reader["additional_field4_enabled"]),
                                AdditionalField4Value = reader["additional_field4_value"] != DBNull.Value ? Convert.ToDateTime(reader["additional_field4_value"]) : (DateTime?)null,
                                PartyGroup = reader["partygroup"] != DBNull.Value ? Convert.ToString(reader["partygroup"]) : string.Empty,
                                PartyGroupId = reader["partygroupid"] != DBNull.Value ? Convert.ToInt32(reader["partygroupid"]) : 0,
                                StateName = reader["name"]?.ToString(),
                                StateCode = reader["code"]?.ToString(),
                                StateId = Convert.ToInt32(reader["stateid"])
                            };
                        }
                    }
                }
            }

            return model;
        }

        public BankAccountModel GetBankForPdf()
        {
            BankAccountModel Model = new BankAccountModel();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                SELECT  
                    id                      AS ""Id"",
                    account_display_name    AS ""AccountDisplayName"",
                    opening_balance         AS ""OpeningBalance"",
                    as_of_date              AS ""AsOfDate"",
                    print_upi_qr             AS ""PrintUPIQrCode"",
                    print_bank_details      AS ""PrintBankDetails"",
                    account_number          AS ""AccountNumber"",
                    ifsc_code               AS ""IFSCCode"",
                    upi_id                  AS ""UPIID"",
                    bank_name               AS ""BankName"",
                    account_holder_name     AS ""AccountHolderName""
                FROM public.extended_bank_accounts ; ";

                Model = conn
                    .QuerySql<BankAccountModel>(query, new
                    {

                    }).FirstOrDefault() ?? new BankAccountModel();
            }

            return Model;
        }

        public static String ConvertAmount(double amount)
        {
            try
            {
                Int64 amount_int = (Int64)amount;
                Int64 amount_dec = (Int64)Math.Round((amount - (double)(amount_int)) * 100);
                if (amount_dec == 0)
                    return Convertvalue(amount_int) + " Only.";
                else
                    return Convertvalue(amount_int) + " Point " + Convertvalue(amount_dec) + " Only.";
            }
            catch (Exception e) { }
            return "";
        }

        public static String Convertvalue(Int64 i)
        {
            String[] units = { "Zero", "One", "Two", "Three",
                "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
                "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                "Seventeen", "Eighteen", "Nineteen" };
            String[] tens = { "", "", "Twenty", "Thirty", "Forty",
                "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            if (i < 20) return units[i];
            if (i < 100) return tens[i / 10] + ((i % 10 > 0) ? " " + Convertvalue(i % 10) : "");
            if (i < 1000) return units[i / 100] + " Hundred" + ((i % 100 > 0) ? " And " + Convertvalue(i % 100) : "");
            if (i < 100000) return Convertvalue(i / 1000) + " Thousand " + ((i % 1000 > 0) ? " " + Convertvalue(i % 1000) : "");
            if (i < 10000000) return Convertvalue(i / 100000) + " Lakh " + ((i % 100000 > 0) ? " " + Convertvalue(i % 100000) : "");
            if (i < 1000000000) return Convertvalue(i / 10000000) + " Crore " + ((i % 10000000 > 0) ? " " + Convertvalue(i % 10000000) : "");
            return Convertvalue(i / 1000000000) + " Arab " + ((i % 1000000000 > 0) ? " " + Convertvalue(i % 1000000000) : "");
        }
    }
}

