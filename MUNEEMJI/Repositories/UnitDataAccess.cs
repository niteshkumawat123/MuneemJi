using MUNEEMJI.Models;
using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Repositories
{
    public class UnitDataAccess
    {
        private static string ConnString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

        public static List<Unit> GetAll()
        {
            var list = new List<Unit>();
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "SELECT id,name as  fullname ,name as  shortname FROM units ORDER BY name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Unit
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            ShortName = reader.GetString(2)
                        });
                    }
                }
            }
            return list;
        }

        public static void AddUnit(Unit unit)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "INSERT INTO units (fullname, shortname) VALUES (@FullName, @ShortName)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("FullName", unit.FullName);
                    cmd.Parameters.AddWithValue("ShortName", unit.ShortName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteUnit(int id)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "DELETE FROM units WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<UnitConversion> GetConversions(int fromUnitId)
        {
            var list = new List<UnitConversion>();
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                SELECT uc.id, uc.fromunitid, uc.tounitid, uc.factor
                FROM unit_conversions uc
                WHERE uc.fromunitid = @FromId";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("FromId", fromUnitId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new UnitConversion
                            {
                                Id = reader.GetInt32(0),
                                FromUnitId = reader.GetInt32(1),
                                ToUnitId = reader.GetInt32(2),
                                Factor = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static void AddConversion(int fromUnitId, int toUnitId, decimal factor)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = @"
                INSERT INTO unit_conversions (fromunitid, tounitid, factor)
                VALUES (@FromId, @ToId, @Factor)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("FromId", fromUnitId);
                    cmd.Parameters.AddWithValue("ToId", toUnitId);
                    cmd.Parameters.AddWithValue("Factor", factor);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteConversion(int id)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "DELETE FROM unit_conversions WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
