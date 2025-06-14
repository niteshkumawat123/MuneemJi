using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class CategoryRepository
    {
        private static string connString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

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
