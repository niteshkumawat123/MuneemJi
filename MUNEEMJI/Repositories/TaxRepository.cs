using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class TaxRepository
    {
        private static string connString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

        public static List<Tax> GetAll()
        {
            var taxes = new List<Tax>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name, rate FROM taxes", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        taxes.Add(new Tax
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Rate = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return taxes;
        }
    }
}
