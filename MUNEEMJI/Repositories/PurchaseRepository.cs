using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Repositories
{
    public class PurchaseRepository
    {
        private readonly IConfiguration _config;

        public PurchaseRepository(IConfiguration config)
        {
            _config = config;
        }

        public void InsertPurchase(Purchase purchase)
        {
            using var con = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();

            using var tran = con.BeginTransaction();

            var cmd = new NpgsqlCommand(@"
            INSERT INTO purchase (invoicenumber, invoicedate, ponumber, podate, ewaybillnumber,
                stateofsupplyid, paymenttype, transportname, deliverylocation, vehiclenumber, deliverydate,
                roundoff, roundoffamount, total, grandtotal)
            VALUES (@invoicenumber, @invoicedate, @ponumber, @podate, @ewaybillnumber, @stateofsupplyid, 
                @paymenttype, @transportname, @deliverylocation, @vehiclenumber, @deliverydate, 
                @roundoff, @roundoffamount, @total, @grandtotal)
            RETURNING purchaseid", con);

            cmd.Parameters.AddWithValue("@invoicenumber", purchase.InvoiceNumber ?? "");
            cmd.Parameters.AddWithValue("@invoicedate", purchase.InvoiceDate);
            cmd.Parameters.AddWithValue("@ponumber", purchase.PONumber ?? "");
            cmd.Parameters.AddWithValue("@podate", (object?)purchase.PODate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ewaybillnumber", purchase.EWayBillNumber ?? "");
            cmd.Parameters.AddWithValue("@stateofsupplyid", (object?)purchase.StateOfSupplyId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@paymenttype", purchase.PaymentType ?? "");
            cmd.Parameters.AddWithValue("@transportname", purchase.TransportName ?? "");
            cmd.Parameters.AddWithValue("@deliverylocation", purchase.DeliveryLocation ?? "");
            cmd.Parameters.AddWithValue("@vehiclenumber", purchase.VehicleNumber ?? "");
            cmd.Parameters.AddWithValue("@deliverydate", (object?)purchase.DeliveryDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roundoff", purchase.RoundOff);
            cmd.Parameters.AddWithValue("@roundoffamount", purchase.RoundOffAmount);
            cmd.Parameters.AddWithValue("@total", purchase.Total);
            cmd.Parameters.AddWithValue("@grandtotal", purchase.GrandTotal);

            var purchaseId = (int)cmd.ExecuteScalar();

            foreach (var item in purchase.Items)
            {
                var itemCmd = new NpgsqlCommand(@"
                INSERT INTO purchaseitem (purchaseid, itemid, quantity, unitid, priceperunit, discountpercent,
                    discountamount, taxid, taxpercent, taxamount, amount)
                VALUES (@purchaseid, @itemid, @quantity, @unitid, @priceperunit, @discountpercent,
                    @discountamount, @taxid, @taxpercent, @taxamount, @amount)", con);

                itemCmd.Parameters.AddWithValue("@purchaseid", purchaseId);
                itemCmd.Parameters.AddWithValue("@itemid", item.ItemId);
                itemCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                itemCmd.Parameters.AddWithValue("@unitid", item.UnitId);
                itemCmd.Parameters.AddWithValue("@priceperunit", item.PricePerUnit);
                itemCmd.Parameters.AddWithValue("@discountpercent", item.DiscountPercent);
                itemCmd.Parameters.AddWithValue("@discountamount", item.DiscountAmount);
                itemCmd.Parameters.AddWithValue("@taxid", item.TaxId);
                itemCmd.Parameters.AddWithValue("@taxpercent", item.TaxPercent);
                itemCmd.Parameters.AddWithValue("@taxamount", item.TaxAmount);
                itemCmd.Parameters.AddWithValue("@amount", item.Amount);

                itemCmd.ExecuteNonQuery();
            }

            tran.Commit();
        }

    }
}
