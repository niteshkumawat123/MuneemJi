using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Repositories
{
    public class ServiceDataAccess
    {
        private static string ConnString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

        public static List<Service> GetAll()
        {
            var list = new List<Service>();
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "SELECT id, name, saleprice FROM items ORDER BY name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Service
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return list;
        }

        public static Service GetById(int id)
        {
            Service s = null;
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "SELECT id, name, price FROM services WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            s = new Service
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2)
                            };
                        }
                    }
                }
            }
            return s;
        }

        public static void Add(Service s)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "INSERT INTO services (name, price) VALUES (@Name, @Price)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", s.Name);
                    cmd.Parameters.AddWithValue("Price", s.Price);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Update(Service s)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "UPDATE services SET name = @Name, price = @Price WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", s.Name);
                    cmd.Parameters.AddWithValue("Price", s.Price);
                    cmd.Parameters.AddWithValue("Id", s.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sql = "DELETE FROM services WHERE id = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
