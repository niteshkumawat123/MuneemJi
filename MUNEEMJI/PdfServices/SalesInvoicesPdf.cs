using Insight.Database;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MUNEEMJI.Models;
using MUNEEMJI.Models.BankAccount;
using MUNEEMJI.PdfServices.Common;
using Newtonsoft.Json;
using Npgsql;
using Npgsql.Internal;
using System.Data;
using System.Drawing.Printing;
using System.Text;
using System.Xml.Linq;

namespace MUNEEMJI.PdfServices
{
    public interface ISalesInvoicesPdf
    {
        Task<bool> GetContractPdfById(int id, IWebHostEnvironment _env);

    }

    public class SalesInvoicesPdf: ISalesInvoicesPdf
    {
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public SalesInvoicesPdf()
        {
        }

        public async Task<bool> GetContractPdfById(int id, IWebHostEnvironment _env)
        {
            await Task.Delay(1);
            var documentIds = new List<int>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string POFilePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "POFile");
            string FontPath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Font");
            string ImagePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Images");
            string QRCodePath = string.Empty;
            string POFileDuplicatePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "POFile_Duplicate");
            BusinessProfileModel companydetail = new BusinessProfileModel();
            PurchaseBill Bill = new PurchaseBill();


            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var Id = 1;

                // Get profile
                using (var cmd = new NpgsqlCommand($"SELECT bp.*,sts.name,sts.code  FROM business_profiles as bp left join states as sts on bp.state_id = sts.id WHERE businessesid = {Id}", conn))
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
            iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36, 36, 88, 65);
            try
            {
                await Task.Delay(1);

                using (var connGet = new NpgsqlConnection(_connectionString))
                {

                    Bill = await GetBillByIdForPdf(id) ?? new PurchaseBill();


                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    if (!FontFactory.IsRegistered("ARIAL"))
                    {
                        var fontPath = (Path.Combine(FontPath, "ARIAL.ttf"));
                        FontFactory.Register(fontPath, "ARIAL");
                    }

                    BaseFont bffont = BaseFont.CreateFont(Path.Combine(FontPath, "ARIAL.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    BaseFont bfRupeesfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial_with_rupee.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    BaseFont bfArialBlackfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial-black.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    Font fontozel = new Font(bffont, 7f, Font.NORMAL);

                    Font fontRupees = new Font(bfRupeesfont, 6f, Font.NORMAL);
                    Font fontRupeesBold = new Font(bfRupeesfont, 6f, Font.BOLD);
                    Font fontArialBlack = new Font(bfArialBlackfont, 6f, Font.BOLD);

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();


                    MemoryStream stream = new MemoryStream();
                    using (PdfWriter wri = PdfWriter.GetInstance(doc, stream))
                    {

                        wri.CloseStream = false;
                        wri.PageEvent = new PdfPageEvents(companydetail, _env);

                        doc.Open();
                        doc.NewPage();
                        doc.Add(new Paragraph(" ")); // Ensure at least one page has content


                        int IndexCount = 0;
                        int PositionIndex = 0;
                        bool IsBlankPrint = false;
                        bool IsRightBorder = true;

                        float[] columns = new float[50];
                        columns[0] = 5f;
                        columns[1] = 5f;
                        columns[2] = 5f;
                        columns[3] = 5f;
                        columns[4] = 5f;
                        columns[5] = 5f;
                        columns[6] = 5f;
                        columns[7] = 5f;
                        columns[8] = 5f;
                        columns[9] = 5f;
                        columns[10] = 5f;
                        columns[11] = 5f;
                        columns[12] = 5f;
                        columns[13] = 5f;
                        columns[14] = 5f;
                        columns[15] = 5f;
                        columns[16] = 5f;
                        columns[17] = 5f;
                        columns[18] = 5f;
                        columns[19] = 5f;
                        columns[20] = 5f;
                        columns[21] = 5f;
                        columns[22] = 5f;
                        columns[23] = 5f;
                        columns[24] = 5f;
                        columns[25] = 5f;
                        columns[26] = 5f;
                        columns[27] = 5f;
                        columns[28] = 5f;
                        columns[29] = 5f;
                        columns[30] = 5f;
                        columns[31] = 5f;
                        columns[32] = 5f;
                        columns[33] = 5f;
                        columns[34] = 5f;
                        columns[35] = 5f;
                        columns[36] = 5f;
                        columns[37] = 5f;
                        columns[38] = 5f;
                        columns[39] = 5f;
                        columns[40] = 5f;
                        columns[41] = 5f;
                        columns[42] = 5f;
                        columns[43] = 5f;
                        columns[44] = 5f;
                        columns[45] = 5f;
                        columns[46] = 5f;
                        columns[47] = 5f;
                        columns[48] = 5f;
                        columns[49] = 5f;


                        Font normalFont = new Font(bffont, 8.2f, Font.NORMAL);
                        Font boldFont = new Font(bffont, 8.2f, Font.BOLD);

                        var phrase = new Phrase();

                        var phraseIRN = new Phrase();

                        var RowsCount = 1;





                        if (!string.IsNullOrEmpty(Bill.EWayBillNo))
                        {
                            float[] Einvoice = new float[50];
                            Einvoice[0] = 5f;
                            Einvoice[1] = 5f;
                            Einvoice[2] = 5f;
                            Einvoice[3] = 5f;
                            Einvoice[4] = 5f;
                            Einvoice[5] = 5f;
                            Einvoice[6] = 5f;
                            Einvoice[7] = 5f;
                            Einvoice[8] = 5f;
                            Einvoice[9] = 5f;
                            Einvoice[10] = 5f;
                            Einvoice[11] = 5f;
                            Einvoice[12] = 5f;
                            Einvoice[13] = 5f;
                            Einvoice[14] = 5f;
                            Einvoice[15] = 5f;
                            Einvoice[16] = 5f;
                            Einvoice[17] = 5f;
                            Einvoice[18] = 5f;
                            Einvoice[19] = 5f;
                            Einvoice[20] = 5f;
                            Einvoice[21] = 5f;
                            Einvoice[22] = 5f;
                            Einvoice[23] = 5f;
                            Einvoice[24] = 5f;
                            Einvoice[25] = 5f;
                            Einvoice[26] = 5f;
                            Einvoice[27] = 5f;
                            Einvoice[28] = 5f;
                            Einvoice[29] = 5f;
                            Einvoice[30] = 5f;
                            Einvoice[31] = 5f;
                            Einvoice[32] = 5f;
                            Einvoice[33] = 5f;
                            Einvoice[34] = 5f;
                            Einvoice[35] = 5f;
                            Einvoice[36] = 5f;
                            Einvoice[37] = 5f;
                            Einvoice[38] = 5f;
                            Einvoice[39] = 5f;
                            Einvoice[40] = 5f;
                            Einvoice[41] = 5f;
                            Einvoice[42] = 5f;
                            Einvoice[43] = 5f;
                            Einvoice[44] = 5f;
                            Einvoice[45] = 5f;
                            Einvoice[46] = 5f;
                            Einvoice[47] = 5f;
                            Einvoice[48] = 5f;
                            Einvoice[49] = 5f;

                            PdfPTable EnvoiceTable = new PdfPTable(Einvoice);
                            EnvoiceTable.TotalWidth = 520f;
                            EnvoiceTable.LockedWidth = true;



                            PdfPCell EnvoiceCell = ConfigControls.GetPdfTableCell("E-Way Bill DETAILS", 0, 2, 35, 1, 8f, Font.BOLD, 3f, 5f, false, false, false, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 15, 1, 8f, Font.BOLD, 3.5f, 3.5f, false, false, false, false, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("E-Way Bill No", 0, 0, 5, 1, 8f, Font.BOLD, 3.5f, 5f, true, true, true, true, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 27f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            //EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 12, 1, 8f, Font.BOLD, 3.5f, 3.5f, false, true, false, false, BaseColor.BLACK);
                            //EnvoiceCell.PaddingLeft = 5f;
                            //EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            //EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell(Bill.EWayBillNo, 0, 0, 30, 1, 8f, Font.NORMAL, 3.5f, 5f, true, true, true, true, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 27f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 15, 1, 8f, Font.BOLD, 3.5f, 3.5f, false, false, false, false, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 27f;
                            EnvoiceTable.AddCell(EnvoiceCell);

                            //EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 12, 1, 8f, Font.BOLD, 3.5f, 3.5f, true, true, false, false, BaseColor.BLACK);
                            //EnvoiceCell.PaddingLeft = 5f;
                            //EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            //EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("ACK.NO", 0, 0, 5, 1, 8f, Font.BOLD, 3.5f, 3.5f, true, true, false, false, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 25f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 30, 1, 8f, Font.NORMAL, 3.5f, 3.5f, true, true, true, true, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 25f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 15, 1, 8f, Font.BOLD, 3.5f, 3.5f, false, false, false, false, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 25f;
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 5, 1, 8f, Font.BOLD, 3.5f, 3.5f, true, false, true, true, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 25f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 30, 1, 8f, Font.NORMAL, 3.5f, 3.5f, true, true, true, true, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 25f;
                            EnvoiceCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            EnvoiceTable.AddCell(EnvoiceCell);

                            EnvoiceCell = ConfigControls.GetPdfTableCell("", 0, 0, 15, 1, 8f, Font.BOLD, 3.5f, 3.5f, false, false, false, false, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.MinimumHeight = 25f;
                            EnvoiceTable.AddCell(EnvoiceCell);


                            EnvoiceCell = ConfigControls.GetPdfTableCell(" ", 0, 0, 50, 1, 11f, Font.NORMAL, 1f, 1f, false, false, false, false, BaseColor.BLACK);
                            EnvoiceCell.PaddingLeft = 5f;
                            EnvoiceCell.PaddingBottom = -2f;
                            EnvoiceTable.AddCell(EnvoiceCell);

                            string base64String = ""; // Replace with your Base64 QR code

                            if (!string.IsNullOrEmpty(base64String))
                            {
                                // Convert Base64 string to byte array
                                byte[] qrCodeBytes = Convert.FromBase64String(base64String);

                                // Convert bytes to iTextSharp image
                                iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(qrCodeBytes);

                                // Set image size (Optional)
                                qrImage.ScaleAbsolute(185f, 185f); // Adjust size as needed
                                float paddingTop = 2f;
                                float xPosition = doc.PageSize.Width - qrImage.ScaledWidth;
                                float yPosition = doc.PageSize.Height - qrImage.ScaledHeight;
                                qrImage.SetAbsolutePosition(xPosition, yPosition);
                                PdfContentByte cb = wri.DirectContent;
                                // Add image to the document
                                doc.Add(qrImage);
                            }

                            doc.Add(EnvoiceTable);

                        }



                        string InvoiceType = string.Empty;
                        string InvoiceTypeDate = string.Empty;

                        InvoiceType = "Proforma Invoice No.";
                        InvoiceTypeDate = "Proforma Invoice Date.";




                        string InvoiceNo = string.Empty;
                        InvoiceNo = Bill.InvoiceNumber?.ToString();



                        var BillingCycle = "";


                        var CategoryName = "B2C";
                        var fields = new List<(string title, string value, int index)>
                                {
                                    (InvoiceTypeDate, Bill.InvoiceDate.HasValue && Bill.InvoiceDate.Value != DateTime.MinValue ? Bill.InvoiceDate.Value.ToString("dd-MMM-yyyy") : "", 1),
                                    (InvoiceType, InvoiceNo, 2),

                                    ("PO. No.", Bill.PONo!=null ? Bill.PONo:"", 3),
                                    ("PO Date", Bill.PODate.HasValue && Bill.PODate.Value != DateTime.MinValue ? Bill.PODate.Value.ToString("dd-MMM-yyyy") : "", 4),
                                    ("WCC No.", "", 5),
                                    ("WCC Date",  "", 6),
                                    ("Receipt No.", "", 7),
                                    ("Receipt Date",  "", 8),
                                    ("Site Name", "", 9),
                                    ("Site Id", "", 10),
                                    ("Reference No.", "", 11),
                                    ("RFAI Date",  "", 12),
                                    ("Billing Cycle", "", 13),
                                    ("Engine No.", "", 14),
                                    ("Claim No.", "", 15),
                                    ("Complain No.", "", 16),
                                    ("Invoice Type", "", 17),
                                    ("PO. Type/Subtype", "", 18),
                                    ("Category", CategoryName , 19),
                                    ("Item Code", "", 20)
                                };



                        float[] columns1 = new float[50];
                        columns1[0] = 5f;
                        columns1[1] = 5f;
                        columns1[2] = 5f;
                        columns1[3] = 5f;
                        columns1[4] = 5f;
                        columns1[5] = 5f;
                        columns1[6] = 5f;
                        columns1[7] = 5f;
                        columns1[8] = 5f;
                        columns1[9] = 5f;
                        columns1[10] = 5f;
                        columns1[11] = 5f;
                        columns1[12] = 5f;
                        columns1[13] = 5f;
                        columns1[14] = 5f;
                        columns1[15] = 5f;
                        columns1[16] = 5f;
                        columns1[17] = 5f;
                        columns1[18] = 5f;
                        columns1[19] = 5f;
                        columns1[20] = 5f;
                        columns1[21] = 5f;
                        columns1[22] = 5f;
                        columns1[23] = 5f;
                        columns1[24] = 5f;
                        columns1[25] = 5f;
                        columns1[26] = 5f;
                        columns1[27] = 5f;
                        columns1[28] = 5f;
                        columns1[29] = 5f;
                        columns1[30] = 5f;
                        columns1[31] = 5f;
                        columns1[32] = 5f;
                        columns1[33] = 5f;
                        columns1[34] = 5f;
                        columns1[35] = 5f;
                        columns1[36] = 5f;
                        columns1[37] = 5f;
                        columns1[38] = 5f;
                        columns1[39] = 5f;
                        columns1[40] = 5f;
                        columns1[41] = 5f;
                        columns1[42] = 5f;
                        columns1[43] = 5f;
                        columns1[44] = 5f;
                        columns1[45] = 5f;
                        columns1[46] = 5f;
                        columns1[47] = 5f;
                        columns1[48] = 5f;
                        columns1[49] = 5f;
                        PdfPTable SupplierTable = new PdfPTable(columns1);

                        SupplierTable.WidthPercentage = 100;


                        PdfPCell tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 50, 1, 8f, Font.BOLD, 3f, 5f, false, false, false, false, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.PaddingBottom = 20f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        //Row -1
                        tempCell = ConfigControls.GetPdfTableCell("SUPPLIER", 0, 0, 24, 1, 8f, Font.BOLD, 3f, 5f, false, false, false, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);




                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, true, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);



                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, true, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);




                        tempCell = ConfigControls.GetPdfTableCell(companydetail.BusinessName != null ? companydetail.BusinessName : "", 0, 0, 24, 1, 8f, Font.BOLD, 3f, 3f, true, true, true, false, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        //tempCell.PaddingBottom = 3f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 3f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 3f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        tempCell = ConfigControls.GetPdfTableCell(companydetail.Address != null ? companydetail.Address : string.Empty, 0, 0, 24, 1, 8f, Font.NORMAL, 3f, 3f, true, true, false, false, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;

                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 3f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        tempCell.PaddingLeft = 5f;
                        SupplierTable.AddCell(tempCell);


                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 3f, false, true, false, IsRightBorder, BaseColor.BLACK); ; ;
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("GSTIN: ", boldFont));
                        phrase.Add(new Chunk(!string.IsNullOrEmpty(companydetail.Gstin) ? companydetail.Gstin : "N/A", normalFont));
                        tempCell = new PdfPCell(phrase);
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = 3f;
                        tempCell.PaddingBottom = 3f;
                        tempCell.BorderWidthBottom = 0;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        tempCell.PaddingLeft = 5f;
                        SupplierTable.AddCell(tempCell);

                        //var rfidate = contract.RFAIDate != null ? contract.RFAIDate.Value.ToString("dd-MMM-yyyy") : "NA";
                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        phrase = new Phrase();
                        phrase.Add(new Chunk("VENDOR CODE: ", boldFont));
                        phrase.Add(new Chunk("NA", normalFont));
                        tempCell = new PdfPCell(phrase);
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = 3f;
                        tempCell.PaddingBottom = 3f;
                        tempCell.BorderWidthBottom = 0;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        var Code = companydetail.statecode;

                        var statename = string.Empty;
                        statename = companydetail.statename;

                        phrase = new Phrase();
                        phrase.Add(new Chunk("STATE CODE: ", boldFont));
                        phrase.Add(new Chunk(Code, normalFont));
                        phrase.Add(new Chunk($" ({(statename ?? string.Empty).ToUpper()})", normalFont));
                        tempCell = new PdfPCell(phrase);
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = 3f;
                        tempCell.PaddingBottom = 3f;
                        tempCell.BorderWidthBottom = 0;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }



                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell("CUSTOMER", 0, 0, 24, 1, 8f, Font.BOLD, 3f, 5f, false, false, false, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);



                        //tempCell = ConfigControls.GetPdfTableCell("Destination Country - " + "India", 0, 0, 12, 1, 8f, Font.BOLD, 3f, 5f, false, false, false, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                        //tempCell.PaddingLeft = 4f;
                        //tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                        //tempCell.PaddingRight = 6f;
                        //SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        tempCell = ConfigControls.GetPdfTableCell("BILLING ADDRESS", 0, 0, 24, 1, 8f, Font.BOLD, 3f, 5f, true, true, false, false, new iTextSharp.text.BaseColor(32, 117, 233));
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);
                        //Row--4
                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }


                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(Bill.PartyName != null ? Bill.PartyName : string.Empty/* ?? default(string)*/, 0, 0, 24, 1, 8f, Font.BOLD, 3f, 3f, true, true, false, false, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        tempCell.PaddingLeft = 5f;
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(Bill.BillingAddress != null ? Bill.BillingAddress : string.Empty, 0, 0, 24, 2, 8f, Font.NORMAL, 3f, 3f, true, true, false, false, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);

                        // Important changes:
                        tempCell.NoWrap = false; // Allow wrapping if text is long
                        tempCell.MinimumHeight = 20f; // Optional: Set a sensible minimum height
                        tempCell.VerticalAlignment = Element.ALIGN_MIDDLE; // Optional: Center text vertically

                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        tempCell.PaddingLeft = 5f;
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        tempCell.PaddingLeft = 5f;
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        var partydetail = PartDetailForPdfById(Bill.PartyId);

                        var CoustomerStateCode = partydetail != null && partydetail.StateCode != null ? partydetail.StateCode : "NA";
                        var CoustomerStateName = partydetail != null && partydetail.StateName != null ? partydetail.StateName : "NA";

                        phrase = new Phrase();
                        phrase.Add(new Chunk("STATE CODE: ", boldFont));
                        phrase.Add(new Chunk(CoustomerStateCode, normalFont));
                        phrase.Add(new Chunk($" ({CoustomerStateName.ToUpper()})", normalFont));
                        tempCell = new PdfPCell(phrase);
                        // Important changes:
                        tempCell.NoWrap = false; // Allow wrapping if text is long
                        tempCell.MinimumHeight = 20f; // Optional: Set a sensible minimum height
                        tempCell.VerticalAlignment = Element.ALIGN_MIDDLE; // Optional: Center text vertically
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = -10f;
                        tempCell.PaddingBottom = 3f;
                        tempCell.BorderWidthBottom = 0;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);



                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        phrase = new Phrase();
                        phrase.Add(new Chunk("GSTIN: ", boldFont));
                        phrase.Add(new Chunk(partydetail != null && !string.IsNullOrEmpty(partydetail.GSTIN) ? partydetail.GSTIN : "N/A", normalFont));
                        tempCell = new PdfPCell(phrase);
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = -10f;
                        tempCell.PaddingBottom = 5f;
                        tempCell.BorderWidthBottom = 0;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);
                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell("SHIPPING ADDRESS", 0, 0, 24, 1, 8f, Font.BOLD, 3f, 5f, true, true, false, false, new iTextSharp.text.BaseColor(25, 135, 84));
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);
                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(!string.IsNullOrEmpty(Bill.ShippingAddress) ? Bill.ShippingAddress : (partydetail != null && partydetail.ShippingAddress != null) ?
                            partydetail.ShippingAddress : string.Empty, 0, 0, 24, 2, 8f, Font.NORMAL, 1f, 1f, true, true, false, false, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.PaddingTop = 0f;
                        tempCell.PaddingBottom = 0f;
                        tempCell.NoWrap = false; // Allow wrapping if text is long
                        tempCell.MinimumHeight = 20f; // Optional: Set a sensible minimum height
                        tempCell.VerticalAlignment = Element.ALIGN_MIDDLE; // Optional: Center text vertically
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);



                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;

                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);
                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);
                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        var DeliveryStateCode = partydetail != null && partydetail.StateCode != null ? partydetail.StateCode : "NA";

                        var DeliveryStateName = partydetail != null && partydetail.StateName != null ? partydetail.StateName : "NA";

                        phrase = new Phrase();
                        phrase.Add(new Chunk("STATE CODE: ", boldFont));
                        phrase.Add(new Chunk(DeliveryStateCode, normalFont));
                        phrase.Add(new Chunk(" (" + DeliveryStateName.ToUpper() + ")", normalFont));
                        tempCell = new PdfPCell(phrase);
                        tempCell.NoWrap = false; // Allow wrapping if text is long
                        tempCell.MinimumHeight = 20f; // Optional: Set a sensible minimum height
                        tempCell.VerticalAlignment = Element.ALIGN_MIDDLE; // Optional: Center text vertically
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = -10f;
                        tempCell.PaddingBottom = 3f;
                        tempCell.BorderWidthBottom = 0;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        for (int i = 0; i < fields.Count(); i++)
                        {
                            PositionIndex = IndexCount;
                            if (fields.Count() > IndexCount && !string.IsNullOrEmpty(fields[IndexCount].value))
                            {
                                PositionIndex = IndexCount;
                                IndexCount = IndexCount + 1;
                                break;
                            }
                            else if (fields.Count() <= IndexCount)
                            {
                                IsRightBorder = false;
                                IsBlankPrint = true;
                            }
                            else
                            {
                                IndexCount = IndexCount + 1;
                            }
                        }

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].title : " ", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, IsRightBorder, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        tempCell.PaddingLeft = 5f;
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(IsBlankPrint == false ? fields[PositionIndex].value : " ", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, IsRightBorder, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("GSTIN: ", boldFont));
                        phrase.Add(new Chunk(partydetail != null && !string.IsNullOrEmpty(partydetail.GSTIN) ? partydetail.GSTIN : "N/A", normalFont));
                        tempCell = new PdfPCell(phrase);
                        tempCell.PaddingLeft = 5f;
                        tempCell.Colspan = 24;
                        tempCell.Rowspan = 1;
                        tempCell.PaddingTop = -10f;
                        tempCell.PaddingBottom = 5f;
                        tempCell.BorderWidthBottom = 0.5f;
                        tempCell.BorderWidthLeft = 0.5f;
                        tempCell.BorderWidthRight = 0.5f;
                        tempCell.BorderWidthTop = 0;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 10, 1, 8f, Font.NORMAL, 3f, 5f, false, false, false, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 16, 1, 8f, Font.BOLD, 3f, 5f, false, true, false, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                        SupplierTable.AddCell(tempCell);


                        //tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 50, 1, 11f, Font.NORMAL, 1f, 1f, false, false, false, false, BaseColor.BLACK);
                        //tempCell.MinimumHeight = 10f;
                        //tempCell.PaddingBottom = 10f;
                        //SupplierTable.AddCell(tempCell);



                        doc.Add(SupplierTable);

                        #region Particulares



                        int pageCount = 1;
                        int NoMoreDeliverableContent = 0;

                        float[] columns2 = new float[50];
                        columns2[0] = 5f;
                        columns2[1] = 5f;
                        columns2[2] = 5f;
                        columns2[3] = 5f;
                        columns2[4] = 5f;
                        columns2[5] = 5f;
                        columns2[6] = 5f;
                        columns2[7] = 5f;
                        columns2[8] = 5f;
                        columns2[9] = 5f;
                        columns2[10] = 5f;
                        columns2[11] = 5f;
                        columns2[12] = 5f;
                        columns2[13] = 5f;
                        columns2[14] = 5f;
                        columns2[15] = 5f;
                        columns2[16] = 5f;
                        columns2[17] = 5f;
                        columns2[18] = 5f;
                        columns2[19] = 5f;
                        columns2[20] = 5f;
                        columns2[21] = 5f;
                        columns2[22] = 5f;
                        columns2[23] = 5f;
                        columns2[24] = 5f;
                        columns2[25] = 5f;
                        columns2[26] = 5f;
                        columns2[27] = 5f;
                        columns2[28] = 5f;
                        columns2[29] = 5f;
                        columns2[30] = 5f;
                        columns2[31] = 5f;
                        columns2[32] = 5f;
                        columns2[33] = 5f;
                        columns2[34] = 5f;
                        columns2[35] = 5f;
                        columns2[36] = 5f;
                        columns2[37] = 5f;
                        columns2[38] = 5f;
                        columns2[39] = 5f;
                        columns2[40] = 5f;
                        columns2[41] = 5f;
                        columns2[42] = 5f;
                        columns2[43] = 5f;
                        columns2[44] = 5f;
                        columns2[45] = 5f;
                        columns2[46] = 5f;
                        columns2[47] = 5f;
                        columns2[48] = 5f;
                        columns2[49] = 5f;



                        PdfPTable Particulares = new PdfPTable(columns2);
                        //Particulares.TotalWidth = 520f;
                        Particulares.WidthPercentage = 100;
                        Particulares.LockedWidth = false;
                        var itemindex = "";
                        #region Service

                        if (Bill != null && Bill.BillItems != null && Bill.BillItems.Count() > 0 && Bill.BillItems.Where(x => x.PricePerUnit > 0).Count() >= 1)
                        {
                            var round = 1;
                            bool IsDiscount = Bill.BillItems.Any(x => x.DiscountAmount > 0);
                            int ColspanNo = 0;
                            if (IsDiscount == false)
                            {
                                ColspanNo = 5;
                            }
                            var HeaderTitleOld = string.Empty;

                            foreach (var itemservice in Bill.BillItems)
                            {

                                tempCell.BorderColor = BaseColor.BLACK;

                                if (itemservice.Item != null && itemservice.Item != "")
                                {

                                    if (round == 1)
                                    {
                                        tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 50, 1, 11f, Font.NORMAL, 1f, 1f, false, false, false, false, BaseColor.BLACK);
                                        tempCell.MinimumHeight = 10f;
                                        tempCell.PaddingBottom = 10f;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("#", 0, 0, 2, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);

                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("ITEM CODE", 0, 0, 5, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingLeft = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("PARTICULAR", 0, 0, 10 + ColspanNo, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingLeft = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("HSN/SAC", 0, 0, 4, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingLeft = 5f;
                                        tempCell.PaddingRight = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("UOM", 0, 0, 5, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingLeft = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("QTY", 0, 0, 5, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingLeft = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                        Particulares.AddCell(tempCell);

                                        tempCell = ConfigControls.GetPdfTableCell("RATE", 0, 0, 6, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingRight = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                        Particulares.AddCell(tempCell);

                                        if (IsDiscount == true)
                                        {
                                            tempCell = ConfigControls.GetPdfTableCell("DISCOUNT", 0, 0, 5, 1, 7f, Font.BOLD, 3f, 4f, true, false, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                            tempCell.PaddingRight = 5f;
                                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                            Particulares.AddCell(tempCell);
                                        }

                                        tempCell = ConfigControls.GetPdfTableCell("AMOUNT", 0, 0, 8, 1, 7f, Font.BOLD, 3f, 4f, true, true, true, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK);
                                        tempCell.PaddingRight = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                        Particulares.AddCell(tempCell);

                                        doc.Add(Particulares);

                                        Particulares = new PdfPTable(columns2);
                                        Particulares.WidthPercentage = 100;

                                    }
                                    var Topborder = true;
                                    var Bottomborder = true;
                                    if (round == 1)
                                    {
                                        Topborder = true;
                                        Bottomborder = true;
                                    }
                                    else
                                    {
                                        Topborder = true;
                                        Bottomborder = true;
                                    }

                                    //var HeaderTitleNew = itemservice.HeaderTitle;

                                    //if (!string.IsNullOrEmpty(itemservice.HeaderTitle) && !string.IsNullOrEmpty(HeaderTitleNew) && !HeaderTitleNew.Equals(HeaderTitleOld))
                                    //{
                                    //    tempCell = ConfigControls.GetPdfTableCell(itemindex + itemservice.HeaderTitle.ToUpper(), 0, 0, 50, 1, 7f, Font.BOLD, 3f, 4f, true, true, false, false, new iTextSharp.text.BaseColor(240, 240, 240), BaseColor.BLACK);
                                    //    tempCell.PaddingLeft = 5f;
                                    //    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                    //    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    //    Particulares.AddCell(tempCell);

                                    //    HeaderTitleOld = itemservice.HeaderTitle;
                                    //}


                                    tempCell = ConfigControls.GetPdfTableCell(round.ToString(), 0, 0, 2, 1, 8f, Font.BOLD, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    Particulares.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell(itemservice.ItemCode, 0, 0, 5, 1, 8f, Font.NORMAL, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.PaddingLeft = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);

                                    Font descriptionnormal = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7f, Font.NORMAL);
                                    Font descriptionboldFont = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, Font.NORMAL);

                                    phrase = new Phrase();
                                    phrase.Add(new Chunk(!string.IsNullOrEmpty(itemservice.Item) ? itemservice.Item.ToUpper() : "", descriptionboldFont));
                                    if (!string.IsNullOrEmpty(itemservice.Item))
                                    {
                                        phrase.Add(new Chunk("\n\n" + itemservice.Item.ToUpper() + "\n\n", descriptionnormal));
                                    }
                                    tempCell = new PdfPCell(phrase);
                                    tempCell.PaddingLeft = 5f;
                                    tempCell.Colspan = 10 + ColspanNo;
                                    tempCell.Rowspan = 1;
                                    tempCell.PaddingTop = 3f;
                                    tempCell.PaddingBottom = 3f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthLeft = 0.5f;
                                    tempCell.BorderWidthRight = 0f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);


                                    //tempCell = ConfigControls.GetPdfTableCell(itemservice.ItemServiceValue, 0, 0, 10 + ColspanNo, 1, 7f, Font.NORMAL, 5f, 10f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    //tempCell.PaddingLeft = 5f;
                                    //tempCell.BorderWidthBottom = 0.25f;
                                    //tempCell.BorderWidthTop = 0.25f;
                                    //tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                    //Particulares.AddCell(tempCell);


                                    tempCell = ConfigControls.GetPdfTableCell(itemservice.HSNCode, 1, 0, 4, 1, 8f, Font.NORMAL, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.PaddingLeft = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell(itemservice.Unit, 1, 0, 5, 1, 8f, Font.NORMAL, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                    tempCell.PaddingLeft = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell(Convert.ToString(itemservice.Quantity), 1, 0, 5, 1, 8f, Font.NORMAL, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                    tempCell.PaddingLeft = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell(itemservice.PricePerUnit.ToString("0.00"), 1, 0, 6, 1, 8f, Font.NORMAL, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);

                                    if (IsDiscount == true)
                                    {
                                        tempCell = ConfigControls.GetPdfTableCell(itemservice.DiscountAmount.ToString("0.00"), 1, 0, 5, 1, 8f, Font.NORMAL, 5f, 4f, true, false, Topborder, Bottomborder, BaseColor.BLACK);
                                        tempCell.PaddingRight = 5f;
                                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                        tempCell.BorderWidthBottom = 0.25f;
                                        tempCell.BorderWidthTop = 0.25f;
                                        tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                        Particulares.AddCell(tempCell);
                                    }

                                    var itemamount = itemservice.PricePerUnit * itemservice.Quantity - itemservice.DiscountAmount;
                                    tempCell = ConfigControls.GetPdfTableCell(itemamount != null ? Math.Round(itemamount, 2).ToString("0.00") : "0.00", 1, 0, 8, 1, 8f, Font.NORMAL, 5f, 4f, true, true, Topborder, Bottomborder, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                                    Particulares.AddCell(tempCell);

                                    round = round + 1;
                                    RowsCount++;
                                    itemindex = "";

                                    doc.Add(Particulares);
                                    Particulares = new PdfPTable(columns2);
                                    Particulares.WidthPercentage = 100;
                                }

                            }

                        }





                        #region Taxation section

                        bool isDomestic = true;



                        columns2 = new float[7];
                        columns2[0] = 130f;
                        columns2[1] = 100f;
                        columns2[2] = 50f; //44
                        columns2[3] = 50f; //35
                        columns2[4] = 50f; //54
                        columns2[5] = 50f; //64
                        columns2[6] = 90f; //63//63



                        PdfPTable Discount = new PdfPTable(columns2);
                        Discount.WidthPercentage = 100;
                        Discount.KeepTogether = true;
                        //PdfPTable Taxable = new PdfPTable(columns2);
                        //Taxable.WidthPercentage = 100;

                        PdfPTable TaxHeadingTable = new PdfPTable(columns2);  // service heading table for fixed for tax heading can not move to next page 
                        TaxHeadingTable.WidthPercentage = 100;
                        TaxHeadingTable.KeepTogether = true;

                        PdfPTable TaxValueTable = new PdfPTable(columns2);  // service heading table for fixed for tax heading can not move to next page 
                        TaxValueTable.WidthPercentage = 100;

                       
                        int service = 0;

                        if (partydetail != null && Bill.StateId == partydetail.StateId)
                        {
                            isDomestic = true;
                        }
                        else
                        {
                            isDomestic = false;
                        }






                        tempCell = ConfigControls.GetPdfTableCell("TAXABLE VALUE GOODS", 0, 0, 2, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderWidthBottom = 0.25f;
                        tempCell.BorderWidthTop = 0.25f;
                        TaxHeadingTable.AddCell(tempCell);


                        if (isDomestic)
                        {
                            tempCell = ConfigControls.GetPdfTableCell(" CGST ", 0, 0, 2, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell(" SGST ", 0, 0, 2, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);
                        }
                        else
                        {
                            tempCell = ConfigControls.GetPdfTableCell(" IGST ", 0, 0, 4, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);
                        }

                        tempCell = ConfigControls.GetPdfTableCell(" ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderWidthBottom = 0.25f;
                        tempCell.BorderWidthTop = 0.25f;
                        TaxHeadingTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(" ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, false, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderWidthBottom = 0.25f;
                        tempCell.BorderWidthTop = 0.25f;
                        TaxHeadingTable.AddCell(tempCell);

                        tempCell = ConfigControls.GetPdfTableCell(" ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, false, false, false, true, BaseColor.BLACK);
                        tempCell.PaddingLeft = 5f;
                        tempCell.BorderWidthBottom = 0.25f;
                        tempCell.BorderWidthTop = 0.25f;
                        TaxHeadingTable.AddCell(tempCell);
                        if (isDomestic)
                        {
                            tempCell = ConfigControls.GetPdfTableCell("RATE(%) ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("AMOUNT ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("RATE(%)", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("AMOUNT ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);
                        }
                        else
                        {
                            tempCell = ConfigControls.GetPdfTableCell("RATE(%) ", 0, 0, 2, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("AMOUNT ", 0, 0, 2, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                            tempCell.BorderWidthBottom = 0.25f;
                            tempCell.BorderWidthTop = 0.25f;
                            TaxHeadingTable.AddCell(tempCell);
                        }
                        tempCell = ConfigControls.GetPdfTableCell("SUB TOTAL", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, true, false, true, BaseColor.BLACK);
                        tempCell.PaddingRight = 5f;
                        tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                        tempCell.BorderWidthTop = 0.25f;
                        TaxHeadingTable.AddCell(tempCell);

                        doc.Add(TaxHeadingTable);
                        TaxHeadingTable = new PdfPTable(columns2);
                        TaxHeadingTable.WidthPercentage = 100;

                        foreach (var servicedata in Bill.BillItems)
                        {

                            if (servicedata.Tax != null && servicedata.TaxAmount > 0)
                            {
                                if (isDomestic)
                                {

                                    tempCell = ConfigControls.GetPdfTableCell("CGST" + "%", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    TaxValueTable.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell("CGSTValue", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    TaxValueTable.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell("SGST" + "%", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    TaxValueTable.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell("SGSTValue", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    TaxValueTable.AddCell(tempCell);


                                    //var itemamount = (servicedata.Subtotal - servicedata.DiscountValue) + (contract.IsRCM == true ? 0 : servicedata.Taxes.Select(x => x.CGSTValue + x.SGSTValue + x.IGSTValue).Sum() ?? 0);
                                    tempCell = ConfigControls.GetPdfTableCell(Math.Round(2.3).ToString("0.00"), 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                                    tempCell.PaddingBottom = 5f;
                                    tempCell.PaddingRight = 5f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    TaxValueTable.AddCell(tempCell);






                                    doc.Add(TaxValueTable);
                                    TaxValueTable = new PdfPTable(columns2);
                                    TaxValueTable.WidthPercentage = 100;

                                }
                                else
                                {

                                    tempCell = ConfigControls.GetPdfTableCell("IGST" + "%", 0, 0, 2, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                    tempCell.PaddingRight = 5f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    TaxValueTable.AddCell(tempCell);

                                    tempCell = ConfigControls.GetPdfTableCell("IGSTValue", 0, 0, 2, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.PaddingRight = 5f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    TaxValueTable.AddCell(tempCell);


                                    tempCell = ConfigControls.GetPdfTableCell("1", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                                    tempCell.PaddingBottom = 5f;
                                    tempCell.PaddingRight = 5f;
                                    tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                    tempCell.BorderWidthBottom = 0.25f;
                                    tempCell.BorderWidthTop = 0.25f;
                                    tempCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    TaxValueTable.AddCell(tempCell);





                                    doc.Add(TaxValueTable);
                                    TaxValueTable = new PdfPTable(columns2);
                                    TaxValueTable.WidthPercentage = 100;
                                }

                                tempCell = ConfigControls.GetPdfTableCell("1", 0, 0, 2, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                tempCell.PaddingLeft = 5f;
                                tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("NIL", 0, 0, 2, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("NIL", 0, 0, 2, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("1", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                                tempCell.PaddingBottom = 5f;
                                tempCell.PaddingRight = 5f;
                                tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                tempCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                                TaxValueTable.AddCell(tempCell);

                                doc.Add(TaxValueTable);
                                TaxValueTable = new PdfPTable(columns2);
                                TaxValueTable.WidthPercentage = 100;





                            }
                            else
                            {
                                tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 2, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("NIL", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("NIL", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("NIL", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("NIL", 0, 0, 1, 1, 8f, Font.NORMAL, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                                tempCell.PaddingLeft = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                TaxValueTable.AddCell(tempCell);

                                tempCell = ConfigControls.GetPdfTableCell("1", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                                tempCell.PaddingBottom = 5f;
                                tempCell.PaddingRight = 5f;
                                tempCell.BorderWidthBottom = 0.25f;
                                tempCell.BorderWidthTop = 0.25f;
                                tempCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tempCell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
                                TaxValueTable.AddCell(tempCell);

                                doc.Add(TaxValueTable);
                                TaxValueTable = new PdfPTable(columns2);
                                TaxValueTable.WidthPercentage = 100;



                            }
                            #endregion

                        } // <--- End of foreach (var servicedata in Bill.BillItems) loop

                        PdfPTable AdditionalCharge = new PdfPTable(columns2);
                            AdditionalCharge.WidthPercentage = 100;



                            tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 7, 1, 11f, Font.NORMAL, 1f, 1f, false, false, false, false, BaseColor.BLACK);
                            tempCell.MinimumHeight = 10f;
                            tempCell.PaddingBottom = 10f;
                            AdditionalCharge.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("TOTAL PAYABLE AMOUNT ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            tempCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            AdditionalCharge.AddCell(tempCell);



                            tempCell = ConfigControls.GetPdfTableCell("INR" + " " + "1", 0, 0, 6, 1, 10f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tempCell.BorderColor = new iTextSharp.text.BaseColor(169, 169, 169);
                            AdditionalCharge.AddCell(tempCell);

                            doc.Add(AdditionalCharge);
                            AdditionalCharge = new PdfPTable(columns2);
                            AdditionalCharge.WidthPercentage = 100;


                            tempCell = ConfigControls.GetPdfTableCell("TOTAL AMOUNT IN WORDS ", 0, 0, 1, 1, 8f, Font.BOLD, 3f, 4f, true, false, true, true, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            tempCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            AdditionalCharge.AddCell(tempCell);


                            tempCell = ConfigControls.GetPdfTableCell("INR" + " ", 0, 0, 6, 1, 8f, Font.BOLD, 3f, 4f, true, true, true, true, BaseColor.BLACK);
                            tempCell.PaddingRight = 5f;
                            tempCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            AdditionalCharge.AddCell(tempCell);

                            doc.Add(AdditionalCharge);
                            AdditionalCharge = new PdfPTable(columns2);
                            AdditionalCharge.WidthPercentage = 100;




                            #endregion

                            var BankDetail = GetBankForPdf();





                            #region Bank Details
                            float[] columns3 = new float[1];
                            columns3 = new float[3];
                            columns3[0] = 225f;
                            columns3[1] = 210f;
                            columns3[2] = 85f;


                            PdfPTable BankDetails = new PdfPTable(columns3);
                            BankDetails.WidthPercentage = 100;
                            BankDetails.KeepTogether = true;
                            //BankDetails.LockedWidth = true;

                            tempCell = ConfigControls.GetPdfTableCell("", 0, 0, 7, 1, 11f, Font.NORMAL, 1f, 1f, false, false, false, false, BaseColor.BLACK);
                            tempCell.MinimumHeight = 10f;
                            tempCell.PaddingBottom = 10f;
                            BankDetails.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("BANK DETAILS ", 0, 0, 1, 1, 9f, Font.BOLD, 3f, 4f, true, false, true, false, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            BankDetails.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("  ", 0, 0, 1, 1, 7f, Font.BOLD, 3f, 4f, false, false, true, false, BaseColor.BLACK);
                            tempCell.PaddingLeft = 5f;
                            BankDetails.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell("  ", 0, 0, 1, 1, 7f, Font.BOLD, 3f, 4f, false, true, true, false, BaseColor.BLACK);//Pay with QR Code
                            tempCell.PaddingLeft = 5f;
                            tempCell.PaddingTop = 5f;
                            BankDetails.AddCell(tempCell);

                           
                          

                            var Phrase = new Phrase();
                            Phrase.Add(new Chunk("BENEFICIARY ACCOUNT NAME: ", boldFont));
                            Phrase.Add(new Chunk(BankDetail != null && BankDetail.AccountDisplayName != null ? BankDetail.AccountDisplayName.ToUpper() : "N/A", normalFont));
                            tempCell = new PdfPCell(Phrase);
                            tempCell.PaddingLeft = 4f;
                            tempCell.Colspan = 2;
                            tempCell.Rowspan = 1;
                            tempCell.PaddingTop = 3f;
                            tempCell.PaddingBottom = 4f;
                            tempCell.Border = 0;
                            tempCell.BorderWidthLeft = 0.5f;
                            tempCell.BorderWidthRight = 0;
                            tempCell.BorderWidthTop = 0.5f;
                            tempCell.BorderWidthBottom = 0;
                            tempCell.BorderColor = BaseColor.BLACK;
                            BankDetails.AddCell(tempCell);



                            Phrase = new Phrase();
                            Phrase.Add(new Chunk(" ", boldFont));
                            Phrase.Add(new Chunk(" ", normalFont));
                            tempCell = new PdfPCell(Phrase);
                            tempCell.PaddingLeft = 4f;
                            tempCell.Colspan = 1;
                            tempCell.Rowspan = 5;
                            tempCell.PaddingTop = 3f;
                            tempCell.PaddingBottom = 4f;
                            tempCell.Border = 0;
                            tempCell.BorderWidthLeft = 0f;
                            tempCell.BorderWidthRight = 0.5f;
                            tempCell.BorderWidthTop = 0.5f;
                            tempCell.BorderWidthBottom = 0.5f;
                            tempCell.BorderColor = BaseColor.BLACK;
                            BankDetails.AddCell(tempCell);



                            Phrase = new Phrase();
                            Phrase.Add(new Chunk("BANK NAME: ", boldFont));
                            Phrase.Add(new Chunk(BankDetail != null && BankDetail.BankName != null && BankDetail.BankName != null ? BankDetail.BankName.ToUpper() : "N/A", normalFont));
                    
                            tempCell = new PdfPCell(Phrase);
                            tempCell.PaddingLeft = 4f;
                            tempCell.Colspan = 2;
                            tempCell.Rowspan = 1;
                            tempCell.PaddingTop = 3f;
                            tempCell.PaddingBottom = 4f;
                            tempCell.Border = 0;
                            tempCell.BorderWidthLeft = 0.5f;
                            tempCell.BorderWidthRight = 0;
                            tempCell.BorderWidthTop = 0f;
                            tempCell.BorderWidthBottom = 0;
                            tempCell.BorderColor = BaseColor.BLACK;
                            BankDetails.AddCell(tempCell);


                            Phrase = new Phrase();
                            Phrase.Add(new Chunk("BENEFICIARY ACCOUNT NO: ", boldFont));
                            Phrase.Add(new Chunk(BankDetail != null && BankDetail.AccountNumber != null ? BankDetail.AccountNumber.ToUpper() : "N/A", normalFont));
                            tempCell = new PdfPCell(Phrase);
                            tempCell.PaddingLeft = 4f;
                            tempCell.Colspan = 2;
                            tempCell.Rowspan = 1;
                            tempCell.PaddingTop = 3f;
                            tempCell.PaddingBottom = 4f;
                            tempCell.Border = 0;
                            tempCell.BorderWidthLeft = 0.5f;
                            tempCell.BorderWidthRight = 0;
                            tempCell.BorderWidthTop = 0f;
                            tempCell.BorderWidthBottom = 0;
                            tempCell.BorderColor = BaseColor.BLACK;
                            BankDetails.AddCell(tempCell);



                            Phrase = new Phrase();
                            Phrase.Add(new Chunk("IFSC CODE: ", boldFont));
                            Phrase.Add(new Chunk(BankDetail != null && BankDetail.IFSCCode != null ? BankDetail.IFSCCode.ToUpper() : "N/A", normalFont));
                            tempCell = new PdfPCell(Phrase);
                            tempCell.PaddingLeft = 4f;
                            tempCell.Colspan = 2;
                            tempCell.Rowspan = 1;
                            tempCell.PaddingTop = 3f;
                            tempCell.PaddingBottom = 4f;
                            tempCell.Border = 0;
                            tempCell.BorderWidthLeft = 0.5f;
                            tempCell.BorderWidthRight = 0;
                            tempCell.BorderWidthTop = 0.0f;
                            tempCell.BorderWidthBottom = 0;
                            tempCell.BorderColor = BaseColor.BLACK;
                            BankDetails.AddCell(tempCell);



                            Phrase = new Phrase();
                            Phrase.Add(new Chunk("BANK SWIFT CODE: ", boldFont));
                            Phrase.Add(new Chunk( "N/A", normalFont));
                            tempCell = new PdfPCell(Phrase);
                            tempCell.PaddingLeft = 4f;
                            tempCell.Colspan = 2;
                            tempCell.Rowspan = 1;
                            tempCell.PaddingTop = 2f;
                            tempCell.PaddingBottom = 7f;
                            tempCell.Border = 0;
                            tempCell.BorderWidthLeft = 0.5f;
                            tempCell.BorderWidthRight = 0;
                            tempCell.BorderWidthTop = 0f;
                            tempCell.BorderWidthBottom = 0.5f;
                            tempCell.BorderColor = BaseColor.BLACK;
                            BankDetails.AddCell(tempCell);

                            tempCell = ConfigControls.GetPdfTableCell(" ", 0, 0, 7, 1, 7f, Font.NORMAL, 2f, 4f, false, false, false, false, BaseColor.BLACK);
                            BankDetails.AddCell(tempCell);

                            doc.Add(BankDetails);
                            BankDetails = new PdfPTable(columns3);
                            BankDetails.WidthPercentage = 100;


                            float[] columns4 = new float[1];
                            columns4 = new float[3];
                            columns4[0] = 225f;
                            columns4[1] = 210f;
                            columns4[2] = 85f;


                            #endregion

                            #endregion

                        Byte[] bytes = stream.ToArray();
                        String file = Convert.ToBase64String(bytes);
                    } // End of PdfWriter using
                } // End of inner connGet using
            } // End of try block
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try { if (doc.IsOpen()) doc.Close(); } catch { }
            }

            return true;
        }

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

            // ? Explicit column list for tradedocumentitems
            var itemsQuery = @"
        SELECT 
            id, tradedocumentsid, itemid, categoryid, serialno, batchno, modelno, expirydate,
            mfgdate, item, quantity, unit, price_per_unit, discount_percentage, discount_amount,
            created_on, tax_amount, tax_percentage, total_amount, itemcode, addcessamount
        FROM tradedocumentitems
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
                    Item = itemsReader.IsDBNull("item") ? "" : itemsReader.GetString("item"),
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
                });
            }

            return bill;
        }


        //public PdfPTable InvoiceHeader(InvoiceSave Model, IWebHostEnvironment _env, CommonCompanyModel companydetail)
        //{
        //    float[] columns = new float[50];
        //    PdfPTable datatable = new PdfPTable(columns);
        //    try
        //    {
        //        string ImagePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Images");
        //        string POFilePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "POFile");
        //        string FontPath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Font");

        //        BaseFont bffont = BaseFont.CreateFont(Path.Combine(FontPath, "ARIAL.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        //        BaseFont bfRupeesfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial_with_rupee.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        //        BaseFont bfArialBlackfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial-black.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        //        Font fontozel = new Font(bffont, 7f, Font.NORMAL);

        //        Font fontRupees = new Font(bfRupeesfont, 6f, Font.NORMAL);
        //        Font fontRupeesBold = new Font(bfRupeesfont, 6f, Font.BOLD);

        //        Font fontArialBlack = new Font(bfArialBlackfont, 6f, Font.BOLD);


        //        #region Header

        //        columns[0] = 5f;
        //        columns[1] = 5f;
        //        columns[2] = 5f;
        //        columns[3] = 5f;
        //        columns[4] = 5f;
        //        columns[5] = 5f;
        //        columns[6] = 5f;
        //        columns[7] = 5f;
        //        columns[8] = 5f;
        //        columns[9] = 5f;
        //        columns[10] = 5f;
        //        columns[11] = 5f;
        //        columns[12] = 5f;
        //        columns[13] = 5f;
        //        columns[14] = 5f;
        //        columns[15] = 5f;
        //        columns[16] = 5f;
        //        columns[17] = 5f;
        //        columns[18] = 5f;
        //        columns[19] = 5f;
        //        columns[20] = 5f;
        //        columns[21] = 5f;
        //        columns[22] = 5f;
        //        columns[23] = 5f;
        //        columns[24] = 5f;
        //        columns[25] = 5f;
        //        columns[26] = 5f;
        //        columns[27] = 5f;
        //        columns[28] = 5f;
        //        columns[29] = 5f;
        //        columns[30] = 5f;
        //        columns[31] = 5f;
        //        columns[32] = 5f;
        //        columns[33] = 5f;
        //        columns[34] = 5f;
        //        columns[35] = 5f;
        //        columns[36] = 5f;
        //        columns[37] = 5f;
        //        columns[38] = 5f;
        //        columns[39] = 5f;
        //        columns[40] = 5f;
        //        columns[41] = 5f;
        //        columns[42] = 5f;
        //        columns[43] = 5f;
        //        columns[44] = 5f;
        //        columns[45] = 5f;
        //        columns[46] = 5f;
        //        columns[47] = 5f;
        //        columns[48] = 5f;
        //        columns[49] = 5f;


        //        datatable.TotalWidth = 520f;
        //        datatable.LockedWidth = true;
        //        string RsGroup = "RsGroup.png";
        //        iTextSharp.text.Image imgLogo = iTextSharp.text.Image.GetInstance(Path.Combine(ImagePath, RsGroup));
        //        // iTextSharp.text.Image imgLogo = iTextSharp.text.Image.GetInstance("https://erp.rs-group.co.in/assets/images/login-logo.png");
        //        //string logoPath = (companydetail != null && companydetail.LogoPic1 != null && !string.IsNullOrEmpty(companydetail.LogoPic1.DocumentPath)) ? companydetail.LogoPic1.DocumentPath : Path.Combine(ImagePath, SprinkLogo);
        //        // iTextSharp.text.Image imgLogo = iTextSharp.text.Image.GetInstance(logoPath);
        //        imgLogo.ScaleAbsolute(15f, 15f);
        //        PdfPCell cell = new PdfPCell(imgLogo, true);
        //        cell.Colspan = 11;
        //        cell.Rowspan = 4;
        //        cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT;
        //        cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_MIDDLE;
        //        cell.PaddingTop = -25f;
        //        cell.PaddingBottom = 10f;
        //        cell.BorderWidthLeft = 0f;
        //        cell.BorderWidthRight = 0f;
        //        cell.BorderWidthTop = 0f;
        //        cell.BorderWidthBottom = 0f;
        //        datatable.AddCell(cell);

        //        var invoicetype = Model.IsInvoice == null || Model.IsInvoice == 1 ? 1 : 2;

        //        if (invoicetype == 1)
        //        {
        //            Font ArialFont = new Font(bfArialBlackfont, 12f, Font.NORMAL);
        //            var phInvoiceText = new Phrase();
        //            phInvoiceText.Add(new Chunk("INVOICE", ArialFont));
        //            cell = new PdfPCell(phInvoiceText);
        //            cell.PaddingTop = -7f;
        //            cell.PaddingBottom = 5f;
        //            cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
        //            cell.Colspan = 39;
        //            cell.Border = 0;
        //            datatable.AddCell(cell);
        //        }
        //        else
        //        {
        //            Font ArialFont = new Font(bfArialBlackfont, 12f, Font.NORMAL);
        //            var phInvoiceText = new Phrase();
        //            phInvoiceText.Add(new Chunk("PROFORMA INVOICE", ArialFont));
        //            cell = new PdfPCell(phInvoiceText);
        //            cell.PaddingTop = -7f;
        //            cell.PaddingBottom = 5f;
        //            cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
        //            cell.Colspan = 39;
        //            cell.Border = 0;
        //            datatable.AddCell(cell);
        //        }

        //        Font normalFont = new Font(bffont, 8.2f, Font.NORMAL);
        //        Font boldFont = new Font(bffont, 8.2f, Font.BOLD);

        //        var phInvoice = new Phrase();
        //        phInvoice.Add(new Chunk("# ", normalFont));
        //        phInvoice.Add(new Chunk(Model.InvoiceNumber, normalFont));
        //        cell = new PdfPCell(phInvoice);
        //        cell.PaddingTop = -5f;
        //        cell.PaddingBottom = 5f;
        //        cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
        //        cell.Colspan = 39;
        //        cell.Border = 0;
        //        datatable.AddCell(cell);

        //        var phrase = new Phrase();
        //        phrase.Add(new Chunk("Date: ", normalFont));
        //        phrase.Add(new Chunk(DateTime.UtcNow.ToString("dd-MM-yyyy"), normalFont));
        //        cell = new PdfPCell(phrase);
        //        cell.PaddingTop = -2f;
        //        cell.PaddingBottom = 4f;
        //        cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
        //        cell.Colspan = 39;
        //        cell.Border = 0;
        //        datatable.AddCell(cell);

        //        var IRN = "";
        //        if (IRN != "")
        //        {
        //            var phraseIRN = new Phrase();
        //            phraseIRN.Add(new Chunk("IRN: ", normalFont));
        //            phraseIRN.Add(new Chunk(IRN, normalFont));

        //            cell = new PdfPCell(phraseIRN);
        //            cell.PaddingTop = -3f;
        //            cell.PaddingBottom = 4f;
        //            cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT;
        //            cell.Colspan = 39;
        //            cell.Border = 0;
        //            datatable.AddCell(cell);
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return datatable;
        //}
        //public PdfPTable InvoiceFooter(CommonCompanyModel companydetail, IHostingEnvironment _env)
        //{
        //    float[] columns = new float[50];
        //    PdfPTable Footer = new PdfPTable(columns);
        //    try
        //    {
        //        string ImagePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Images");
        //        string POFilePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "POFile");
        //        string FontPath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Font");

        //        BaseFont bffont = BaseFont.CreateFont(Path.Combine(FontPath, "ARIAL.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        //        BaseFont bfRupeesfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial_with_rupee.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        //        Font fontozel = new Font(bffont, 7f, Font.NORMAL);

        //        Font fontRupees = new Font(bfRupeesfont, 6f, Font.NORMAL);
        //        Font fontRupeesBold = new Font(bfRupeesfont, 6f, Font.BOLD);


        //        columns[0] = 5f;
        //        columns[1] = 5f;
        //        columns[2] = 5f;
        //        columns[3] = 5f;
        //        columns[4] = 5f;
        //        columns[5] = 5f;
        //        columns[6] = 5f;
        //        columns[7] = 5f;
        //        columns[8] = 5f;
        //        columns[9] = 5f;
        //        columns[10] = 5f;
        //        columns[11] = 5f;
        //        columns[12] = 5f;
        //        columns[13] = 5f;
        //        columns[14] = 5f;
        //        columns[15] = 5f;
        //        columns[16] = 5f;
        //        columns[17] = 5f;
        //        columns[18] = 5f;
        //        columns[19] = 5f;
        //        columns[20] = 5f;
        //        columns[21] = 5f;
        //        columns[22] = 5f;
        //        columns[23] = 5f;
        //        columns[24] = 5f;
        //        columns[25] = 5f;
        //        columns[26] = 5f;
        //        columns[27] = 5f;
        //        columns[28] = 5f;
        //        columns[29] = 5f;
        //        columns[30] = 5f;
        //        columns[31] = 5f;
        //        columns[32] = 5f;
        //        columns[33] = 5f;
        //        columns[34] = 5f;
        //        columns[35] = 5f;
        //        columns[36] = 5f;
        //        columns[37] = 5f;
        //        columns[38] = 5f;
        //        columns[39] = 5f;
        //        columns[40] = 5f;
        //        columns[41] = 5f;
        //        columns[42] = 5f;
        //        columns[43] = 5f;
        //        columns[44] = 5f;
        //        columns[45] = 5f;
        //        columns[46] = 5f;
        //        columns[47] = 5f;
        //        columns[48] = 5f;
        //        columns[49] = 5f;

        //        Footer = new PdfPTable(columns);
        //        Footer.TotalWidth = 520f;
        //        Footer.LockedWidth = true;
        //        // Footer.PaddingTop = 10;
        //        PdfPCell cell = new PdfPCell();

        //        Font normalFont2 = new Font(bffont, 7f, Font.NORMAL);
        //        Font boldFont2 = new Font(bffont, 7f, Font.BOLD);
        //        var phrase2 = new Phrase();
        //        phrase2.Add(new Chunk(companydetail.CompanyName, boldFont2));
        //        phrase2.Add(new Chunk(" | CIN NO.: " + (companydetail.CINNo != null ? companydetail.CINNo.Value : ""), boldFont2));
        //        //if (companydetail.CompanyCode != "05"/*Model.CompanyId != 65*/)
        //        //{
        //        //    phrase2.Add(new Chunk(" | CIN NO.: " + /*companydetail.CINNo*/"", boldFont2));
        //        //}
        //        //else
        //        //{
        //        //    phrase2.Add(new Chunk(" | Reg NO.: " + companydetail.RegistrationNo, boldFont2));
        //        //}
        //        cell = new PdfPCell(phrase2);
        //        cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER;
        //        cell.Colspan = 50;
        //        cell.Border = 0;
        //        cell.PaddingTop = 4f;
        //        cell.BorderColor = cell.BackgroundColor = new iTextSharp.text.BaseColor(187, 187, 187);
        //        Footer.AddCell(cell);

        //        var phrase3 = new Phrase();
        //        phrase3.Add(new Chunk("Reg.Office: ", boldFont2));
        //        phrase3.Add(new Chunk(companydetail.AuthOfficeAddress != null ? companydetail.AuthOfficeAddress : string.Empty, normalFont2));
        //        //if (companydetail.CompanyCode != "05"/*Model.CompanyId != 65*/)
        //        //{
        //        //    phrase3.Add(new Chunk(companydetail.AuthOfficeAddress != null ? companydetail.AuthOfficeAddress : string.Empty/* + ", " + Model.CompanyCity.CityName + "-" + Model.CompanyPinCode + ", " + Model.CompanyState.StateName + ", " + Model.CompanyCountry.CountryName*/, normalFont2));
        //        //}
        //        //else
        //        //{
        //        //    phrase3.Add(new Chunk(Model.BillingFromM != null ? Model.BillingFromM.BillingFrom : string.Empty + ", " + Model.CompanyCity != null ? Model.CompanyCity.CityName : string.Empty, normalFont2));
        //        //}
        //        cell = new PdfPCell(phrase3);
        //        cell.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER;
        //        cell.Colspan = 50;
        //        cell.Border = 0;
        //        cell.BorderColor = cell.BackgroundColor = new iTextSharp.text.BaseColor(187, 187, 187);
        //        Footer.AddCell(cell);

        //        Footer.AddCell(ConfigControls.GetPdfTableCell((companydetail.AuthContactNo != null ? companydetail.AuthContactNo.ISDCode + " " + companydetail.AuthContactNo.MobileNo : "") + " | " + companydetail.AuthEmailId + " | " + companydetail.Website, 1, 0, 50, 1, 7f, Font.NORMAL, 0f, 6f, false, false, false, false, new iTextSharp.text.BaseColor(187, 187, 187), BaseColor.BLACK));
        //        // Footer.PaddingTop = totalHeight - Footer.TotalHeight;
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return Footer;
        //}
        public PdfPTable GenrateBlankRow(int rowNo)
        {

            float[] columns2 = new float[1];

            columns2 = new float[1];
            columns2[0] = 520f;



            PdfPTable BlankRowFilter = new PdfPTable(columns2);
            BlankRowFilter.TotalWidth = 520f;
            //Particulares.LockedWidth = true;

            PdfPCell tempCell = new PdfPCell();

            for (int i = 0; i < rowNo; i++)
            {
                BlankRowFilter.AddCell(ConfigControls.GetPdfTableCell(" ", 0, 0, 1, 1, 7f, Font.BOLD, 5f, 0f, false, false, false, false, BaseColor.WHITE));
            }

            return BlankRowFilter;
        }

        public static String ConvertAmount(double amount)
        {
            try
            {
                Int64 amount_int = (Int64)amount;
                Int64 amount_dec = (Int64)Math.Round((amount - (double)(amount_int)) * 100);
                if (amount_dec == 0)
                {
                    return Convertvalue(amount_int) + " Only.";
                }
                else
                {
                    return Convertvalue(amount_int) + " Point " + Convertvalue(amount_dec) + " Only.";
                }
            }
            catch (Exception e)
            {
                // TODO: handle exception  
            }
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
            if (i < 20)
            {
                return units[i];
            }
            if (i < 100)
            {
                return tens[i / 10] + ((i % 10 > 0) ? " " + Convertvalue(i % 10) : "");
            }
            if (i < 1000)
            {
                return units[i / 100] + " Hundred"
                        + ((i % 100 > 0) ? " And " + Convertvalue(i % 100) : "");
            }
            if (i < 100000)
            {
                return Convertvalue(i / 1000) + " Thousand "
                + ((i % 1000 > 0) ? " " + Convertvalue(i % 1000) : "");
            }
            if (i < 10000000)
            {
                return Convertvalue(i / 100000) + " Lakh "
                        + ((i % 100000 > 0) ? " " + Convertvalue(i % 100000) : "");
            }
            if (i < 1000000000)
            {
                return Convertvalue(i / 10000000) + " Crore "
                        + ((i % 10000000 > 0) ? " " + Convertvalue(i % 10000000) : "");
            }
            return Convertvalue(i / 1000000000) + " Arab "
                    + ((i % 1000000000 > 0) ? " " + Convertvalue(i % 1000000000) : "");
        }

        private static PdfPCell CreateLabelCell(string text, BaseColor borderColor)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, FontFactory.GetFont("Arial", 8f, Font.BOLD)));
            cell.Padding = 4f;
            cell.BorderColor = borderColor;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            return cell;
        }

        private static PdfPCell CreateValueCell(string text, BaseColor borderColor)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, FontFactory.GetFont("Arial", 8f)));
            cell.Padding = 4f;
            cell.BorderColor = borderColor;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            return cell;
        }


        public class PdfPageEvents : PdfPageEventHelper
        {
            PdfContentByte cb;
            PdfTemplate headerTemplate, footerTemplate;
            BaseFont bf;
            DateTime printTime = DateTime.Now;
            iTextSharp.text.Image logo;
            string companyName;
            IWebHostEnvironment _env;
            BusinessProfileModel companyModel;


            public PdfPageEvents(BusinessProfileModel model, IWebHostEnvironment _env)
            {
                this.companyName = model.BusinessName;
                this._env = _env;
                this.companyModel = model;

            }

            public override void OnOpenDocument(PdfWriter writer, iTextSharp.text.Document document)
            {
                string ImagePath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Images");
                try
                {
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    headerTemplate = cb.CreateTemplate(100, 100);
                    footerTemplate = cb.CreateTemplate(50, 50);

                    string RsGroup = "MuneemJiLogo.png";
                    logo = iTextSharp.text.Image.GetInstance(Path.Combine(ImagePath, RsGroup));

                }
                catch (DocumentException de)
                {
                    // Handle exception here
                }
                catch (System.IO.IOException ioe)
                {
                    // Handle exception here
                }
            }

            public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);
                string FontPath = Path.Combine(_env.WebRootPath, /*"wwwroot",*/ "DataContainer", "Font");
                BaseFont bffont = BaseFont.CreateFont(Path.Combine(FontPath, "ARIAL.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                BaseFont bfRupeesfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial_with_rupee.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                BaseFont bfArialBlackfont = BaseFont.CreateFont(Path.Combine(FontPath, "arial-black.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                // Calculate header text
                string header = string.Empty;

                header = "PROFORMA INVOICE";

                // Draw header text centered at the top
                PdfContentByte cb = writer.DirectContent;
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

                // Begin writing text
                cb.BeginText();
                cb.SetFontAndSize(bf, 14); // Set your preferred font size

                float centerX = document.PageSize.Width / 2;
                float topY = document.PageSize.Top - 40f - 14f; // Adjust vertical position as needed

                cb.ShowTextAligned(Element.ALIGN_CENTER, header, centerX, topY, 0);
                cb.EndText();



                Font InvoiceFont = new Font(bffont, 9, Font.BOLD); // Create a bold font



                float len = bf.GetWidthPoint(header, 12);
                //cb.AddTemplate(headerTemplate, document.PageSize.GetLeft(80) + len, document.PageSize.GetTop(40));

                float desiredWidth = 48f;
                float desiredHeight = 47f; // Adjust the height as needed
                logo.ScaleAbsolute(desiredWidth, desiredHeight);

                // Set the absolute position of the logo
                float xPos = document.PageSize.GetLeft(40);
                float yPos = document.PageSize.GetTop(15) - logo.ScaledHeight;
                logo.SetAbsolutePosition(xPos, yPos);
                logo.PaddingTop = 5f;
                //logo.Width(250f,250f);
                cb.AddImage(logo);

                cb.BeginText();
                header = "A Step Towards The Best";
                cb.SetFontAndSize(InvoiceFont.BaseFont, InvoiceFont.Size);
                cb.SetTextMatrix(document.PageSize.GetLeft(37), document.PageSize.GetTop(75));
                cb.ShowText(header);

                cb.EndText();

                BaseColor footerBgColor = new BaseColor(200, 200, 200); // Light gray color

                // Define the footer height
                float footerHeight = 60; // Adjust as needed


                string CompanyName = companyName;
                string CIn = "U31900RJ2021PTC073464";



                // Draw the background color for the footer area
                //float footerYPosition = document.PageSize.GetBottom(footerHeight);

                // Draw the background color for the footer area
                cb.SaveState(); // Save the current graphic state
                cb.SetColorFill(footerBgColor);
                cb.Rectangle(0, 0, document.PageSize.Width, footerHeight);
                cb.Fill();
                cb.RestoreState();





                // Total height of the footer

                // Calculate the available space for text (total height minus top and bottom padding)
                float verticalPadding = 10f; // Equal padding at top and bottom
                float availableHeight = footerHeight - (2 * verticalPadding);

                // Calculate line positions with equal spacing
                float lineSpacing = availableHeight / 3f; // Equal spacing for 3 lines
                float yLine1 = footerHeight - verticalPadding; // Top line (starting from top of footer)
                float yLine2 = yLine1 - lineSpacing; // Middle line
                float yLine3 = yLine2 - lineSpacing; // Bottom line

                // Draw Footer Background
                cb.SaveState();
                cb.SetColorFill(footerBgColor);
                cb.Rectangle(0, 0, document.PageSize.Width, footerHeight);
                cb.Fill();
                cb.RestoreState();

                // Line 1: Company Info (top line)
                string line1 = CompanyName + "  | CIN NO.: " + CIn + "  | PAN NO.: " + "EXLPK1621M";
                float line1Width = bf.GetWidthPoint(line1, 8);
                float xLine1 = (document.PageSize.Width - line1Width) / 2;
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(xLine1, yLine1 - 8); // Subtract font height (approx) for proper alignment
                cb.ShowText(line1);
                cb.EndText();

                // Line 2: Contact Info (middle line)
                string line2 = "+91 " + (companyModel.PhoneNumber ?? "") + " | " + (companyModel.Email ?? "");
                float line2Width = bf.GetWidthPoint(line2, 8);
                float xLine2 = (document.PageSize.Width - line2Width) / 2;
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(xLine2, yLine2 - 8); // Subtract font height
                cb.ShowText(line2);
                cb.EndText();

                // Line 3: Address (bottom line)
                string line3 = companyModel.Address?.Replace("\n", "").Trim() ?? "";
                float line3Width = bf.GetWidthPoint(line3, 8);
                float xLine3 = (document.PageSize.Width - line3Width) / 2;
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(xLine3, yLine3 - 8); // Subtract font height
                cb.ShowText(line3);
                cb.EndText();
            }

            public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnCloseDocument(writer, document);

                headerTemplate.BeginText();
                headerTemplate.SetFontAndSize(bf, 12);
                headerTemplate.SetTextMatrix(0, 0);
                headerTemplate.ShowText("" + (writer.PageNumber - 1));
                headerTemplate.EndText();

                footerTemplate.BeginText();
                footerTemplate.SetFontAndSize(bf, 12);
                footerTemplate.SetTextMatrix(0, 0);
                footerTemplate.ShowText("" + (writer.PageNumber - 1));
                footerTemplate.EndText();
            }
        }



        public class HeaderFooter : PdfPageEventHelper
        {
            // Define fonts for the header and footer
            private readonly Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            private readonly Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
            {
                // Add a header
                PdfPTable headerTable = new PdfPTable(1);
                headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                headerTable.DefaultCell.Border = 0;

                PdfPCell headerCell = new PdfPCell(new Phrase("This is the Header", headerFont));
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Border = Rectangle.NO_BORDER;
                headerTable.AddCell(headerCell);
                headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 10, writer.DirectContent);

                // Add a footer
                PdfPTable footerTable = new PdfPTable(1);
                footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                footerTable.DefaultCell.Border = 0;

                PdfPCell footerCell = new PdfPCell(new Phrase("This is the Footer - Page " + writer.PageNumber, footerFont));
                footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                footerCell.Border = Rectangle.NO_BORDER;
                footerTable.AddCell(footerCell);
                footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 10, writer.DirectContent);
            }
        }

        public PartyModel PartDetailForPdfById(int id)
        {
            PartyModel model = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT ps.* , ss.name , ss.code  FROM parties as ps left join sates as ss on ss.id = ps.stateid  WHERE id = @id";

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

    }
}



