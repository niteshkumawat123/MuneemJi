using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class CategoryRepository
    {
        private static string connString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

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
