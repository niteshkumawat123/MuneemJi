namespace MUNEEMJI.Models
{
    public class RolePermission
    {
       
        public int? RoleId { get; set; }

        public int? ModuleId { get; set; }

        public int? PermissionId { get; set; }

        public bool Allowed { get; set; }
    }
}
