using Npgsql;

namespace MUNEEMJI.Services
{
    public class DropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public interface IDropdownService
    {
        Task<List<DropdownItem>> GetGodownsAsync(int companyId);
        Task<List<DropdownItem>> GetUsersAsync(int companyId);
        Task<List<DropdownItem>> GetCategoriesAsync();
        Task<List<DropdownItem>> GetStatesAsync();
        Task<List<DropdownItem>> GetBankAccountsAsync(int companyId);
        Task<List<DropdownItem>> GetUnitsAsync();
    }

    public class DropdownService : IDropdownService
    {
        private readonly string _connectionString;

        public DropdownService()
        {
            _connectionString = DbConfig.ConnectionString;
        }

        public async Task<List<DropdownItem>> GetGodownsAsync(int companyId)
        {
            var list = new List<DropdownItem>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var sql = "SELECT id, godownname AS name FROM godowns WHERE companyid = @cid ORDER BY godownname";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("cid", companyId);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    list.Add(new DropdownItem { Id = r.GetInt32(0), Name = r.GetString(1) });
            }
            catch { }
            return list;
        }

        public async Task<List<DropdownItem>> GetUsersAsync(int companyId)
        {
            var list = new List<DropdownItem>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var sql = "SELECT id, COALESCE(username, business_name) AS name FROM businesses WHERE companyid = @cid ORDER BY COALESCE(username, business_name)";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("cid", companyId);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    list.Add(new DropdownItem { Id = r.GetInt32(0), Name = r.IsDBNull(1) ? "" : r.GetString(1) });
            }
            catch { }
            return list;
        }

        public async Task<List<DropdownItem>> GetCategoriesAsync()
        {
            var list = new List<DropdownItem>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var sql = "SELECT id, name FROM categorieses ORDER BY name";
                using var cmd = new NpgsqlCommand(sql, conn);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    list.Add(new DropdownItem { Id = r.GetInt32(0), Name = r.GetString(1) });
            }
            catch { }
            return list;
        }

        public async Task<List<DropdownItem>> GetStatesAsync()
        {
            var list = new List<DropdownItem>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var sql = "SELECT id, name FROM states ORDER BY name";
                using var cmd = new NpgsqlCommand(sql, conn);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    list.Add(new DropdownItem { Id = r.GetInt32(0), Name = r.GetString(1) });
            }
            catch { }
            return list;
        }

        public async Task<List<DropdownItem>> GetBankAccountsAsync(int companyId)
        {
            var list = new List<DropdownItem>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var sql = "SELECT id, account_display_name AS name FROM extended_bank_accounts ORDER BY account_display_name";
                using var cmd = new NpgsqlCommand(sql, conn);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    list.Add(new DropdownItem { Id = r.GetInt32(0), Name = r.IsDBNull(1) ? "" : r.GetString(1) });
            }
            catch { }
            return list;
        }

        public async Task<List<DropdownItem>> GetUnitsAsync()
        {
            var list = new List<DropdownItem>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var sql = "SELECT id, name FROM units ORDER BY name";
                using var cmd = new NpgsqlCommand(sql, conn);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    list.Add(new DropdownItem { Id = r.GetInt32(0), Name = r.GetString(1) });
            }
            catch { }
            return list;
        }
    }
}
