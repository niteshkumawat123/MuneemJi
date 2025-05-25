using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class CategoryDataAccess
    {
        private static string ConnString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

        public static List<Category> GetAllWithCounts()
        {
            var list = new List<Category>();
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                SELECT c.id, c.name, COUNT(p.id) AS itemcount
                FROM categories c
                LEFT JOIN items p ON p.categoryid = c.id
                GROUP BY c.id, c.name
                ORDER BY c.name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ItemCount = reader.GetInt32(2)
                        });
                    }
                }
            }
            // Add special "Items not in any Category"
            int uncategorizedCount = GetUncategorizedCount();
            list.Insert(0, new Category { Id = 0, Name = "Items not in any Category", ItemCount = uncategorizedCount });
            return list;
        }

        private static int GetUncategorizedCount()
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM items WHERE categoryid IS NULL";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void Add(string name)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "INSERT INTO categories (name) VALUES (@Name)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
