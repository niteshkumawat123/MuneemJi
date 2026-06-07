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
    public interface IDeliveryChallanPdf
    {
        Task<string> GetDeliveryChallanPdfById(int id, IWebHostEnvironment _env);
    }

    public class DeliveryChallanPdf : IDeliveryChallanPdf
    {
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public DeliveryChallanPdf() { }

        public async Task<string> GetDeliveryChallanPdfById(int id, IWebHostEnvironment _env)
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

            BaseColor borderClr = new BaseColor(169, 169, 169);
            BaseColor darkBrown = new BaseColor(78, 42, 10);
            BaseColor totalRowBg = new BaseColor(255, 243, 205);
            Font fWhiteSmallBold = new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE);
            Font fWhiteBold = new Font(bfArial, 8f, Font.BOLD, BaseColor.WHITE);
            Font fWhiteRupeeSmall = new Font(bfRupee, 7f, Font.BOLD, BaseColor.WHITE);

            Document doc = new Document(PageSize.A4, 36, 36, 88, 36);
            try
            {
                MemoryStream stream = new MemoryStream();
                using (PdfWriter wri = PdfWriter.GetInstance(doc, stream))
                {
                    wri.CloseStream = false;
                    wri.PageEvent = new SalesInvoicesPdf.PdfPageEvents(companydetail, _env);

                    doc.Open();
                    doc.NewPage();

                    // ===== PAGE 1 =====

                    // Title
                    var titlePara = new Paragraph(new Chunk("Delivery Challan", new Font(bfArial, 14f, Font.BOLD, darkBrown)));
                    titlePara.Alignment = Element.ALIGN_CENTER;
                    titlePara.SpacingAfter = 8f;
                    doc.Add(titlePara);

                    // ===== Info Grid: Delivery Challan For | Ship To | Transportation Details | Challan Details =====
                    PdfPTable infoGrid = new PdfPTable(4);
                    infoGrid.WidthPercentage = 100;
                    infoGrid.SetWidths(new float[] { 27f, 25f, 24f, 24f });

                    // Header row
                    AddHeaderCell(infoGrid, "Delivery Challan For", fSmallBold, darkBrown);
                    AddHeaderCell(infoGrid, "Ship To", fSmallBold, darkBrown);
                    AddHeaderCell(infoGrid, "Transportation Details", fSmallBold, darkBrown);
                    AddHeaderCell(infoGrid, "Challan Details", fSmallBold, darkBrown);

                    // Delivery Challan For content
                    var billToPhrase = new Phrase(12f);
                    billToPhrase.Add(new Chunk((partydetail?.PartyName ?? "N/A") + "\n", fBold));
                    billToPhrase.Add(new Chunk((partydetail?.BillingAddress ?? "") + "\n", fSmall));
                    if (!string.IsNullOrEmpty(partydetail?.PhoneNumber))
                        billToPhrase.Add(new Chunk("Contact No. : " + partydetail.PhoneNumber + "\n", fSmall));
                    if (!string.IsNullOrEmpty(partydetail?.GSTIN))
                        billToPhrase.Add(new Chunk("GSTIN : " + partydetail.GSTIN + "\n", fSmall));
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
                    transportPhrase.Add(new Chunk("Transport Name:\n", fSmall));
                    transportPhrase.Add(new Chunk("Vehicle Number:\n", fSmall));
                    transportPhrase.Add(new Chunk("Delivery Date: " + deliveryDateStr + "\n", fSmall));
                    transportPhrase.Add(new Chunk("Delivery Location:\n", fSmall));
                    transportPhrase.Add(new Chunk("Field 5:\n", fSmall));
                    transportPhrase.Add(new Chunk("Field 6:", fSmall));
                    AddContentCell(infoGrid, transportPhrase, borderClr);

                    // Challan Details content
                    string challanDateStr = Bill.Challandate != DateTime.MinValue
                        ? Bill.Challandate.ToString("dd-MM-yyyy") : (Bill.BillDate != DateTime.MinValue ? Bill.BillDate.ToString("dd-MM-yyyy") : "");
                    string timeStr = Bill.Time.HasValue && Bill.Time.Value != TimeSpan.MinValue
                        ? Bill.Time.Value.ToString(@"hh\:mm") + " " + (Bill.Time.Value.Hours >= 12 ? "PM" : "AM") : "";
                    var challanPhrase = new Phrase(12f);
                    challanPhrase.Add(new Chunk("Challan No. : " + (Bill.ChallanNo ?? Bill.BillNumber ?? "") + "\n", fSmall));
                    challanPhrase.Add(new Chunk("Date : " + challanDateStr + "\n", fSmall));
                    challanPhrase.Add(new Chunk("Time : " + timeStr + "\n", fSmall));
                    challanPhrase.Add(new Chunk("Place of supply: " + (Bill.StateOfSupply ?? ""), fSmall));
                    var challanCell = new PdfPCell(challanPhrase);
                    challanCell.BorderColor = borderClr;
                    challanCell.Padding = 5f;
                    challanCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    challanCell.VerticalAlignment = Element.ALIGN_TOP;
                    infoGrid.AddCell(challanCell);

                    doc.Add(infoGrid);

                    // ===== Items Table (16 columns) =====
                    var validItems = Bill.BillItems?.Where(x => x.PricePerUnit > 0).ToList() ?? new List<PurchaseBillItem>();

                    float[] itemWidths = { 2.5f, 8f, 5.5f, 5f, 4f, 5f, 4f, 5.5f, 5f, 5.5f, 5.5f, 5.5f, 5.5f, 5f, 5.5f, 6f };
                    PdfPTable itemsTable = new PdfPTable(16);
                    itemsTable.WidthPercentage = 100;
                    itemsTable.SetWidths(itemWidths);
                    itemsTable.SpacingBefore = 5f;

                    string[] headers = { "#", "Item name", "Item Code", "HSN/ SAC", "Colour", "MRP", "Quantity", "Price/ Unit", "Discount", "Taxable\nPrice/ Unit", "Taxable\namount", "CGST", "SGST", "Ad. CESS", "Final Rate", "Amount" };
                    foreach (var h in headers)
                    {
                        var hCell = new PdfPCell(new Phrase(h, fWhiteSmallBold));
                        hCell.BackgroundColor = darkBrown;
                        hCell.BorderColor = darkBrown;
                        hCell.Padding = 4f;
                        hCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        hCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        itemsTable.AddCell(hCell);
                    }

                    decimal totalQty = 0, totalTaxableAmt = 0, totalCgst = 0, totalSgst = 0, totalCess = 0, totalAmount = 0;
                    int rowNum = 1;
                    foreach (var item in validItems)
                    {
                        decimal taxableAmt = item.PricePerUnit * item.Quantity - item.DiscountAmount;
                        decimal taxablePerUnit = item.Quantity != 0 ? taxableAmt / item.Quantity : 0;
                        decimal halfTaxPct = item.TaxPercentage / 2;
                        decimal cgstAmt = taxableAmt * halfTaxPct / 100;
                        decimal sgstAmt = taxableAmt * halfTaxPct / 100;
                        decimal cessAmt = item.AddCessAmount ?? 0;
                        decimal finalRate = taxablePerUnit + (item.Quantity != 0 ? (cgstAmt + sgstAmt + cessAmt) / item.Quantity : 0);
                        decimal amount = item.TotalAmount ?? 0;

                        totalQty += item.Quantity;
                        totalTaxableAmt += taxableAmt;
                        totalCgst += cgstAmt;
                        totalSgst += sgstAmt;
                        totalCess += cessAmt;
                        totalAmount += amount;

                        // Item name with note
                        var itemNamePhrase = new Phrase();
                        itemNamePhrase.Add(new Chunk((item.Item ?? "") + "\n", fSmall));
                        if (!string.IsNullOrEmpty(item.serialno))
                            itemNamePhrase.Add(new Chunk("(" + item.serialno + ")\n", new Font(bfArial, 6f, Font.ITALIC)));
                        itemNamePhrase.Add(new Chunk("*Tax calculated on MRP", new Font(bfArial, 5.5f, Font.ITALIC)));

                        AddItemCell(itemsTable, rowNum.ToString(), fSmall, borderClr, Element.ALIGN_CENTER);
                        var nameCell = new PdfPCell(itemNamePhrase);
                        nameCell.BorderColor = borderClr;
                        nameCell.Padding = 3f;
                        nameCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        itemsTable.AddCell(nameCell);
                        AddItemCell(itemsTable, item.ItemCode ?? "", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, item.HSNCode ?? "", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, item.modelno ?? "", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddRupeeCell(itemsTable, item.PricePerUnit.ToString("N2"), fRupeeSmall, borderClr);
                        AddItemCell(itemsTable, item.Quantity.ToString("0.##"), fSmall, borderClr, Element.ALIGN_CENTER);
                        AddRupeeCell(itemsTable, item.PricePerUnit.ToString("N2"), fRupeeSmall, borderClr);
                        // Discount
                        var discPhrase = new Phrase();
                        discPhrase.Add(new Chunk("\u20B9 " + item.DiscountAmount.ToString("N2") + "\n", fRupeeSmall));
                        if (item.DiscountPercentage > 0)
                            discPhrase.Add(new Chunk("(" + item.DiscountPercentage.ToString("0.##") + "%)", fSmall));
                        var discCell = new PdfPCell(discPhrase);
                        discCell.BorderColor = borderClr;
                        discCell.Padding = 3f;
                        discCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(discCell);
                        // Taxable Price/Unit
                        AddRupeeCell(itemsTable, taxablePerUnit.ToString("N2"), fRupeeSmall, borderClr);
                        // Taxable amount
                        AddRupeeCell(itemsTable, taxableAmt.ToString("N2"), fRupeeSmall, borderClr);
                        // CGST
                        var cgstPhrase = new Phrase();
                        cgstPhrase.Add(new Chunk("\u20B9 " + cgstAmt.ToString("N2") + "\n", fRupeeSmall));
                        cgstPhrase.Add(new Chunk("(" + halfTaxPct.ToString("0.##") + "%)", fSmall));
                        var cgstCell = new PdfPCell(cgstPhrase);
                        cgstCell.BorderColor = borderClr;
                        cgstCell.Padding = 3f;
                        cgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(cgstCell);
                        // SGST
                        var sgstPhrase = new Phrase();
                        sgstPhrase.Add(new Chunk("\u20B9 " + sgstAmt.ToString("N2") + "\n", fRupeeSmall));
                        sgstPhrase.Add(new Chunk("(" + halfTaxPct.ToString("0.##") + "%)", fSmall));
                        var sgstCell = new PdfPCell(sgstPhrase);
                        sgstCell.BorderColor = borderClr;
                        sgstCell.Padding = 3f;
                        sgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(sgstCell);
                        // Ad. CESS
                        AddRupeeCell(itemsTable, cessAmt.ToString("N2"), fRupeeSmall, borderClr);
                        // Final Rate
                        AddRupeeCell(itemsTable, finalRate.ToString("N2"), fRupeeSmall, borderClr);
                        // Amount
                        AddRupeeCell(itemsTable, amount.ToString("N2"), fRupeeSmall, borderClr);
                        rowNum++;
                    }

                    // Total row
                    var totLabelCell = new PdfPCell(new Phrase("Total", fWhiteBold));
                    totLabelCell.Colspan = 6;
                    totLabelCell.BackgroundColor = darkBrown;
                    totLabelCell.BorderColor = darkBrown;
                    totLabelCell.Padding = 4f;
                    itemsTable.AddCell(totLabelCell);

                    var totQtyCell = new PdfPCell(new Phrase(totalQty.ToString("0.##"), fWhiteSmallBold));
                    totQtyCell.BackgroundColor = darkBrown;
                    totQtyCell.BorderColor = darkBrown;
                    totQtyCell.Padding = 4f;
                    totQtyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    itemsTable.AddCell(totQtyCell);

                    // blank: Price/Unit, Discount
                    for (int i = 0; i < 2; i++)
                    {
                        var blankCell = new PdfPCell(new Phrase("", fSmall));
                        blankCell.BackgroundColor = darkBrown;
                        blankCell.BorderColor = darkBrown;
                        blankCell.Padding = 4f;
                        itemsTable.AddCell(blankCell);
                    }

                    // Taxable Price/Unit blank
                    var blankTP = new PdfPCell(new Phrase("", fSmall));
                    blankTP.BackgroundColor = darkBrown;
                    blankTP.BorderColor = darkBrown;
                    blankTP.Padding = 4f;
                    itemsTable.AddCell(blankTP);

                    // Taxable amount total
                    var totTaxableCell = new PdfPCell(new Phrase("\u20B9 " + totalTaxableAmt.ToString("N2"), fWhiteRupeeSmall));
                    totTaxableCell.BackgroundColor = darkBrown;
                    totTaxableCell.BorderColor = darkBrown;
                    totTaxableCell.Padding = 4f;
                    totTaxableCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totTaxableCell);

                    // CGST total
                    var totCgstCell = new PdfPCell(new Phrase("\u20B9 " + totalCgst.ToString("N2"), fWhiteRupeeSmall));
                    totCgstCell.BackgroundColor = darkBrown;
                    totCgstCell.BorderColor = darkBrown;
                    totCgstCell.Padding = 4f;
                    totCgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totCgstCell);

                    // SGST total
                    var totSgstCell = new PdfPCell(new Phrase("\u20B9 " + totalSgst.ToString("N2"), fWhiteRupeeSmall));
                    totSgstCell.BackgroundColor = darkBrown;
                    totSgstCell.BorderColor = darkBrown;
                    totSgstCell.Padding = 4f;
                    totSgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totSgstCell);

                    // Ad. CESS total
                    var totCessCell = new PdfPCell(new Phrase("\u20B9 " + totalCess.ToString("N2"), fWhiteRupeeSmall));
                    totCessCell.BackgroundColor = darkBrown;
                    totCessCell.BorderColor = darkBrown;
                    totCessCell.Padding = 4f;
                    totCessCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totCessCell);

                    // Final Rate blank
                    var blankFR = new PdfPCell(new Phrase("", fSmall));
                    blankFR.BackgroundColor = darkBrown;
                    blankFR.BorderColor = darkBrown;
                    blankFR.Padding = 4f;
                    itemsTable.AddCell(blankFR);

                    // Amount total
                    var totAmtCell = new PdfPCell(new Phrase("\u20B9 " + totalAmount.ToString("N2"), fWhiteRupeeSmall));
                    totAmtCell.BackgroundColor = darkBrown;
                    totAmtCell.BorderColor = darkBrown;
                    totAmtCell.Padding = 4f;
                    totAmtCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totAmtCell);

                    doc.Add(itemsTable);

                    // ===== Tax Summary + Amounts (two-column) =====
                    PdfPTable taxAmtOuter = new PdfPTable(2);
                    taxAmtOuter.WidthPercentage = 100;
                    taxAmtOuter.SetWidths(new float[] { 50f, 50f });
                    taxAmtOuter.SpacingBefore = 5f;

                    // LEFT: Tax type table
                    PdfPTable taxTbl = new PdfPTable(4);
                    taxTbl.SetWidths(new float[] { 20f, 30f, 15f, 20f });

                    // Tax table header
                    string[] taxHeaders = { "Tax type", "Taxable amount", "Rate", "Tax amount" };
                    foreach (var th in taxHeaders)
                    {
                        var thCell = new PdfPCell(new Phrase(th, fWhiteSmallBold));
                        thCell.BackgroundColor = darkBrown;
                        thCell.BorderColor = darkBrown;
                        thCell.Padding = 3f;
                        thCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        taxTbl.AddCell(thCell);
                    }

                    // Tax rows per item
                    foreach (var item in validItems)
                    {
                        decimal taxableAmt = item.PricePerUnit * item.Quantity - item.DiscountAmount;
                        decimal halfPct = item.TaxPercentage / 2;
                        decimal sgst = taxableAmt * halfPct / 100;
                        decimal cgst = taxableAmt * halfPct / 100;

                        AddTaxRow(taxTbl, "SGST", taxableAmt, halfPct, sgst, fSmall, fRupeeSmall, borderClr);
                        AddTaxRow(taxTbl, "CGST", taxableAmt, halfPct, cgst, fSmall, fRupeeSmall, borderClr);
                    }

                    // Ad. CESS row
                    if (totalCess > 0)
                    {
                        AddItemCell(taxTbl, "Ad. CESS", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(taxTbl, "", fSmall, borderClr, Element.ALIGN_RIGHT);
                        AddItemCell(taxTbl, "", fSmall, borderClr, Element.ALIGN_CENTER);
                        AddRupeeCell(taxTbl, totalCess.ToString("N2"), fRupeeSmall, borderClr);
                    }

                    var taxLeftCell = new PdfPCell(taxTbl);
                    taxLeftCell.Border = Rectangle.NO_BORDER;
                    taxLeftCell.Padding = 0f;
                    taxAmtOuter.AddCell(taxLeftCell);

                    // RIGHT: Amounts summary
                    PdfPTable amtTbl = new PdfPTable(2);
                    amtTbl.SetWidths(new float[] { 50f, 50f });

                    // "Amounts" header spanning 2 cols
                    var amtHeader = new PdfPCell(new Phrase("Amounts", fWhiteSmallBold));
                    amtHeader.Colspan = 2;
                    amtHeader.BackgroundColor = darkBrown;
                    amtHeader.BorderColor = darkBrown;
                    amtHeader.Padding = 3f;
                    amtHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    amtTbl.AddCell(amtHeader);

                    AddSummaryRow(amtTbl, "Sub Total", "\u20B9 " + totalAmount.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);
                    if (Bill.RoundOffValue != 0)
                        AddSummaryRow(amtTbl, "Round off", "\u20B9 " + Bill.RoundOffValue.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);

                    // Total (bold highlighted)
                    var totLbl = new PdfPCell(new Phrase("Total", fBold));
                    totLbl.BackgroundColor = totalRowBg;
                    totLbl.BorderColor = borderClr;
                    totLbl.Padding = 4f;
                    amtTbl.AddCell(totLbl);
                    var totVal = new PdfPCell(new Phrase("\u20B9 " + Bill.FinalAmount.ToString("N2"), fRupeeBold));
                    totVal.BackgroundColor = totalRowBg;
                    totVal.BorderColor = borderClr;
                    totVal.Padding = 4f;
                    totVal.HorizontalAlignment = Element.ALIGN_RIGHT;
                    amtTbl.AddCell(totVal);

                    AddSummaryRow(amtTbl, "Previous Balance", "\u20B9 " + Bill.ReturnNo.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);
                    AddSummaryRow(amtTbl, "You Saved", "\u20B9 " + Bill.DiscountAmount.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);

                    var amtRightCell = new PdfPCell(amtTbl);
                    amtRightCell.Border = Rectangle.NO_BORDER;
                    amtRightCell.Padding = 0f;
                    taxAmtOuter.AddCell(amtRightCell);

                    doc.Add(taxAmtOuter);

                    // ===== Delivery Challan Amount In Words =====
                    PdfPTable wordsTable = new PdfPTable(1);
                    wordsTable.WidthPercentage = 100;
                    wordsTable.SpacingBefore = 5f;

                    var wordsHeader = new PdfPCell(new Phrase("Delivery Challan Amount In Words", fWhiteSmallBold));
                    wordsHeader.BackgroundColor = darkBrown;
                    wordsHeader.BorderColor = darkBrown;
                    wordsHeader.Padding = 4f;
                    wordsHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(wordsHeader);

                    var wordsVal = new PdfPCell(new Phrase(ConfigControls.ConvertAmountToWords(Bill.FinalAmount), fSmall));
                    wordsVal.BorderColor = borderClr;
                    wordsVal.Padding = 5f;
                    wordsVal.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(wordsVal);

                    doc.Add(wordsTable);

                    // ===== Bank Details + Terms and Conditions =====
                    PdfPTable bankTerms = new PdfPTable(2);
                    bankTerms.WidthPercentage = 100;
                    bankTerms.SetWidths(new float[] { 50f, 50f });
                    bankTerms.SpacingBefore = 5f;

                    // Bank Details header
                    var bankHdr = new PdfPCell(new Phrase("Bank Details", fWhiteSmallBold));
                    bankHdr.BackgroundColor = darkBrown;
                    bankHdr.BorderColor = darkBrown;
                    bankHdr.Padding = 4f;
                    bankTerms.AddCell(bankHdr);

                    // Terms header
                    var termsHdr = new PdfPCell(new Phrase("Terms and Conditions", fWhiteSmallBold));
                    termsHdr.BackgroundColor = darkBrown;
                    termsHdr.BorderColor = darkBrown;
                    termsHdr.Padding = 4f;
                    bankTerms.AddCell(termsHdr);

                    // Bank content (with QR placeholder)
                    var bankPhrase = new Phrase(14f);
                    bankPhrase.Add(new Chunk("Name : " + (BankDetail?.BankName ?? "N/A") + "\n", fSmall));
                    bankPhrase.Add(new Chunk("Account No. : " + (BankDetail?.AccountNumber ?? "N/A") + "\n", fSmall));
                    bankPhrase.Add(new Chunk("IFSC code : " + (BankDetail?.IFSCCode ?? "N/A") + "\n", fSmall));
                    bankPhrase.Add(new Chunk("Account holder's name : " + (BankDetail?.AccountDisplayName ?? "N/A"), fSmall));
                    var bankCell = new PdfPCell(bankPhrase);
                    bankCell.BorderColor = borderClr;
                    bankCell.Padding = 5f;
                    bankCell.MinimumHeight = 60f;
                    bankTerms.AddCell(bankCell);

                    // Terms content
                    var termsCell = new PdfPCell(new Phrase(Bill.Description ?? "", fSmall));
                    termsCell.BorderColor = borderClr;
                    termsCell.Padding = 5f;
                    termsCell.MinimumHeight = 60f;
                    bankTerms.AddCell(termsCell);

                    doc.Add(bankTerms);

                    // ===== PAGE 2 (COMPULSORY): Received By / Delivered By + Authorized Signatory =====
                    doc.NewPage();

                    // Title
                    var title2 = new Paragraph(new Chunk("Delivery Challan", new Font(bfArial, 14f, Font.BOLD, darkBrown)));
                    title2.Alignment = Element.ALIGN_CENTER;
                    title2.SpacingAfter = 15f;
                    doc.Add(title2);

                    // Received By / Delivered By + Signatory (3 columns)
                    PdfPTable page2Tbl = new PdfPTable(3);
                    page2Tbl.WidthPercentage = 100;
                    page2Tbl.SetWidths(new float[] { 35f, 35f, 30f });

                    // Headers
                    var recByHdr = new PdfPCell(new Phrase("Received By", fWhiteSmallBold));
                    recByHdr.BackgroundColor = darkBrown;
                    recByHdr.BorderColor = darkBrown;
                    recByHdr.Padding = 5f;
                    page2Tbl.AddCell(recByHdr);

                    var delByHdr = new PdfPCell(new Phrase("Delivered By", fWhiteSmallBold));
                    delByHdr.BackgroundColor = darkBrown;
                    delByHdr.BorderColor = darkBrown;
                    delByHdr.Padding = 5f;
                    page2Tbl.AddCell(delByHdr);

                    // "For : COMPANY" header in third column
                    var forHdr = new PdfPCell(new Phrase("For : " + (companydetail.BusinessName ?? ""), fBold));
                    forHdr.BorderColor = borderClr;
                    forHdr.Padding = 5f;
                    forHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                    page2Tbl.AddCell(forHdr);

                    // Received By content
                    var recPhrase = new Phrase(16f);
                    recPhrase.Add(new Chunk("Name:\n\n", fSmall));
                    recPhrase.Add(new Chunk("Comment:\n\n", fSmall));
                    recPhrase.Add(new Chunk("Date:\n\n", fSmall));
                    recPhrase.Add(new Chunk("Signature:", fSmall));
                    var recCell = new PdfPCell(recPhrase);
                    recCell.BorderColor = borderClr;
                    recCell.Padding = 8f;
                    recCell.MinimumHeight = 120f;
                    page2Tbl.AddCell(recCell);

                    // Delivered By content
                    var delPhrase = new Phrase(16f);
                    delPhrase.Add(new Chunk("Name:\n\n", fSmall));
                    delPhrase.Add(new Chunk("Comment:\n\n", fSmall));
                    delPhrase.Add(new Chunk("Date:\n\n", fSmall));
                    delPhrase.Add(new Chunk("Signature:", fSmall));
                    var delCell = new PdfPCell(delPhrase);
                    delCell.BorderColor = borderClr;
                    delCell.Padding = 8f;
                    delCell.MinimumHeight = 120f;
                    page2Tbl.AddCell(delCell);

                    // Signature + Authorized Signatory cell
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
                                sigImg.Alignment = Element.ALIGN_CENTER;
                                sigCell = new PdfPCell();
                                sigCell.AddElement(new Paragraph(" ") { SpacingAfter = 10f });
                                sigCell.AddElement(sigImg);
                                var authLabel = new Paragraph("Authorized Signatory", fBold);
                                authLabel.Alignment = Element.ALIGN_CENTER;
                                authLabel.SpacingBefore = 8f;
                                sigCell.AddElement(authLabel);
                            }
                            catch
                            {
                                sigCell = new PdfPCell(new Phrase("\n\n\n\nAuthorized Signatory", fBold));
                            }
                        }
                        else
                        {
                            sigCell = new PdfPCell(new Phrase("\n\n\n\nAuthorized Signatory", fBold));
                        }
                    }
                    else
                    {
                        sigCell = new PdfPCell(new Phrase("\n\n\n\nAuthorized Signatory", fBold));
                    }
                    sigCell.BorderColor = borderClr;
                    sigCell.Padding = 8f;
                    sigCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    sigCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    sigCell.MinimumHeight = 120f;
                    page2Tbl.AddCell(sigCell);

                    doc.Add(page2Tbl);

                    doc.Close();

                    byte[] bytes = stream.ToArray();

                    // Save PDF to wwwroot
                    string pdfFolderPath = Path.Combine(_env.WebRootPath, "DataContainer", "GeneratedInvoices");
                    if (!Directory.Exists(pdfFolderPath))
                        Directory.CreateDirectory(pdfFolderPath);

                    string fileName = $"DeliveryChallan_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
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

        // ===== Helper methods =====
        private static void AddHeaderCell(PdfPTable table, string text, Font font, BaseColor bg)
        {
            var cell = new PdfPCell(new Phrase(text, new Font(font.BaseFont, font.Size, Font.BOLD, BaseColor.WHITE)));
            cell.BackgroundColor = bg;
            cell.BorderColor = bg;
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

        private static void AddTaxRow(PdfPTable table, string taxType, decimal taxableAmt, decimal rate, decimal taxAmt, Font font, Font rupeeFont, BaseColor border)
        {
            AddItemCell(table, taxType, font, border, Element.ALIGN_LEFT);
            var tCell = new PdfPCell(new Phrase("\u20B9 " + taxableAmt.ToString("N2"), rupeeFont));
            tCell.BorderColor = border;
            tCell.Padding = 3f;
            tCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(tCell);
            AddItemCell(table, rate.ToString("0.##") + "%", font, border, Element.ALIGN_CENTER);
            var aCell = new PdfPCell(new Phrase("\u20B9 " + taxAmt.ToString("N2"), rupeeFont));
            aCell.BorderColor = border;
            aCell.Padding = 3f;
            aCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(aCell);
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

