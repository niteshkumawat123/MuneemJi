using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class UnitRepository
    {
        private static string connString = MUNEEMJI.DbConfig.ConnectionString;

        public static List<Unit> GetAll()
        {
            var units = new List<Unit>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM units", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        units.Add(new Unit
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return units;
        }
    }
}
