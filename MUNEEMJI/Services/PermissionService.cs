using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace MUNEEMJI.Services
{
    public interface IPermissionService
    {
        Task<UserPermissions> GetUserPermissionsAsync(int roleId);
        Task<ModulePermission> GetModulePermissionAsync(int roleId, string moduleName);
        void ClearCache(int roleId);
    }

    public class ModulePermission
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = "";
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanShare { get; set; }
        public bool CanDelete { get; set; }
    }

    public class UserPermissions
    {
        public int RoleId { get; set; }
        public List<ModulePermission> Modules { get; set; } = new();

        public ModulePermission GetModule(string moduleName)
        {
            return Modules.FirstOrDefault(m =>
                string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
                ?? new ModulePermission { ModuleName = moduleName };
        }

        public bool HasView(string moduleName) => GetModule(moduleName).CanView;
        public bool HasCreate(string moduleName) => GetModule(moduleName).CanCreate;
        public bool HasEdit(string moduleName) => GetModule(moduleName).CanEdit;
        public bool HasDelete(string moduleName) => GetModule(moduleName).CanDelete;
        public bool HasShare(string moduleName) => GetModule(moduleName).CanShare;
    }

    public class PermissionService : IPermissionService
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;

        public PermissionService(IMemoryCache cache)
        {
            _connectionString = DbConfig.ConnectionString;
            _cache = cache;
        }

        public async Task<UserPermissions> GetUserPermissionsAsync(int roleId)
        {
            string cacheKey = $"permissions_role_{roleId}";

            if (_cache.TryGetValue(cacheKey, out object cachedObj) && cachedObj is UserPermissions cached)
                return cached;

            var permissions = new UserPermissions { RoleId = roleId };

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT moduleid, modulename,
                    BOOL_OR(CASE WHEN permissionid = 1 THEN allowed ELSE false END) AS canview,
                    BOOL_OR(CASE WHEN permissionid = 2 THEN allowed ELSE false END) AS cancreate,
                    BOOL_OR(CASE WHEN permissionid = 3 THEN allowed ELSE false END) AS canedit,
                    BOOL_OR(CASE WHEN permissionid = 4 THEN allowed ELSE false END) AS canshare,
                    BOOL_OR(CASE WHEN permissionid = 5 THEN allowed ELSE false END) AS candelete
                FROM public.rolepermissions
                WHERE roleid = @roleId
                GROUP BY moduleid, modulename
                ORDER BY moduleid";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("roleId", roleId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Modules.Add(new ModulePermission
                {
                    ModuleId = reader.GetInt32(reader.GetOrdinal("moduleid")),
                    ModuleName = reader.GetString(reader.GetOrdinal("modulename")),
                    CanView = reader.GetBoolean(reader.GetOrdinal("canview")),
                    CanCreate = reader.GetBoolean(reader.GetOrdinal("cancreate")),
                    CanEdit = reader.GetBoolean(reader.GetOrdinal("canedit")),
                    CanShare = reader.GetBoolean(reader.GetOrdinal("canshare")),
                    CanDelete = reader.GetBoolean(reader.GetOrdinal("candelete"))
                });
            }

            _cache.Set(cacheKey, (object)permissions, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            return permissions;
        }

        public async Task<ModulePermission> GetModulePermissionAsync(int roleId, string moduleName)
        {
            var all = await GetUserPermissionsAsync(roleId);
            return all.GetModule(moduleName);
        }

        public void ClearCache(int roleId)
        {
            _cache.Remove($"permissions_role_{roleId}");
        }
    }
}
