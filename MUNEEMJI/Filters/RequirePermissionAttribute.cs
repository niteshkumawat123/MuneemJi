using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MUNEEMJI.Services;

namespace MUNEEMJI.Filters
{
    /// <summary>
    /// Action filter that checks if the current user's role has the required permission
    /// for the specified module. Usage: [RequirePermission("Sales Invoice", PermissionType.View)]
    /// </summary>
    public enum PermissionType
    {
        View = 1,
        Create = 2,
        Edit = 3,
        Share = 4,
        Delete = 5
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string moduleName, PermissionType permission = PermissionType.View)
            : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { moduleName, permission };
        }
    }

    public class RequirePermissionFilter : IAsyncActionFilter
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _moduleName;
        private readonly PermissionType _permission;

        public RequirePermissionFilter(
            IPermissionService permissionService,
            IHttpContextAccessor httpContextAccessor,
            string moduleName,
            PermissionType permission)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
            _moduleName = moduleName;
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var roleIdStr = _httpContextAccessor.HttpContext?.Session.GetString("RoleId");
            var isOwnerStr = _httpContextAccessor.HttpContext?.Session.GetString("IsOwner");
            bool isOwner = !string.IsNullOrEmpty(isOwnerStr) && bool.TryParse(isOwnerStr, out bool ownerVal) && ownerVal;
            int roleId = 0;
            bool hasRoleId = !string.IsNullOrEmpty(roleIdStr) && int.TryParse(roleIdStr, out roleId) && roleId > 0;

            // Only bypass for owners with Admin role (1) or no role
            if (isOwner && (!hasRoleId || roleId == 1))
            {
                await next();
                return;
            }

            // No valid role — allow (safety fallback)
            if (!hasRoleId)
            {
                await next();
                return;
            }

            var modulePerm = await _permissionService.GetModulePermissionAsync(roleId, _moduleName);

            bool hasPermission = _permission switch
            {
                PermissionType.View => modulePerm.CanView,
                PermissionType.Create => modulePerm.CanCreate,
                PermissionType.Edit => modulePerm.CanEdit,
                PermissionType.Share => modulePerm.CanShare,
                PermissionType.Delete => modulePerm.CanDelete,
                _ => false
            };

            if (!hasPermission)
            {
                // For AJAX requests return 403 JSON
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    context.HttpContext.Request.ContentType?.Contains("application/json") == true)
                {
                    context.Result = new JsonResult(new { success = false, message = "Access denied. You don't have permission for this action." })
                    {
                        StatusCode = 403
                    };
                    return;
                }

                // For regular requests redirect to access denied
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            await next();
        }
    }
}
