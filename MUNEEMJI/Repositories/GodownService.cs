using MUNEEMJI.Models;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Repositories
{
    public interface IGodownService
    {
        Task<List<Godown>> GetAllGodownsAsync();
        Task<Godown> GetGodownByIdAsync(int id);
        Task<bool> CreateGodownAsync(Godown godown);
        Task<bool> UpdateGodownAsync(Godown godown);
        Task<bool> DeleteGodownAsync(int id);
        Task<int> GetGodownCountAsync();
    }

    public class GodownService : IGodownService
    {
        private readonly string _connectionString;

        public GodownService(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        public async Task<List<Godown>> GetAllGodownsAsync()
        {
            var godowns = new List<Godown>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"SELECT Id, GodownName, GodownType, PhoneNo, EmailId, GSTIN, 
                           GodownAddress, GodownPincode, CreatedDate, UpdatedDate 
                           FROM Godowns ORDER BY CreatedDate DESC";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                godowns.Add(new Godown
                {
                    Id = reader.GetInt32("Id"),
                    GodownName = reader.GetString("GodownName"),
                    GodownType = reader.GetString("GodownType"),
                    PhoneNo = reader.GetString("PhoneNo"),
                    EmailId = reader.IsDBNull("EmailId") ? null : reader.GetString("EmailId"),
                    GSTIN = reader.IsDBNull("GSTIN") ? null : reader.GetString("GSTIN"),
                    GodownAddress = reader.IsDBNull("GodownAddress") ? null : reader.GetString("GodownAddress"),
                    GodownPincode = reader.IsDBNull("GodownPincode") ? null : reader.GetString("GodownPincode"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    UpdatedDate = reader.GetDateTime("UpdatedDate")
                });
            }

            return godowns;
        }

        public async Task<Godown> GetGodownByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"SELECT Id, GodownName, GodownType, PhoneNo, EmailId, GSTIN, 
                           GodownAddress, GodownPincode, CreatedDate, UpdatedDate 
                           FROM Godowns WHERE Id = @Id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Godown
                {
                    Id = reader.GetInt32("Id"),
                    GodownName = reader.GetString("GodownName"),
                    GodownType = reader.GetString("GodownType"),
                    PhoneNo = reader.GetString("PhoneNo"),
                    EmailId = reader.IsDBNull("EmailId") ? null : reader.GetString("EmailId"),
                    GSTIN = reader.IsDBNull("GSTIN") ? null : reader.GetString("GSTIN"),
                    GodownAddress = reader.IsDBNull("GodownAddress") ? null : reader.GetString("GodownAddress"),
                    GodownPincode = reader.IsDBNull("GodownPincode") ? null : reader.GetString("GodownPincode"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    UpdatedDate = reader.GetDateTime("UpdatedDate")
                };
            }

            return null;
        }

        public async Task<bool> CreateGodownAsync(Godown godown)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"INSERT INTO Godowns (GodownName, GodownType, PhoneNo, EmailId, GSTIN, 
                           GodownAddress, GodownPincode, CreatedDate, UpdatedDate) 
                           VALUES (@GodownName, @GodownType, @PhoneNo, @EmailId, @GSTIN, 
                           @GodownAddress, @GodownPincode, @CreatedDate, @UpdatedDate)";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@GodownName", godown.GodownName);
            command.Parameters.AddWithValue("@GodownType", godown.GodownType);
            command.Parameters.AddWithValue("@PhoneNo", godown.PhoneNo);
            command.Parameters.AddWithValue("@EmailId", (object)godown.EmailId ?? DBNull.Value);
            command.Parameters.AddWithValue("@GSTIN", (object)godown.GSTIN ?? DBNull.Value);
            command.Parameters.AddWithValue("@GodownAddress", (object)godown.GodownAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@GodownPincode", (object)godown.GodownPincode ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> UpdateGodownAsync(Godown godown)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"UPDATE Godowns SET GodownName = @GodownName, GodownType = @GodownType, 
                           PhoneNo = @PhoneNo, EmailId = @EmailId, GSTIN = @GSTIN, 
                           GodownAddress = @GodownAddress, GodownPincode = @GodownPincode, 
                           UpdatedDate = @UpdatedDate WHERE Id = @Id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", godown.Id);
            command.Parameters.AddWithValue("@GodownName", godown.GodownName);
            command.Parameters.AddWithValue("@GodownType", godown.GodownType);
            command.Parameters.AddWithValue("@PhoneNo", godown.PhoneNo);
            command.Parameters.AddWithValue("@EmailId", (object)godown.EmailId ?? DBNull.Value);
            command.Parameters.AddWithValue("@GSTIN", (object)godown.GSTIN ?? DBNull.Value);
            command.Parameters.AddWithValue("@GodownAddress", (object)godown.GodownAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@GodownPincode", (object)godown.GodownPincode ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> DeleteGodownAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "DELETE FROM Godowns WHERE Id = @Id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<int> GetGodownCountAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT COUNT(*) FROM Godowns";

            using var command = new NpgsqlCommand(query, connection);
            object result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
    }
}
