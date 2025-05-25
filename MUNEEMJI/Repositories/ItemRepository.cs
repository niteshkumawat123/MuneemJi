using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class ItemRepository
    {
        private static string connString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

        public static int InsertItem(Item item)
        {
            int newId;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert item and return generated id
                string sql = @"
                    INSERT INTO items (name, hsn, categoryid, itemcode, unitid, imagepath, 
                                       saleprice, discount, purchaseprice, taxid, 
                                       openingquantity, atprice, minstock, location, entrydate)
                    VALUES ($1, $2, $3, $4, $5, $6,
                            $7, $8, $9, $10,
                            $11, $12, $13, $14, $15)
                    RETURNING id;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(item.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue(item.HSN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue(item.CategoryId);
                    cmd.Parameters.AddWithValue(item.ItemCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue(item.UnitId);
                    cmd.Parameters.AddWithValue(item.ImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue(item.SalePrice);
                    cmd.Parameters.AddWithValue(item.Discount);
                    cmd.Parameters.AddWithValue(item.PurchasePrice);
                    cmd.Parameters.AddWithValue(item.TaxId);
                    cmd.Parameters.AddWithValue(item.OpeningQuantity);
                    cmd.Parameters.AddWithValue(item.AtPrice);
                    cmd.Parameters.AddWithValue(item.MinStock);
                    cmd.Parameters.AddWithValue(item.Location ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue(item.EntryDate);

                    newId = (int)cmd.ExecuteScalar();
                }
            }
            return newId;
        }

        public static void AddWholesalePrice(int itemId, decimal price, int minQty)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO itemwholesaleprices (itemid, wholesaleprice, minquantity)
                    VALUES ($1, $2, $3);";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue( itemId);
                    cmd.Parameters.AddWithValue( price);
                    cmd.Parameters.AddWithValue( minQty);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
