using Insight.Database;
using MUNEEMJI.Models;
using MUNEEMJI.Models.Setting;
using Npgsql;
using System.Data;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>Everything one rendered document needs.</summary>
    public class QuestDocumentData
    {
        public PurchaseBill Bill { get; set; } = new PurchaseBill();
        public PartyModel Party { get; set; }
        public PdfCompanyContext Context { get; set; } = new PdfCompanyContext();
        public List<PurchaseBillItem> Items { get; set; } = new List<PurchaseBillItem>();

        /// <summary>Title printed at the top ("Tax Invoice", "Credit Note", ...).</summary>
        public string DocumentTitle { get; set; } = "Invoice";

        /// <summary>True when supplier and buyer are in the same state (CGST + SGST).</summary>
        public bool IsDomestic { get; set; }

        /// <summary>Running balance of the party, only queried when the option is on.</summary>
        public decimal PartyCurrentBalance { get; set; }

        // ---- Derived totals, computed once by the loader ----
        public decimal TotalQuantity { get; set; }
        public decimal TotalTaxable { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalCess { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GrandTotal { get; set; }

        /// <summary>Tax rate -> (taxable, tax) rows for the GST summary block.</summary>
        public List<TaxSummaryRow> TaxSummary { get; set; } = new List<TaxSummaryRow>();

        public PrintSettingsModel Settings => Context?.Settings ?? new PrintSettingsModel();
        public BusinessProfileModel Company => Context?.Company ?? new BusinessProfileModel();
    }

    public class TaxSummaryRow
    {
        public decimal Rate { get; set; }
        public decimal Taxable { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Cess { get; set; }
    }

    /// <summary>
    /// Loads a trade document plus its party for PDF rendering.
    /// Unlike the legacy generators this scopes every query by companyid.
    /// </summary>
    public class QuestPdfDataLoader
    {
        private readonly string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public async Task<QuestDocumentData> LoadAsync(int tradeDocumentId, PdfCompanyContext context, string documentTitle)
        {
            var data = new QuestDocumentData
            {
                Context = context ?? new PdfCompanyContext(),
                DocumentTitle = documentTitle
            };

            data.Bill = await LoadBillAsync(tradeDocumentId, data.Context.CompanyId) ?? new PurchaseBill();

            if (data.Bill.PartyId > 0)
                data.Party = LoadParty(data.Bill.PartyId, data.Context.CompanyId);

            // A configurable title on the transaction overrides the generator default.
            if (data.Context.TransactionName != null && !string.IsNullOrWhiteSpace(data.Context.TransactionName.DisplayTitle))
                data.DocumentTitle = data.Context.TransactionName.DisplayTitle;

            data.Items = data.Bill.BillItems?.Where(x => x != null).ToList() ?? new List<PurchaseBillItem>();

            data.IsDomestic = data.Party != null && data.Bill.StateId == data.Party.StateId;

            if (data.Settings.PrintCurrentBalanceParty && data.Bill.PartyId > 0)
                data.PartyCurrentBalance = LoadPartyBalance(data.Bill.PartyId, data.Context.CompanyId);

            ComputeTotals(data);

            return data;
        }

        /// <summary>
        /// Builds a fully populated dummy document. Used by the Settings &gt; Print
        /// live preview when the company has no saved transaction to render yet.
        /// </summary>
        public static QuestDocumentData BuildSample(PdfCompanyContext context, string documentTitle)
        {
            var data = new QuestDocumentData
            {
                Context = context ?? new PdfCompanyContext(),
                DocumentTitle = documentTitle
            };

            if (data.Context.TransactionName != null && !string.IsNullOrWhiteSpace(data.Context.TransactionName.DisplayTitle))
                data.DocumentTitle = data.Context.TransactionName.DisplayTitle;

            data.Bill = new PurchaseBill
            {
                Id = 0,
                BillNumber = "SAMPLE-001",
                BillDate = DateTime.Today,
                InvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(15),
                Time = new TimeSpan(10, 30, 0),
                BillingName = "Sample Customer Pvt. Ltd.",
                BillingAddress = "12, Industrial Area, Phase II",
                ShippingAddress = "Warehouse 4, Transport Nagar",
                PaymentType = "Credit",
                StateOfSupply = "Same State",
                Description = "This is a preview document. Real transactions will use their own data.",
                TransportName = "Sample Logistics",
                VehicleNumber = "RJ 14 AB 1234",
                paidReciveamount = 5000m,
                IsRoundOff = true,
                RoundOffValue = 0.40m
            };

            data.Party = new PartyModel
            {
                Id = 0,
                PartyName = "Sample Customer Pvt. Ltd.",
                GSTIN = "08AAACS1234A1Z5",
                PhoneNumber = "9876543210",
                BillingAddress = "12, Industrial Area, Phase II",
                ShippingAddress = "Warehouse 4, Transport Nagar",
                StateName = "Rajasthan",
                StateCode = "08",
                StateId = data.Bill.StateId
            };

            data.Bill.BillItems = new List<PurchaseBillItem>
            {
                NewSampleItem(1, "Brittania Chocolate Cake", "12345678", 100m, "Box", 100m, 1m, 100m, 5m),
                NewSampleItem(2, "Cadbury Chocolate", "34567890", 50m, "Pac", 150m, 10m, 750m, 5m),
                NewSampleItem(3, "Sample Service", "9983", 1m, "Nos", 3000m, 0m, 0m, 18m)
            };

            data.Items = data.Bill.BillItems;
            data.IsDomestic = true;
            data.PartyCurrentBalance = 12500m;

            ComputeTotals(data);
            return data;
        }

        private static PurchaseBillItem NewSampleItem(int id, string name, string hsn, decimal qty,
            string unit, decimal price, decimal discPct, decimal discAmt, decimal taxPct)
        {
            var taxable = qty * price - discAmt;
            var tax = Math.Round(taxable * taxPct / 100m, 2);

            return new PurchaseBillItem
            {
                Id = id,
                Item = name,
                ItemCode = "IC-" + id.ToString("000"),
                HSNCode = hsn,
                batchno = "N" + (1230 + id),
                serialno = "SN" + id.ToString("0000"),
                modelno = "A" + (12340 + id),
                Description = name + " description",
                Size = "Med/32",
                MRP = price,
                FreeQuantity = id == 2 ? 1m : 0m,
                ExpiryDate = DateTime.Today.AddYears(1),
                ManufacturingDate = DateTime.Today,
                mfgdate = DateTime.Today.ToString("yyyy-MM-dd"),
                Quantity = qty,
                Unit = unit,
                PricePerUnit = price,
                DiscountPercentage = discPct,
                DiscountAmount = discAmt,
                TaxPercentage = taxPct,
                TaxAmount = tax,
                TotalAmount = taxable + tax,
                AddCessAmount = 0m
            };
        }

        // -----------------------------------------------------------------
        private static void ComputeTotals(QuestDocumentData data)
        {
            decimal qty = 0, taxable = 0, tax = 0, cess = 0, discount = 0;
            var buckets = new Dictionary<decimal, TaxSummaryRow>();

            foreach (var item in data.Items)
            {
                var lineGross = item.Quantity * item.PricePerUnit;
                var lineDiscount = item.DiscountAmount;
                var lineTaxable = lineGross - lineDiscount;
                var lineCess = item.AddCessAmount ?? 0m;

                qty += item.Quantity;
                discount += lineDiscount;
                taxable += lineTaxable;
                tax += item.TaxAmount;
                cess += lineCess;

                var rate = item.TaxPercentage;
                if (!buckets.TryGetValue(rate, out var row))
                {
                    row = new TaxSummaryRow { Rate = rate };
                    buckets[rate] = row;
                }
                row.Taxable += lineTaxable;
                row.TaxAmount += item.TaxAmount;
                row.Cess += lineCess;
            }

            data.TotalQuantity = qty;
            data.TotalTaxable = taxable;
            data.TotalTax = tax;
            data.TotalCess = cess;
            data.TotalDiscount = discount;
            data.TaxSummary = buckets.Values.OrderBy(x => x.Rate).ToList();

            // Prefer the stored final amount; fall back to a computed total when it is 0.
            data.GrandTotal = data.Bill.FinalAmount != 0
                ? data.Bill.FinalAmount
                : taxable + tax + cess
                  + data.Bill.ShippingAmount + data.Bill.PackingAmount
                  + data.Bill.AdjustmentAmount + data.Bill.RoundOffValue;
        }

        // -----------------------------------------------------------------
        private async Task<PurchaseBill> LoadBillAsync(int id, int companyId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string billQuery = @"
SELECT
    id, bill_number, bill_date, stateid, state_of_supply, phone_no, po_no, po_date,
    eway_bill_no, transport_name, delivery_location, vehicle_number, delivery_date,
    payment_type, description, image_path, round_off, total, paidreciveamount,
    created_date, tradedocumenttypesid, partyid, orderstatusid, duedate, orderno,
    orderdate, challanno, challandate, iscredit, billingname, billingaddress,
    shippingaddress, invoicenumber, invoicedate, ""time"", paymenttermid, field5,
    field6, documentpath, noofcopi, discount_percent, discount_amount, tax_percentage,
    tax_amount, shipping_amount, packing_amount, adjustment_amount, tdstcs_percentage,
    tdstcs_amount, isroundoff, final_amount, tcstdstype, isreceive, returnno, companyid
FROM tradedocuments
WHERE id = @Id AND (@CompanyId <= 0 OR companyid = @CompanyId)";

            using var billCommand = new NpgsqlCommand(billQuery, connection);
            billCommand.Parameters.AddWithValue("@Id", id);
            billCommand.Parameters.AddWithValue("@CompanyId", companyId);

            using var r = await billCommand.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;

            int timeOrdinal = r.GetOrdinal("time");

            var bill = new PurchaseBill
            {
                Id = r.GetInt32("id"),
                BillNumber = r.IsDBNull("bill_number") ? "" : r.GetString("bill_number"),
                BillDate = r.IsDBNull("bill_date") ? DateTime.MinValue : r.GetDateTime("bill_date"),
                StateId = r.IsDBNull("stateid") ? 0 : r.GetInt32("stateid"),
                StateOfSupply = r.IsDBNull("state_of_supply") ? "" : r.GetString("state_of_supply"),
                PhoneNo = r.IsDBNull("phone_no") ? "" : r.GetString("phone_no"),
                PONo = r.IsDBNull("po_no") ? "" : r.GetString("po_no"),
                PODate = r.IsDBNull("po_date") ? DateTime.MinValue : r.GetDateTime("po_date"),
                EWayBillNo = r.IsDBNull("eway_bill_no") ? "" : r.GetString("eway_bill_no"),
                TransportName = r.IsDBNull("transport_name") ? "" : r.GetString("transport_name"),
                DeliveryLocation = r.IsDBNull("delivery_location") ? "" : r.GetString("delivery_location"),
                VehicleNumber = r.IsDBNull("vehicle_number") ? "" : r.GetString("vehicle_number"),
                DeliveryDate = r.IsDBNull("delivery_date") ? DateTime.MinValue : r.GetDateTime("delivery_date"),
                PaymentType = r.IsDBNull("payment_type") ? "" : r.GetString("payment_type"),
                Description = r.IsDBNull("description") ? "" : r.GetString("description"),
                ImagePath = r.IsDBNull("image_path") ? "" : r.GetString("image_path"),
                RoundOffValue = r.IsDBNull("round_off") ? 0 : r.GetDecimal("round_off"),
                Total = r.IsDBNull("total") ? 0 : r.GetDecimal("total"),
                paidReciveamount = r.IsDBNull("paidreciveamount") ? 0 : r.GetDecimal("paidreciveamount"),
                CreatedDate = r.IsDBNull("created_date") ? DateTime.MinValue : r.GetDateTime("created_date"),
                tradedocumenttypesid = r.IsDBNull("tradedocumenttypesid") ? 0 : r.GetInt32("tradedocumenttypesid"),
                PartyId = r.IsDBNull("partyid") ? 0 : r.GetInt32("partyid"),
                orderstatusid = r.IsDBNull("orderstatusid") ? 0 : r.GetInt32("orderstatusid"),
                DueDate = r.IsDBNull("duedate") ? DateTime.MinValue : r.GetDateTime("duedate"),
                OrderNo = r.IsDBNull("orderno") ? "" : r.GetString("orderno"),
                OrderDate = r.IsDBNull("orderdate") ? DateTime.MinValue : r.GetDateTime("orderdate"),
                ChallanNo = r.IsDBNull("challanno") ? "" : r.GetString("challanno"),
                Challandate = r.IsDBNull("challandate") ? DateTime.MinValue : r.GetDateTime("challandate"),
                IsCredit = !r.IsDBNull("iscredit") && r.GetBoolean("iscredit"),
                BillingName = r.IsDBNull("billingname") ? "" : r.GetString("billingname"),
                BillingAddress = r.IsDBNull("billingaddress") ? "" : r.GetString("billingaddress"),
                ShippingAddress = r.IsDBNull("shippingaddress") ? "" : r.GetString("shippingaddress"),
                InvoiceNumber = r.IsDBNull("invoicenumber") ? 0 : r.GetInt32("invoicenumber"),
                InvoiceDate = r.IsDBNull("invoicedate") ? DateTime.MinValue : r.GetDateTime("invoicedate"),
                PaymentTermId = r.IsDBNull("paymenttermid") ? 0 : r.GetInt32("paymenttermid"),
                Field5 = r.IsDBNull("field5") ? "" : r.GetString("field5"),
                Field6 = r.IsDBNull("field6") ? "" : r.GetString("field6"),
                DocumentPath = r.IsDBNull("documentpath") ? "" : r.GetString("documentpath"),
                NoOfCopi = r.IsDBNull("noofcopi") ? 0 : r.GetInt32("noofcopi"),
                DiscountPercent = r.IsDBNull("discount_percent") ? 0 : r.GetDecimal("discount_percent"),
                DiscountAmount = r.IsDBNull("discount_amount") ? 0 : r.GetDecimal("discount_amount"),
                TaxPercentage = r.IsDBNull("tax_percentage") ? 0 : r.GetDecimal("tax_percentage"),
                TaxAmount = r.IsDBNull("tax_amount") ? 0 : r.GetDecimal("tax_amount"),
                ShippingAmount = r.IsDBNull("shipping_amount") ? 0 : r.GetDecimal("shipping_amount"),
                PackingAmount = r.IsDBNull("packing_amount") ? 0 : r.GetDecimal("packing_amount"),
                AdjustmentAmount = r.IsDBNull("adjustment_amount") ? 0 : r.GetDecimal("adjustment_amount"),
                TdsTcsPercentage = r.IsDBNull("tdstcs_percentage") ? 0 : r.GetDecimal("tdstcs_percentage"),
                TdsTcsAmount = r.IsDBNull("tdstcs_amount") ? 0 : r.GetDecimal("tdstcs_amount"),
                IsRoundOff = !r.IsDBNull("isroundoff") && r.GetBoolean("isroundoff"),
                FinalAmount = r.IsDBNull("final_amount") ? 0 : r.GetDecimal("final_amount"),
                IsReceive = !r.IsDBNull("isreceive") && r.GetBoolean("isreceive"),
                ReturnNo = r.IsDBNull("returnno") ? 0 : r.GetDecimal("returnno"),
                Time = r.IsDBNull(timeOrdinal) ? TimeSpan.MinValue : r.GetTimeSpan(timeOrdinal)
            };

            await r.CloseAsync();

            const string itemsQuery = @"
SELECT
    td.id, td.tradedocumentsid, td.itemid, td.categoryid, td.serialno, td.batchno,
    td.modelno, td.expirydate, td.mfgdate, td.item, td.quantity, td.unit,
    td.price_per_unit, td.discount_percentage, td.discount_amount, td.created_on,
    td.tax_amount, td.tax_percentage, td.total_amount, td.itemcode, td.addcessamount,
    bi.item_name AS itemname, bi.item_hsn AS hsncode,
    bi.description AS itemdescription, bi.size AS itemsize, bi.sale_price AS itemmrp
FROM tradedocumentitems AS td
LEFT JOIN billitem AS bi ON td.itemid = bi.id
WHERE td.tradedocumentsid = @BillId
ORDER BY td.id";

            using var itemsCommand = new NpgsqlCommand(itemsQuery, connection);
            itemsCommand.Parameters.AddWithValue("@BillId", id);

            using var ir = await itemsCommand.ExecuteReaderAsync();
            while (await ir.ReadAsync())
            {
                bill.BillItems.Add(new PurchaseBillItem
                {
                    Id = ir.GetInt32("id"),
                    BillId = ir.GetInt32("tradedocumentsid"),
                    TradeDocumentsId = ir.GetInt32("tradedocumentsid"),
                    ItemId = ir.IsDBNull("itemid") ? 0 : ir.GetInt32("itemid"),
                    categoryid = ir.IsDBNull("categoryid") ? 0 : ir.GetInt32("categoryid"),
                    serialno = ir.IsDBNull("serialno") ? "" : ir.GetString("serialno"),
                    batchno = ir.IsDBNull("batchno") ? "" : ir.GetString("batchno"),
                    modelno = ir.IsDBNull("modelno") ? "" : ir.GetString("modelno"),
                    ExpiryDate = ir.IsDBNull("expirydate") ? (DateTime?)null : ir.GetDateTime("expirydate"),
                    Quantity = ir.IsDBNull("quantity") ? 0 : ir.GetDecimal("quantity"),
                    Unit = ir.IsDBNull("unit") ? "" : ir.GetString("unit"),
                    PricePerUnit = ir.IsDBNull("price_per_unit") ? 0 : ir.GetDecimal("price_per_unit"),
                    DiscountPercentage = ir.IsDBNull("discount_percentage") ? 0 : ir.GetDecimal("discount_percentage"),
                    DiscountAmount = ir.IsDBNull("discount_amount") ? 0 : ir.GetDecimal("discount_amount"),
                    CreatedOn = ir.IsDBNull("created_on") ? DateTime.MinValue : ir.GetDateTime("created_on"),
                    TaxAmount = ir.IsDBNull("tax_amount") ? 0 : ir.GetDecimal("tax_amount"),
                    TaxPercentage = ir.IsDBNull("tax_percentage") ? 0 : ir.GetDecimal("tax_percentage"),
                    TotalAmount = ir.IsDBNull("total_amount") ? 0 : ir.GetDecimal("total_amount"),
                    ItemCode = ir.IsDBNull("itemcode") ? "" : ir.GetString("itemcode"),
                    AddCessAmount = ir.IsDBNull("addcessamount") ? 0 : ir.GetDecimal("addcessamount"),
                    Item = ir.IsDBNull("itemname") ? (ir.IsDBNull("item") ? "" : ir.GetString("item")) : ir.GetString("itemname"),
                    HSNCode = ir.IsDBNull("hsncode") ? "" : ir.GetString("hsncode"),

                    // Thermal receipts print these; they live on the item master,
                    // not on the document line.
                    Description = Str(ir, "itemdescription") ?? string.Empty,
                    Size = Str(ir, "itemsize") ?? string.Empty,
                    MRP = ir.IsDBNull("itemmrp") ? (decimal?)null : ir.GetDecimal("itemmrp"),
                    mfgdate = Str(ir, "mfgdate") ?? string.Empty,
                    ManufacturingDate = ParseDate(Str(ir, "mfgdate"))
                });
            }

            return bill;
        }

        // -----------------------------------------------------------------
        private PartyModel LoadParty(int partyId, int companyId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string query = @"
SELECT ps.*, ss.name AS state_name, ss.code AS state_code
FROM parties AS ps
LEFT JOIN states AS ss ON ss.id = ps.stateid
WHERE ps.id = @id AND (@companyid <= 0 OR ps.companyid = @companyid)";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", partyId);
                cmd.Parameters.AddWithValue("companyid", companyId);

                using var r = cmd.ExecuteReader();
                if (!r.Read()) return null;

                return new PartyModel
                {
                    Id = partyId,
                    PartyName = Str(r, "party_name"),
                    GSTIN = Str(r, "gstin"),
                    PhoneNumber = Str(r, "phone_number"),
                    GSTType = Str(r, "gst_type"),
                    State = Str(r, "state"),
                    Email = Str(r, "email"),
                    BillingAddress = Str(r, "billing_address"),
                    ShippingAddress = Str(r, "shipping_address"),
                    StateName = Str(r, "state_name"),
                    StateCode = Str(r, "state_code"),
                    StateId = Int(r, "stateid")
                };
            }
            catch
            {
                return null;
            }
        }

        // -----------------------------------------------------------------
        private decimal LoadPartyBalance(int partyId, int companyId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                const string query = @"
SELECT COALESCE(opening_balance, 0)
     + COALESCE((SELECT SUM(COALESCE(final_amount, 0) - COALESCE(paidreciveamount, 0))
                 FROM tradedocuments
                 WHERE partyid = @p_partyid
                   AND (@p_companyid <= 0 OR companyid = @p_companyid)), 0)
FROM parties
WHERE id = @p_partyid";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("p_partyid", partyId);
                cmd.Parameters.AddWithValue("p_companyid", companyId);

                var value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0m : Convert.ToDecimal(value);
            }
            catch
            {
                return 0m;
            }
        }

        // -----------------------------------------------------------------
        private static bool Has(System.Data.IDataRecord r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static string Str(System.Data.IDataRecord r, string name)
        {
            if (!Has(r, name)) return null;
            var v = r[name];
            return v == DBNull.Value ? null : v.ToString();
        }

        private static int Int(System.Data.IDataRecord r, string name)
        {
            if (!Has(r, name)) return 0;
            var v = r[name];
            return v == DBNull.Value ? 0 : Convert.ToInt32(v);
        }

        /// <summary>mfgdate is stored loosely, so parse defensively.</summary>
        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.TryParse(value, out var parsed) ? parsed : (DateTime?)null;
        }
    }
}
