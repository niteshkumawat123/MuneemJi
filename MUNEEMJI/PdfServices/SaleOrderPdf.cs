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
    public interface ISaleOrderPdf
    {
        Task<string> GetSaleOrderPdfById(int id, IWebHostEnvironment _env);
    }

    public class SaleOrderPdf : ISaleOrderPdf
    {
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public SaleOrderPdf() { }

        public async Task<string> GetSaleOrderPdfById(int id, IWebHostEnvironment _env)
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

            // Pre-compute common values
            float lineSpacing = 6f;
            string shipAddr = !string.IsNullOrEmpty(Bill.ShippingAddress) ? Bill.ShippingAddress : (partydetail?.ShippingAddress ?? "NA");
            string deliveryDateStr = Bill.DeliveryDate.HasValue && Bill.DeliveryDate.Value != DateTime.MinValue
                ? Bill.DeliveryDate.Value.ToString("dd-MM-yyyy") : "NA";
            string invDateStr = Bill.InvoiceDate.HasValue && Bill.InvoiceDate.Value != DateTime.MinValue
                ? Bill.InvoiceDate.Value.ToString("dd-MM-yyyy") : "NA";
            string timeStr = Bill.Time.HasValue && Bill.Time.Value != TimeSpan.MinValue
                ? Bill.Time.Value.ToString(@"hh\:mm") + " " + (Bill.Time.Value.Hours >= 12 ? "PM" : "AM") : "NA";
            string poDateStr = Bill.PODate.HasValue && Bill.PODate.Value != DateTime.MinValue
                ? Bill.PODate.Value.ToString("dd-MM-yyyy") : "NA";
            string dueDateStr = Bill.DueDate != DateTime.MinValue
                ? Bill.DueDate.ToString("dd-MM-yyyy") : "NA";
            var validItems = Bill.BillItems?.Where(x => x.PricePerUnit > 0).ToList() ?? new List<PurchaseBillItem>();

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

                    // ==================== PAGE 1 — Main Sale Order ====================
                    // "Sale Order" Title
                    var titlePara = new Paragraph("Sale Order", new Font(bfArial, 14f, Font.BOLD, darkBrown));
                    titlePara.Alignment = Element.ALIGN_CENTER;
                    titlePara.SpacingAfter = 8f;
                    doc.Add(titlePara);

                    // Info Grid (Order From / Ship To / Transportation Details / Order Details)
                    PdfPTable infoGrid = new PdfPTable(4);
                    infoGrid.WidthPercentage = 100;
                    infoGrid.SetWidths(new float[] { 25f, 25f, 25f, 25f });

                    AddHeaderCell(infoGrid, "Order From", fSmallBold, grayBg, borderClr);
                    AddHeaderCell(infoGrid, "Ship To", fSmallBold, grayBg, borderClr);
                    AddHeaderCell(infoGrid, "Transportation Details", fSmallBold, grayBg, borderClr);
                    AddHeaderCell(infoGrid, "Order Details", fSmallBold, grayBg, borderClr);

                    // Order From
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f; cell.VerticalAlignment = Element.ALIGN_TOP;
                        var p1 = new Paragraph(partydetail?.PartyName ?? "NA", fBold); p1.SpacingAfter = lineSpacing; cell.AddElement(p1);
                        var p2 = new Paragraph(string.IsNullOrEmpty(partydetail?.BillingAddress) ? "NA" : partydetail.BillingAddress, fSmall); p2.SpacingAfter = lineSpacing; cell.AddElement(p2);
                        var p3 = new Paragraph("Contact No.: " + (string.IsNullOrEmpty(partydetail?.PhoneNumber) ? "NA" : partydetail.PhoneNumber), fSmall); p3.SpacingAfter = lineSpacing; cell.AddElement(p3);
                        var p4 = new Paragraph("GSTIN: " + (string.IsNullOrEmpty(partydetail?.GSTIN) ? "NA" : partydetail.GSTIN), fSmall); p4.SpacingAfter = lineSpacing; cell.AddElement(p4);
                        var p5 = new Paragraph("State: " + (string.IsNullOrEmpty(partydetail?.StateCode) && string.IsNullOrEmpty(partydetail?.StateName) ? "NA" : (partydetail?.StateCode ?? "") + "-" + (partydetail?.StateName ?? "")), fSmall); cell.AddElement(p5);
                        infoGrid.AddCell(cell);
                    }

                    // Ship To
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f; cell.VerticalAlignment = Element.ALIGN_TOP;
                        var p1 = new Paragraph(string.IsNullOrEmpty(shipAddr) ? "NA" : shipAddr, fSmall); cell.AddElement(p1);
                        infoGrid.AddCell(cell);
                    }

                    // Transportation Details
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f; cell.VerticalAlignment = Element.ALIGN_TOP;
                        var p1 = new Paragraph("Transport Name: " + (string.IsNullOrEmpty(Bill.TransportName) ? "NA" : Bill.TransportName), fSmall); p1.SpacingAfter = lineSpacing; cell.AddElement(p1);
                        var p2 = new Paragraph("Vehicle Number: " + (string.IsNullOrEmpty(Bill.VehicleNumber) ? "NA" : Bill.VehicleNumber), fSmall); p2.SpacingAfter = lineSpacing; cell.AddElement(p2);
                        var p3 = new Paragraph("Delivery Date: " + deliveryDateStr, fSmall); p3.SpacingAfter = lineSpacing; cell.AddElement(p3);
                        var p4 = new Paragraph("Delivery Location: " + (string.IsNullOrEmpty(Bill.DeliveryLocation) ? "NA" : Bill.DeliveryLocation), fSmall); p4.SpacingAfter = lineSpacing; cell.AddElement(p4);
                        var p5 = new Paragraph("Field 5: " + (string.IsNullOrEmpty(Bill.Field5) ? "NA" : Bill.Field5), fSmall); p5.SpacingAfter = lineSpacing; cell.AddElement(p5);
                        var p6 = new Paragraph("Field 6: " + (string.IsNullOrEmpty(Bill.Field6) ? "NA" : Bill.Field6), fSmall); cell.AddElement(p6);
                        infoGrid.AddCell(cell);
                    }

                    // Order Details
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f; cell.VerticalAlignment = Element.ALIGN_TOP;
                        var p1 = new Paragraph("Order No. : " + (Bill.InvoiceNumber?.ToString() ?? "NA"), fSmall); p1.Alignment = Element.ALIGN_RIGHT; p1.SpacingAfter = lineSpacing; cell.AddElement(p1);
                        var p2 = new Paragraph("Date : " + invDateStr, fSmall); p2.Alignment = Element.ALIGN_RIGHT; p2.SpacingAfter = lineSpacing; cell.AddElement(p2);
                        var p3 = new Paragraph("Time : " + timeStr, fSmall); p3.Alignment = Element.ALIGN_RIGHT; p3.SpacingAfter = lineSpacing; cell.AddElement(p3);
                        var p4 = new Paragraph("Place of supply: " + (string.IsNullOrEmpty(Bill.StateOfSupply) ? "NA" : Bill.StateOfSupply), fSmall); p4.Alignment = Element.ALIGN_RIGHT; p4.SpacingAfter = lineSpacing; cell.AddElement(p4);
                        var p5 = new Paragraph("Due Date : " + dueDateStr, fSmall); p5.Alignment = Element.ALIGN_RIGHT; p5.SpacingAfter = lineSpacing; cell.AddElement(p5);
                        var p6 = new Paragraph("PO Date : " + poDateStr, fSmall); p6.Alignment = Element.ALIGN_RIGHT; p6.SpacingAfter = lineSpacing; cell.AddElement(p6);
                        var p7 = new Paragraph("PO Number : " + (string.IsNullOrEmpty(Bill.PONo) ? "NA" : Bill.PONo), fSmall); p7.Alignment = Element.ALIGN_RIGHT; cell.AddElement(p7);
                        infoGrid.AddCell(cell);
                    }

                    doc.Add(infoGrid);

                    // Items Table (16 columns like sales)
                    decimal totalQty = 0, totalTaxableAmt = 0, totalGstAmt = 0, totalAmount = 0, totalAddCess = 0;
                    float[] itemWidths = { 2.5f, 9f, 5f, 5f, 4f, 4f, 5f, 5f, 5f, 5.5f, 5.5f, 5.5f, 5.5f, 5f, 5f, 6f };
                    PdfPTable itemsTable = new PdfPTable(itemWidths);
                    itemsTable.WidthPercentage = 100;
                    itemsTable.SpacingBefore = 5f;

                    string[] headers = { "#", "Item Name", "Item\nCode", "HSN/\nSAC", "Colour", "MRP", "Qty", "Price/\nUnit", "Discount", "Taxable\nPrice/Unit", "Taxable\nAmount", "CGST", "SGST", "Ad.\nCESS", "Final\nRate", "Amount" };
                    foreach (var h in headers)
                    {
                        var hCell = new PdfPCell(new Phrase(h, new Font(bfArial, 6f, Font.BOLD, BaseColor.WHITE)));
                        hCell.BackgroundColor = darkBrown;
                        hCell.BorderColor = darkBrown;
                        hCell.Padding = 3f;
                        hCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        hCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        itemsTable.AddCell(hCell);
                    }

                    int rowNum = 1;
                    decimal totalCgst = 0, totalSgst = 0;
                    foreach (var item in validItems)
                    {
                        decimal taxableAmt = item.PricePerUnit * item.Quantity - item.DiscountAmount;
                        decimal halfTax = item.TaxAmount / 2;
                        decimal taxablePricePerUnit = item.Quantity != 0 ? taxableAmt / item.Quantity : 0;
                        decimal finalRate = taxablePricePerUnit + (item.Quantity != 0 ? item.TaxAmount / item.Quantity : 0);
                        decimal addCess = item.AddCessAmount ?? 0;

                        totalQty += item.Quantity;
                        totalTaxableAmt += taxableAmt;
                        totalGstAmt += item.TaxAmount;
                        totalAmount += item.TotalAmount ?? 0;
                        totalAddCess += addCess;
                        totalCgst += halfTax;
                        totalSgst += halfTax;

                        AddItemCell(itemsTable, rowNum.ToString(), fSmall, borderClr, Element.ALIGN_CENTER);
                        AddItemCell(itemsTable, item.Item ?? "NA", fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, string.IsNullOrEmpty(item.ItemCode) ? "NA" : item.ItemCode, fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, string.IsNullOrEmpty(item.HSNCode) ? "NA" : item.HSNCode, fSmall, borderClr, Element.ALIGN_LEFT);
                        AddItemCell(itemsTable, "NA", fSmall, borderClr, Element.ALIGN_CENTER);
                        AddRupeeCell(itemsTable, (item.MRP ?? 0).ToString("0.00"), fRupeeSmall, borderClr);
                        AddItemCell(itemsTable, item.Quantity.ToString("0.##"), fSmall, borderClr, Element.ALIGN_CENTER);
                        AddRupeeCell(itemsTable, item.PricePerUnit.ToString("0.00"), fRupeeSmall, borderClr);
                        string discStr = item.DiscountPercentage > 0 ? "\u20B9 " + item.DiscountAmount.ToString("0.00") + " (" + item.DiscountPercentage.ToString("0.##") + "%)" : "NA";
                        AddItemCell(itemsTable, discStr, fSmall, borderClr, Element.ALIGN_RIGHT);
                        AddRupeeCell(itemsTable, taxablePricePerUnit.ToString("0.00"), fRupeeSmall, borderClr);
                        AddRupeeCell(itemsTable, taxableAmt.ToString("0.00"), fRupeeSmall, borderClr);
                        // CGST
                        var cgstPhrase = new Phrase();
                        cgstPhrase.Add(new Chunk("\u20B9 " + halfTax.ToString("0.00"), fRupeeSmall));
                        cgstPhrase.Add(new Chunk("\n(" + (item.TaxPercentage / 2).ToString("0.##") + "%)", fSmall));
                        var cgstCell = new PdfPCell(cgstPhrase); cgstCell.BorderColor = borderClr; cgstCell.Padding = 2f; cgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(cgstCell);
                        // SGST
                        var sgstPhrase = new Phrase();
                        sgstPhrase.Add(new Chunk("\u20B9 " + halfTax.ToString("0.00"), fRupeeSmall));
                        sgstPhrase.Add(new Chunk("\n(" + (item.TaxPercentage / 2).ToString("0.##") + "%)", fSmall));
                        var sgstCell = new PdfPCell(sgstPhrase); sgstCell.BorderColor = borderClr; sgstCell.Padding = 2f; sgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(sgstCell);
                        AddRupeeCell(itemsTable, addCess.ToString("0.00"), fRupeeSmall, borderClr);
                        AddRupeeCell(itemsTable, finalRate.ToString("0.00"), fRupeeSmall, borderClr);
                        AddRupeeCell(itemsTable, (item.TotalAmount ?? 0).ToString("0.00"), fRupeeSmall, borderClr);
                        rowNum++;
                    }

                    // Total row
                    var totLabelCell = new PdfPCell(new Phrase("Total", new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE)));
                    totLabelCell.Colspan = 6; totLabelCell.BackgroundColor = darkBrown; totLabelCell.BorderColor = darkBrown; totLabelCell.Padding = 3f;
                    itemsTable.AddCell(totLabelCell);
                    var totQtyCell = new PdfPCell(new Phrase(totalQty.ToString("0.##"), new Font(bfArial, 6f, Font.BOLD, BaseColor.WHITE)));
                    totQtyCell.BackgroundColor = darkBrown; totQtyCell.BorderColor = darkBrown; totQtyCell.Padding = 3f; totQtyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    itemsTable.AddCell(totQtyCell);
                    for (int i = 0; i < 3; i++) { var bc = new PdfPCell(new Phrase("", fSmall)); bc.BackgroundColor = darkBrown; bc.BorderColor = darkBrown; bc.Padding = 3f; itemsTable.AddCell(bc); }
                    var totTaxableCell = new PdfPCell(new Phrase("\u20B9 " + totalTaxableAmt.ToString("0.00"), new Font(bfRupee, 6f, Font.BOLD, BaseColor.WHITE)));
                    totTaxableCell.BackgroundColor = darkBrown; totTaxableCell.BorderColor = darkBrown; totTaxableCell.Padding = 3f; totTaxableCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totTaxableCell);
                    var totCgstCell = new PdfPCell(new Phrase("\u20B9 " + totalCgst.ToString("0.00"), new Font(bfRupee, 6f, Font.BOLD, BaseColor.WHITE)));
                    totCgstCell.BackgroundColor = darkBrown; totCgstCell.BorderColor = darkBrown; totCgstCell.Padding = 3f; totCgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totCgstCell);
                    var totSgstCell = new PdfPCell(new Phrase("\u20B9 " + totalSgst.ToString("0.00"), new Font(bfRupee, 6f, Font.BOLD, BaseColor.WHITE)));
                    totSgstCell.BackgroundColor = darkBrown; totSgstCell.BorderColor = darkBrown; totSgstCell.Padding = 3f; totSgstCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totSgstCell);
                    var totCessCell = new PdfPCell(new Phrase("\u20B9 " + totalAddCess.ToString("0.00"), new Font(bfRupee, 6f, Font.BOLD, BaseColor.WHITE)));
                    totCessCell.BackgroundColor = darkBrown; totCessCell.BorderColor = darkBrown; totCessCell.Padding = 3f; totCessCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totCessCell);
                    { var bc = new PdfPCell(new Phrase("", fSmall)); bc.BackgroundColor = darkBrown; bc.BorderColor = darkBrown; bc.Padding = 3f; itemsTable.AddCell(bc); }
                    var totAmtCell = new PdfPCell(new Phrase("\u20B9 " + totalAmount.ToString("0.00"), new Font(bfRupee, 6f, Font.BOLD, BaseColor.WHITE)));
                    totAmtCell.BackgroundColor = darkBrown; totAmtCell.BorderColor = darkBrown; totAmtCell.Padding = 3f; totAmtCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(totAmtCell);

                    doc.Add(itemsTable);

                    // Tax note
                    var taxNote = new Paragraph("Tax calculated on MRP if applicable", new Font(bfArial, 6f, Font.ITALIC));
                    taxNote.SpacingBefore = 2f; taxNote.SpacingAfter = 5f;
                    doc.Add(taxNote);

                    // Summary section (two columns)
                    PdfPTable summaryOuter = new PdfPTable(2);
                    summaryOuter.WidthPercentage = 100;
                    summaryOuter.SetWidths(new float[] { 50f, 50f });
                    summaryOuter.SpacingBefore = 5f;

                    // LEFT: Tax Summary
                    {
                        var cell = new PdfPCell();
                        cell.Border = Rectangle.NO_BORDER; cell.Padding = 5f;

                        PdfPTable taxSummary = new PdfPTable(4);
                        taxSummary.WidthPercentage = 100;
                        taxSummary.SetWidths(new float[] { 25f, 30f, 20f, 25f });
                        string[] taxHeaders = { "Tax Type", "Taxable Amount", "Rate", "Tax Amount" };
                        foreach (var th in taxHeaders)
                        {
                            var thCell = new PdfPCell(new Phrase(th, new Font(bfArial, 6f, Font.BOLD, BaseColor.WHITE)));
                            thCell.BackgroundColor = darkBrown; thCell.BorderColor = darkBrown; thCell.Padding = 3f; thCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            taxSummary.AddCell(thCell);
                        }
                        if (isDomestic)
                        {
                            decimal halfTax = Bill.TaxAmount / 2;
                            string halfRate = Bill.TaxPercentage > 0 ? (Bill.TaxPercentage / 2).ToString("0.##") + "%" : "NA";
                            AddTaxSummaryRow(taxSummary, "SGST", "\u20B9 " + totalTaxableAmt.ToString("0.00"), halfRate, "\u20B9 " + halfTax.ToString("0.00"), fSmall, borderClr);
                            AddTaxSummaryRow(taxSummary, "CGST", "\u20B9 " + totalTaxableAmt.ToString("0.00"), halfRate, "\u20B9 " + halfTax.ToString("0.00"), fSmall, borderClr);
                        }
                        else
                        {
                            string igstRate = Bill.TaxPercentage > 0 ? Bill.TaxPercentage.ToString("0.##") + "%" : "NA";
                            AddTaxSummaryRow(taxSummary, "IGST", "\u20B9 " + totalTaxableAmt.ToString("0.00"), igstRate, "\u20B9 " + Bill.TaxAmount.ToString("0.00"), fSmall, borderClr);
                        }
                        AddTaxSummaryRow(taxSummary, "Ad. CESS", "\u20B9 " + totalTaxableAmt.ToString("0.00"), "NA", "\u20B9 " + totalAddCess.ToString("0.00"), fSmall, borderClr);
                        cell.AddElement(taxSummary);
                        summaryOuter.AddCell(cell);
                    }

                    // RIGHT: Amounts Summary
                    {
                        PdfPTable summaryTbl = new PdfPTable(2);
                        summaryTbl.SetWidths(new float[] { 55f, 45f });

                        AddSummaryRow(summaryTbl, "Sub Total", "\u20B9 " + totalTaxableAmt.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
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
                        totLbl.BackgroundColor = totalRowBg; totLbl.BorderColor = borderClr; totLbl.Padding = 4f;
                        summaryTbl.AddCell(totLbl);
                        var totVal = new PdfPCell(new Phrase("\u20B9 " + Bill.FinalAmount.ToString("0.00"), fRupeeBold));
                        totVal.BackgroundColor = totalRowBg; totVal.BorderColor = borderClr; totVal.Padding = 4f; totVal.HorizontalAlignment = Element.ALIGN_RIGHT;
                        summaryTbl.AddCell(totVal);

                        AddSummaryRow(summaryTbl, "Advance", "\u20B9 " + Bill.paidReciveamount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                        AddSummaryRow(summaryTbl, "Balance", "\u20B9 " + (Bill.FinalAmount - Bill.paidReciveamount).ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);
                        AddSummaryRow(summaryTbl, "You Saved", "\u20B9 " + Bill.DiscountAmount.ToString("0.00"), fSmall, fRupeeSmall, borderClr, false);

                        var rightCell = new PdfPCell(summaryTbl);
                        rightCell.Border = Rectangle.NO_BORDER; rightCell.Padding = 0f;
                        summaryOuter.AddCell(rightCell);
                    }

                    doc.Add(summaryOuter);

                    // Order Amount In Words + Payment mode
                    PdfPTable wordsTable = new PdfPTable(1);
                    wordsTable.WidthPercentage = 55;
                    wordsTable.HorizontalAlignment = Element.ALIGN_LEFT;
                    wordsTable.SpacingBefore = 5f;

                    var wordsHeader = new PdfPCell(new Phrase("Order Amount In Words", new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE)));
                    wordsHeader.BackgroundColor = darkBrown; wordsHeader.BorderColor = darkBrown; wordsHeader.Padding = 4f; wordsHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(wordsHeader);
                    var wordsVal = new PdfPCell(new Phrase(ConfigControls.ConvertAmountToWords(Bill.FinalAmount), fSmall));
                    wordsVal.BorderColor = borderClr; wordsVal.Padding = 4f; wordsVal.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(wordsVal);
                    var pmHeader = new PdfPCell(new Phrase("Payment mode", new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE)));
                    pmHeader.BackgroundColor = darkBrown; pmHeader.BorderColor = darkBrown; pmHeader.Padding = 4f; pmHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(pmHeader);
                    var pmVal = new PdfPCell(new Phrase(string.IsNullOrEmpty(Bill.PaymentType) ? "NA" : Bill.PaymentType, fSmall));
                    pmVal.BorderColor = borderClr; pmVal.Padding = 4f; pmVal.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(pmVal);

                    doc.Add(wordsTable);

                    // ==================== PAGE 2 — Bank/Terms/Signatory + Acknowledgement ====================
                    doc.NewPage();

                    // "Sale Order" Title on page 2
                    var titlePara2 = new Paragraph("Sale Order", new Font(bfArial, 14f, Font.BOLD, darkBrown));
                    titlePara2.Alignment = Element.ALIGN_CENTER;
                    titlePara2.SpacingAfter = 8f;
                    doc.Add(titlePara2);

                    // Bank Details + Terms + Signatory (3 columns)
                    PdfPTable bottomSection = new PdfPTable(3);
                    bottomSection.WidthPercentage = 100;
                    bottomSection.SetWidths(new float[] { 35f, 30f, 35f });
                    bottomSection.SpacingBefore = 10f;

                    // Bank Details (left)
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f;
                        var h1 = new Paragraph("Bank Details", fBold); h1.SpacingAfter = lineSpacing; cell.AddElement(h1);
                        var pb1 = new Paragraph("Name : " + (BankDetail?.BankName ?? "NA"), fSmall); pb1.SpacingAfter = lineSpacing; cell.AddElement(pb1);
                        var pb2 = new Paragraph("Account No. : " + (BankDetail?.AccountNumber ?? "NA"), fSmall); pb2.SpacingAfter = lineSpacing; cell.AddElement(pb2);
                        var pb3 = new Paragraph("IFSC Code : " + (BankDetail?.IFSCCode ?? "NA"), fSmall); pb3.SpacingAfter = lineSpacing; cell.AddElement(pb3);
                        var pb4 = new Paragraph("Account holder name : " + (BankDetail?.AccountDisplayName ?? "NA"), fSmall); cell.AddElement(pb4);
                        bottomSection.AddCell(cell);
                    }

                    // Terms and Conditions (center)
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f;
                        var h1 = new Paragraph("Terms and Conditions", fBold); h1.SpacingAfter = lineSpacing; cell.AddElement(h1);
                        var t1 = new Paragraph(string.IsNullOrEmpty(Bill.Description) ? "NA" : Bill.Description, fSmall); cell.AddElement(t1);
                        bottomSection.AddCell(cell);
                    }

                    // Authorized Signatory (right)
                    {
                        var cell = new PdfPCell();
                        cell.BorderColor = borderClr; cell.Padding = 5f;
                        var h1 = new Paragraph("For : " + (companydetail.BusinessName ?? "NA"), fBold); h1.Alignment = Element.ALIGN_RIGHT; h1.SpacingAfter = lineSpacing; cell.AddElement(h1);
                        string sigPath = companydetail.SignaturePath;
                        if (!string.IsNullOrEmpty(sigPath))
                        {
                            string fullSigPath = sigPath.StartsWith("/") ? Path.Combine(_env.WebRootPath, sigPath.TrimStart('/')) : sigPath;
                            if (File.Exists(fullSigPath))
                            {
                                try
                                {
                                    iTextSharp.text.Image sigImg = iTextSharp.text.Image.GetInstance(fullSigPath);
                                    sigImg.ScaleToFit(80f, 35f);
                                    var imgPara = new Paragraph(); imgPara.Alignment = Element.ALIGN_RIGHT;
                                    imgPara.Add(new Chunk(sigImg, 0, 0, true));
                                    cell.AddElement(imgPara);
                                }
                                catch { }
                            }
                        }
                        var authLabel = new Paragraph("Authorized Signatory", fBold);
                        authLabel.Alignment = Element.ALIGN_RIGHT; authLabel.SpacingBefore = lineSpacing * 2;
                        cell.AddElement(authLabel);
                        bottomSection.AddCell(cell);
                    }

                    doc.Add(bottomSection);

                    // Dashed Separator
                    PdfPTable dashTable = new PdfPTable(1);
                    dashTable.WidthPercentage = 100; dashTable.SpacingBefore = 10f;
                    var dashCell = new PdfPCell(new Phrase(" "));
                    dashCell.Border = Rectangle.NO_BORDER; dashCell.BorderWidthBottom = 1f; dashCell.BorderColorBottom = BaseColor.BLACK; dashCell.Padding = 2f;
                    dashCell.CellEvent = new DashedBorderEvent();
                    dashTable.AddCell(dashCell);
                    doc.Add(dashTable);

                    // Acknowledgement Section
                    PdfPTable ackSection = new PdfPTable(1);
                    ackSection.WidthPercentage = 100; ackSection.SpacingBefore = 5f;

                    var ackHeading = new PdfPCell(new Phrase("Acknowledgement", fBold));
                    ackHeading.Border = Rectangle.NO_BORDER; ackHeading.HorizontalAlignment = Element.ALIGN_CENTER; ackHeading.PaddingBottom = 3f;
                    ackSection.AddCell(ackHeading);

                    var ackCompany = new PdfPCell(new Phrase(companydetail.BusinessName ?? "NA", fLargeBold));
                    ackCompany.Border = Rectangle.NO_BORDER; ackCompany.HorizontalAlignment = Element.ALIGN_CENTER; ackCompany.PaddingBottom = 8f;
                    ackSection.AddCell(ackCompany);

                    PdfPTable ackInner = new PdfPTable(3);
                    ackInner.SetWidths(new float[] { 35f, 35f, 30f });

                    // Invoice To
                    {
                        var cell = new PdfPCell();
                        cell.Border = Rectangle.NO_BORDER; cell.Padding = 4f;
                        var pi1 = new Paragraph("Invoice To:", fBold); pi1.SpacingAfter = lineSpacing; cell.AddElement(pi1);
                        var pi2 = new Paragraph(partydetail?.PartyName ?? "NA", fBold); pi2.SpacingAfter = lineSpacing; cell.AddElement(pi2);
                        var pi3 = new Paragraph(string.IsNullOrEmpty(partydetail?.BillingAddress) ? "NA" : partydetail.BillingAddress, fSmall); cell.AddElement(pi3);
                        ackInner.AddCell(cell);
                    }

                    // Invoice Details
                    {
                        var cell = new PdfPCell();
                        cell.Border = Rectangle.NO_BORDER; cell.Padding = 4f;
                        var pd1 = new Paragraph("Invoice Details:", fBold); pd1.SpacingAfter = lineSpacing; cell.AddElement(pd1);
                        var pd2 = new Paragraph("Order No. : " + (Bill.InvoiceNumber?.ToString() ?? "NA"), fSmall); pd2.SpacingAfter = lineSpacing; cell.AddElement(pd2);
                        var pd3 = new Paragraph("Order Date : " + invDateStr, fSmall); pd3.SpacingAfter = lineSpacing; cell.AddElement(pd3);
                        var pd4 = new Paragraph("Order Amount : \u20B9 " + Bill.FinalAmount.ToString("0.00"), fRupeeSmall); cell.AddElement(pd4);
                        ackInner.AddCell(cell);
                    }

                    // Receiver Seal
                    {
                        var cell = new PdfPCell();
                        cell.Border = Rectangle.NO_BORDER; cell.Padding = 4f;
                        var pr1 = new Paragraph(" ", fSmall); pr1.SpacingAfter = lineSpacing * 3; cell.AddElement(pr1);
                        var pr2 = new Paragraph("---------------------", fSmall); pr2.Alignment = Element.ALIGN_CENTER; pr2.SpacingAfter = lineSpacing; cell.AddElement(pr2);
                        var pr3 = new Paragraph("Receiver's Seal & Sign", fSmall); pr3.Alignment = Element.ALIGN_CENTER; cell.AddElement(pr3);
                        ackInner.AddCell(cell);
                    }

                    var ackInnerCell = new PdfPCell(ackInner);
                    ackInnerCell.Border = Rectangle.NO_BORDER;
                    ackSection.AddCell(ackInnerCell);

                    doc.Add(ackSection);

                    doc.Close();

                    byte[] bytes = stream.ToArray();

                    string pdfFolderPath = Path.Combine(_env.WebRootPath, "DataContainer", "GeneratedInvoices");
                    if (!Directory.Exists(pdfFolderPath))
                        Directory.CreateDirectory(pdfFolderPath);

                    string fileName = $"SaleOrder_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
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

        private static void AddTaxSummaryRow(PdfPTable table, string taxType, string taxableAmt, string rate, string taxAmt, Font font, BaseColor border)
        {
            var c1 = new PdfPCell(new Phrase(taxType, font)); c1.BorderColor = border; c1.Padding = 2f; table.AddCell(c1);
            var c2 = new PdfPCell(new Phrase(taxableAmt, font)); c2.BorderColor = border; c2.Padding = 2f; c2.HorizontalAlignment = Element.ALIGN_RIGHT; table.AddCell(c2);
            var c3 = new PdfPCell(new Phrase(rate, font)); c3.BorderColor = border; c3.Padding = 2f; c3.HorizontalAlignment = Element.ALIGN_CENTER; table.AddCell(c3);
            var c4 = new PdfPCell(new Phrase(taxAmt, font)); c4.BorderColor = border; c4.Padding = 2f; c4.HorizontalAlignment = Element.ALIGN_RIGHT; table.AddCell(c4);
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

