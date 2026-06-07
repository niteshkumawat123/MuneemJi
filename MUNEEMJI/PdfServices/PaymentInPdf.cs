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
    public interface IPaymentInPdf
    {
        Task<string> GetPaymentInPdfById(int id, IWebHostEnvironment _env);
    }

    public class PaymentInPdf : IPaymentInPdf
    {
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public PaymentInPdf() { }

        public async Task<string> GetPaymentInPdfById(int id, IWebHostEnvironment _env)
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

                    // ===== Title: "Payment Receipt" =====
                    var titlePara = new Paragraph(new Chunk("Payment Receipt", new Font(bfArial, 14f, Font.BOLD, darkBrown)));
                    titlePara.Alignment = Element.ALIGN_CENTER;
                    titlePara.SpacingAfter = 8f;
                    doc.Add(titlePara);

                    // ===== Received From + Receipt Details (2 columns) =====
                    PdfPTable infoGrid = new PdfPTable(2);
                    infoGrid.WidthPercentage = 100;
                    infoGrid.SetWidths(new float[] { 60f, 40f });

                    Font fWhiteSmallBold = new Font(bfArial, 7f, Font.BOLD, BaseColor.WHITE);

                    // Header row
                    var recFromHdr = new PdfPCell(new Phrase("Received From", fWhiteSmallBold));
                    recFromHdr.BackgroundColor = darkBrown;
                    recFromHdr.BorderColor = darkBrown;
                    recFromHdr.Padding = 5f;
                    infoGrid.AddCell(recFromHdr);

                    var recDetHdr = new PdfPCell(new Phrase("Receipt Details", fWhiteSmallBold));
                    recDetHdr.BackgroundColor = darkBrown;
                    recDetHdr.BorderColor = darkBrown;
                    recDetHdr.Padding = 5f;
                    recDetHdr.HorizontalAlignment = Element.ALIGN_RIGHT;
                    infoGrid.AddCell(recDetHdr);

                    // Received From content
                    var recFromPhrase = new Phrase(14f);
                    recFromPhrase.Add(new Chunk((partydetail?.PartyName ?? "N/A") + "\n", fBold));
                    recFromPhrase.Add(new Chunk((partydetail?.BillingAddress ?? "") + "\n", fSmall));
                    if (!string.IsNullOrEmpty(partydetail?.PhoneNumber))
                        recFromPhrase.Add(new Chunk("Contact No. : " + partydetail.PhoneNumber + "\n", fSmall));
                    if (!string.IsNullOrEmpty(partydetail?.GSTIN))
                        recFromPhrase.Add(new Chunk("GSTIN : " + partydetail.GSTIN + "\n", fSmall));
                    recFromPhrase.Add(new Chunk("State: " + (partydetail?.StateCode ?? "") + "-" + (partydetail?.StateName ?? ""), fSmall));
                    var recFromCell = new PdfPCell(recFromPhrase);
                    recFromCell.BorderColor = borderClr;
                    recFromCell.Padding = 5f;
                    recFromCell.MinimumHeight = 80f;
                    infoGrid.AddCell(recFromCell);

                    // Receipt Details content
                    string billDateStr = Bill.BillDate != DateTime.MinValue ? Bill.BillDate.ToString("dd-MM-yyyy") : "";
                    string timeStr = Bill.Time.HasValue && Bill.Time.Value != TimeSpan.MinValue
                        ? Bill.Time.Value.ToString(@"hh\:mm") + " " + (Bill.Time.Value.Hours >= 12 ? "PM" : "AM") : "";
                    var recDetPhrase = new Phrase(14f);
                    recDetPhrase.Add(new Chunk("Receipt No. : " + (Bill.BillNumber ?? "") + "\n", fSmall));
                    recDetPhrase.Add(new Chunk("Date : " + billDateStr + "\n", fSmall));
                    recDetPhrase.Add(new Chunk("Time : " + timeStr, fSmall));
                    var recDetCell = new PdfPCell(recDetPhrase);
                    recDetCell.BorderColor = borderClr;
                    recDetCell.Padding = 5f;
                    recDetCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    recDetCell.VerticalAlignment = Element.ALIGN_TOP;
                    infoGrid.AddCell(recDetCell);

                    doc.Add(infoGrid);

                    // ===== Amounts section (two-column: left empty, right amounts) =====
                    PdfPTable amtOuter = new PdfPTable(2);
                    amtOuter.WidthPercentage = 100;
                    amtOuter.SetWidths(new float[] { 50f, 50f });
                    amtOuter.SpacingBefore = 5f;

                    // Left empty cell
                    var emptyCell = new PdfPCell(new Phrase(""));
                    emptyCell.Border = Rectangle.NO_BORDER;
                    emptyCell.MinimumHeight = 10f;
                    amtOuter.AddCell(emptyCell);

                    // Right: Amounts table
                    PdfPTable amtTbl = new PdfPTable(2);
                    amtTbl.SetWidths(new float[] { 50f, 50f });

                    // "Amounts" header
                    var amtHdr = new PdfPCell(new Phrase("Amounts", fWhiteSmallBold));
                    amtHdr.Colspan = 2;
                    amtHdr.BackgroundColor = darkBrown;
                    amtHdr.BorderColor = darkBrown;
                    amtHdr.Padding = 4f;
                    amtHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                    amtTbl.AddCell(amtHdr);

                    AddSummaryRow(amtTbl, "Received", "\u20B9 " + Bill.paidReciveamount.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);
                    AddSummaryRow(amtTbl, "Discount", "\u20B9 " + Bill.DiscountAmount.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);

                    // Total row (bold)
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
                    decimal currentBalance = Bill.ReturnNo - Bill.FinalAmount;
                    AddSummaryRow(amtTbl, "Current Balance", "\u20B9 " + currentBalance.ToString("N2"), fSmall, fRupeeSmall, borderClr, false);

                    var amtRightCell = new PdfPCell(amtTbl);
                    amtRightCell.Border = Rectangle.NO_BORDER;
                    amtRightCell.Padding = 0f;
                    amtOuter.AddCell(amtRightCell);

                    doc.Add(amtOuter);

                    // ===== Amount in words =====
                    PdfPTable wordsTable = new PdfPTable(1);
                    wordsTable.WidthPercentage = 100;
                    wordsTable.SpacingBefore = 5f;

                    var wordsHdr = new PdfPCell(new Phrase("Amount in words", fWhiteSmallBold));
                    wordsHdr.BackgroundColor = darkBrown;
                    wordsHdr.BorderColor = darkBrown;
                    wordsHdr.Padding = 4f;
                    wordsHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(wordsHdr);

                    var wordsVal = new PdfPCell(new Phrase(ConfigControls.ConvertAmountToWords(Bill.FinalAmount), fSmall));
                    wordsVal.BorderColor = borderClr;
                    wordsVal.Padding = 5f;
                    wordsVal.HorizontalAlignment = Element.ALIGN_CENTER;
                    wordsTable.AddCell(wordsVal);

                    doc.Add(wordsTable);

                    // ===== Payment mode =====
                    PdfPTable pmTable = new PdfPTable(1);
                    pmTable.WidthPercentage = 100;
                    pmTable.SpacingBefore = 5f;

                    var pmHdr = new PdfPCell(new Phrase("Payment mode", fWhiteSmallBold));
                    pmHdr.BackgroundColor = darkBrown;
                    pmHdr.BorderColor = darkBrown;
                    pmHdr.Padding = 4f;
                    pmHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                    pmTable.AddCell(pmHdr);

                    var pmVal = new PdfPCell(new Phrase(companydetail.BusinessName ?? "", fSmall));
                    pmVal.BorderColor = borderClr;
                    pmVal.Padding = 5f;
                    pmVal.HorizontalAlignment = Element.ALIGN_CENTER;
                    pmTable.AddCell(pmVal);

                    doc.Add(pmTable);

                    // ===== For: Company + Signature + Authorized Signatory =====
                    PdfPTable sigSection = new PdfPTable(1);
                    sigSection.WidthPercentage = 50;
                    sigSection.HorizontalAlignment = Element.ALIGN_RIGHT;
                    sigSection.SpacingBefore = 15f;

                    var forCell = new PdfPCell(new Phrase("For : " + (companydetail.BusinessName ?? ""), fBold));
                    forCell.Border = Rectangle.NO_BORDER;
                    forCell.Padding = 4f;
                    forCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    sigSection.AddCell(forCell);

                    // Signature image
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
                                var imgCell = new PdfPCell();
                                imgCell.Border = Rectangle.NO_BORDER;
                                imgCell.Padding = 4f;
                                imgCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                imgCell.AddElement(sigImg);
                                sigSection.AddCell(imgCell);
                            }
                            catch
                            {
                                var blankSig = new PdfPCell(new Phrase("\n\n", fSmall));
                                blankSig.Border = Rectangle.NO_BORDER;
                                blankSig.MinimumHeight = 40f;
                                sigSection.AddCell(blankSig);
                            }
                        }
                        else
                        {
                            var blankSig = new PdfPCell(new Phrase("\n\n", fSmall));
                            blankSig.Border = Rectangle.NO_BORDER;
                            blankSig.MinimumHeight = 40f;
                            sigSection.AddCell(blankSig);
                        }
                    }
                    else
                    {
                        var blankSig = new PdfPCell(new Phrase("\n\n", fSmall));
                        blankSig.Border = Rectangle.NO_BORDER;
                        blankSig.MinimumHeight = 40f;
                        sigSection.AddCell(blankSig);
                    }

                    var authCell = new PdfPCell(new Phrase("Authorized Signatory", fBold));
                    authCell.Border = Rectangle.NO_BORDER;
                    authCell.Padding = 4f;
                    authCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    sigSection.AddCell(authCell);

                    doc.Add(sigSection);

                    doc.Close();

                    byte[] bytes = stream.ToArray();

                    // Save PDF to wwwroot
                    string pdfFolderPath = Path.Combine(_env.WebRootPath, "DataContainer", "GeneratedInvoices");
                    if (!Directory.Exists(pdfFolderPath))
                        Directory.CreateDirectory(pdfFolderPath);

                    string fileName = $"PaymentIn_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
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

