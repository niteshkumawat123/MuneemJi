using MUNEEMJI.Services;
using Npgsql;

namespace MUNEEMJI.Repositories
{
    public interface IUser
    {
        Task<List<MUNEEMJI.Models.Business>> GetUserDropdown(int CompanyId);
    }

    public class UserRepository: IUser
    {
        private readonly string _connectionString;
        public UserRepository() 
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        }
        public async Task<List<MUNEEMJI.Models.Business>> GetUserDropdown(int CompanyId)
        {
            var users = new List<MUNEEMJI.Models.Business>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id, business_name, phone, email, status, roleid, created_at, updated_at ,username
                FROM businesses where companyid = @p_companyid
                ORDER BY created_at DESC";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("p_companyid", CompanyId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new MUNEEMJI.Models.Business
                {
                    Id = reader["id"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("id")) : 0,
                    BusinessName = reader["business_name"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("business_name")) : null,
                    Phone = reader["phone"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("phone")) : null,
                    Email = reader["email"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("email")) : null,
                    Status = reader["status"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("status")) : 0,
                    RoleId = reader["roleid"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("roleid")) : 0,
                    CreatedAt = reader["created_at"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("created_at")) : DateTime.MinValue,
                    UpdatedAt = reader["updated_at"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("updated_at")) : DateTime.MinValue,
                    Username = reader["username"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("username")) : reader["business_name"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("business_name")) : null
                });
            }

            return users;
        }

    }
}
