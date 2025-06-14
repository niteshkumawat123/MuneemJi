using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Repositories
{
    public class ProductDataAccess
    {
        private static string ConnString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

        public static List<Product> GetAll()
        {
            var list = new List<Product>();
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                SELECT id, name, saleprice, purchaseprice, minstock, categoryid
                FROM items
                ORDER BY name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            SalePrice = reader.GetDecimal(2),
                            PurchasePrice = reader.GetDecimal(3),
                            Quantity = reader.GetInt32(4),
                            CategoryId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                        });
                    }
                }
            }
            return list;
        }

        public static Product GetById(int id)
        {
            Product p = null;
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                SELECT id, name, saleprice, purchaseprice, minstock, categoryid
                FROM items
                WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                SalePrice = reader.GetDecimal(2),
                                PurchasePrice = reader.GetDecimal(3),
                                Quantity = reader.GetInt32(4),
                                CategoryId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
            return p;
        }

        public static void Add(Product p)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                INSERT INTO products (name, saleprice, purchaseprice, quantity, categoryid)
                VALUES (@Name, @SalePrice, @PurchasePrice, @Quantity, @CategoryId)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", p.Name);
                    cmd.Parameters.AddWithValue("SalePrice", p.SalePrice);
                    cmd.Parameters.AddWithValue("PurchasePrice", p.PurchasePrice);
                    cmd.Parameters.AddWithValue("Quantity", p.Quantity);
                    if (p.CategoryId.HasValue)
                        cmd.Parameters.AddWithValue("CategoryId", p.CategoryId.Value);
                    else
                        cmd.Parameters.AddWithValue("CategoryId", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Update(Product p)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                UPDATE products
                SET name = @Name, saleprice = @SalePrice, purchaseprice = @PurchasePrice, quantity = @Quantity, categoryid = @CategoryId
                WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", p.Name);
                    cmd.Parameters.AddWithValue("SalePrice", p.SalePrice);
                    cmd.Parameters.AddWithValue("PurchasePrice", p.PurchasePrice);
                    cmd.Parameters.AddWithValue("Quantity", p.Quantity);
                    cmd.Parameters.AddWithValue("Id", p.Id);
                    if (p.CategoryId.HasValue)
                        cmd.Parameters.AddWithValue("CategoryId", p.CategoryId.Value);
                    else
                        cmd.Parameters.AddWithValue("CategoryId", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "DELETE FROM products WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateCategory(int productId, int? categoryId)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "UPDATE products SET categoryid = @CategoryId WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", productId);
                    if (categoryId.HasValue)
                        cmd.Parameters.AddWithValue("CategoryId", categoryId.Value);
                    else
                        cmd.Parameters.AddWithValue("CategoryId", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Product> GetByCategory(int? categoryId)
        {
            var list = new List<Product>();
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql;
                if (categoryId.HasValue)
                {
                    sql = @"
                    SELECT id, name, saleprice, purchaseprice, minstock, categoryid
                    FROM items
                    WHERE categoryid = @CategoryId
                    ORDER BY name";
                }
                else
                {
                    sql = @"
                    SELECT id, name, saleprice, purchaseprice, minstock, categoryid
                    FROM items
                    WHERE categoryid IS NULL
                    ORDER BY name";
                }
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (categoryId.HasValue)
                        cmd.Parameters.AddWithValue("CategoryId", categoryId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                SalePrice = reader.GetDecimal(2),
                                PurchasePrice = reader.GetDecimal(3),
                                Quantity = reader.GetInt32(4),
                                CategoryId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
