using Npgsql;
using System;
using System.Threading.Tasks;

namespace MUNEEMJI.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly string _connectionString;

        public ErrorLogService()
        {
            _connectionString = DbConfig.ConnectionString;
        }

        public async Task LogErrorAsync(string errorMessage, string stackTrace)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"INSERT INTO public.error_logs (error_message, stack_trace, created_at) 
                              VALUES (@error_message, @stack_trace, @created_at)";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@error_message", errorMessage ?? string.Empty);
                command.Parameters.AddWithValue("@stack_trace", stackTrace ?? string.Empty);
                command.Parameters.AddWithValue("@created_at", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                // Silently fail - we don't want error logging to break the application
            }
        }
    }
}
