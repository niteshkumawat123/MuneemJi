using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Services;

namespace MUNEEMJI.ViewComponents
{
    /// <summary>
    /// Loads permissions for the current user and passes them to the view.
    /// Usage in views: @await Component.InvokeAsync("UserPermission")
    /// Then access: ViewBag.Permissions (UserPermissions object)
    /// </summary>
    public class UserPermissionViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;

        public UserPermissionViewComponent(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roleIdStr = HttpContext.Session.GetString("RoleId");
            var isOwnerStr = HttpContext.Session.GetString("IsOwner");
            bool isOwner = !string.IsNullOrEmpty(isOwnerStr) && bool.TryParse(isOwnerStr, out bool o) && o;
            int roleId = 0;
            bool hasRoleId = !string.IsNullOrEmpty(roleIdStr) && int.TryParse(roleIdStr, out roleId) && roleId > 0;

            UserPermissions permissions;

            // Only give full access to owners with Admin role (1) or no role set
            if (isOwner && (!hasRoleId || roleId == 1))
            {
                permissions = new UserPermissions { RoleId = 0 };
            }
            else if (hasRoleId)
            {
                permissions = await _permissionService.GetUserPermissionsAsync(roleId);
            }
            else
            {
                permissions = new UserPermissions { RoleId = 0 };
            }

            return View(permissions);
        }
    }
}
