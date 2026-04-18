using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class CategoryRepository
    {
        private static string connString = MUNEEMJI.DbConfig.ConnectionString;

        public static List<Category> GetAll()
        {
            var categories = new List<Category>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Query categories
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return categories;
        }
    }
}
