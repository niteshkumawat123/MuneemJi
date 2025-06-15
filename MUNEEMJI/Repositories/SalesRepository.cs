using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Repositories
{
    public class SalesRepository
    {
        private static string _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
       

        // Load dropdown data
        public static List<ItemDropdownModel> GetItems()
        {
            var list = new List<ItemDropdownModel>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT id as ItemId, Name FROM items", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(new ItemDropdownModel { ItemId = reader.GetInt32(0), Name = reader.GetString(1) });
            return list;
        }
        public static List<UnitModel> GetUnits()
        {
            var list = new List<UnitModel>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT id as UnitId, Name FROM Units", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(new UnitModel { UnitId = reader.GetInt32(0), Name = reader.GetString(1) });
            return list;
        }
        public static List<TaxModel> GetTaxes()
        {
            var list = new List<TaxModel>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT id as TaxId, Name, Rate FROM taxes", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(new TaxModel { TaxId = reader.GetInt32(0), Name = reader.GetString(1), Rate = reader.GetDecimal(2) });
            return list;
        }

        // Create a new sale
        public static void CreateSale(Sale sale)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            // Insert into Sales and get the generated SaleId
            string insertSaleSql = @"
            INSERT INTO Sales (InvoiceNumber, InvoiceDate, IsCash, StateOfSupplyId, BillingName, PhoneNumber, BillingAddress, ShippingAddress, Total, RoundOff, GrandTotal)
            VALUES (@InvoiceNumber, @InvoiceDate, @IsCash, @StateOfSupplyId, @BillingName, @PhoneNumber, @BillingAddress, @ShippingAddress, @Total, @RoundOff, @GrandTotal)
            RETURNING SaleId";
            using (var cmd = new NpgsqlCommand(insertSaleSql, conn))
            {
                cmd.Parameters.AddWithValue("InvoiceNumber", sale.InvoiceNumber);
                cmd.Parameters.AddWithValue("InvoiceDate", sale.InvoiceDate);
                cmd.Parameters.AddWithValue("IsCash", sale.IsCash);
                cmd.Parameters.AddWithValue("StateOfSupplyId", (object)sale.StateOfSupplyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("BillingName", sale.BillingName ?? "");
                cmd.Parameters.AddWithValue("PhoneNumber", sale.PhoneNumber);
                cmd.Parameters.AddWithValue("BillingAddress", sale.BillingAddress);
                cmd.Parameters.AddWithValue("ShippingAddress", sale.ShippingAddress);
                cmd.Parameters.AddWithValue("Total", sale.Total);
                cmd.Parameters.AddWithValue("RoundOff", sale.RoundOff);
                cmd.Parameters.AddWithValue("GrandTotal", sale.GrandTotal);
                sale.SaleId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            // Insert each SaleItem
            string insertItemSql = @"
            INSERT INTO SaleItems (SaleId, ItemId, UnitId, Quantity, PricePerUnit, DiscountPercent, DiscountAmount, TaxId, TaxPercent, TaxAmount, Amount)
            VALUES (@SaleId, @ItemId, @UnitId, @Quantity, @PricePerUnit, @DiscountPercent, @DiscountAmount, @TaxId, @TaxPercent, @TaxAmount, @Amount)";
            foreach (var item in sale.Items)
            {
                using var cmd = new NpgsqlCommand(insertItemSql, conn);
                cmd.Parameters.AddWithValue("SaleId", sale.SaleId);
                cmd.Parameters.AddWithValue("ItemId", item.ItemId);
                cmd.Parameters.AddWithValue("UnitId", item.UnitId);
                cmd.Parameters.AddWithValue("Quantity", item.Quantity);
                cmd.Parameters.AddWithValue("PricePerUnit", item.PricePerUnit);
                cmd.Parameters.AddWithValue("DiscountPercent", item.DiscountPercent);
                cmd.Parameters.AddWithValue("DiscountAmount", item.DiscountAmount);
                cmd.Parameters.AddWithValue("TaxId", item.TaxId.HasValue ? (object)item.TaxId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("TaxPercent", item.TaxPercent);
                cmd.Parameters.AddWithValue("TaxAmount", item.TaxAmount);
                cmd.Parameters.AddWithValue("Amount", item.Amount);
                cmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        // Retrieve a sale and its items
        public static Sale GetSale(int saleId)
        {
            var sale = new Sale();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT SaleId, InvoiceNumber, InvoiceDate, IsCash, StateOfSupplyId, BillingName, PhoneNumber, BillingAddress, ShippingAddress, Total, RoundOff, GrandTotal FROM Sales WHERE SaleId=@SaleId", conn))
            {
                cmd.Parameters.AddWithValue("SaleId", saleId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    sale.SaleId = reader.GetInt32(0);
                    sale.InvoiceNumber = reader.GetString(1);
                    sale.InvoiceDate = reader.GetDateTime(2);
                    sale.IsCash = reader.GetBoolean(3);
                    sale.StateOfSupplyId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4);
                    sale.BillingName = reader.GetString(5);
                    sale.PhoneNumber = reader.GetString(6);
                    sale.BillingAddress = reader.GetString(7);
                    sale.ShippingAddress = reader.GetString(8);
                    sale.Total = reader.GetDecimal(9);
                    sale.RoundOff = reader.GetBoolean(10);
                    sale.GrandTotal = reader.GetDecimal(11);
                }
            }
            // Load items
            using (var cmd = new NpgsqlCommand("SELECT SaleItemId, ItemId, UnitId, Quantity, PricePerUnit, DiscountPercent, DiscountAmount, TaxId, TaxPercent, TaxAmount, Amount FROM SaleItems WHERE SaleId=@SaleId", conn))
            {
                cmd.Parameters.AddWithValue("SaleId", saleId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new SaleItem
                    {
                        SaleItemId = reader.GetInt32(0),
                        SaleId = saleId,
                        ItemId = reader.GetInt32(1),
                        UnitId = reader.GetInt32(2),
                        Quantity = reader.GetDecimal(3),
                        PricePerUnit = reader.GetDecimal(4),
                        DiscountPercent = reader.GetDecimal(5),
                        DiscountAmount = reader.GetDecimal(6),
                        TaxId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                        TaxPercent = reader.GetDecimal(8),
                        TaxAmount = reader.GetDecimal(9),
                        Amount = reader.GetDecimal(10)
                    };
                    sale.Items.Add(item);
                }
            }
            return sale;
        }
    }
}
