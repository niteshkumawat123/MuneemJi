namespace MUNEEMJI.Models
{
    public class AddUserViewModel
    {
        public string FullName { get; set; }
        public string PhoneOrEmail { get; set; }
        public int SelectedRoleId { get; set; }
        public List<RoleOption> AvailableRoles { get; set; }
        public List<ModulePermissionViewModel> ModulePermissions { get; set; }
    }

    public class RoleOption
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class ModulePermissionViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanShare { get; set; }
        public bool CanDelete { get; set; }
        public string ViewText => CanView ? "✓" : "✗";
        public string CreateText => CanCreate ? "✓" : "NA";
        public string EditText => CanEdit ? "✓" : "NA";
        public string ShareText => CanShare ? "✓" : "NA";
        public string DeleteText => CanDelete ? "✓" : "NA";
    }
}
