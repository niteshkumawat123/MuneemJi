using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MUNEEMJI.Services;

namespace MUNEEMJI.Filters
{
    /// <summary>
    /// Global filter that automatically enforces role-based permissions on ALL controllers.
    /// Maps controller names to module names and action names to permission types.
    /// Owners bypass all checks.
    /// </summary>
    public class GlobalPermissionFilter : IAsyncActionFilter
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Map controller names (lowercase) to module names in rolepermissions table
        private static readonly Dictionary<string, string> ControllerModuleMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Sales", "Sales Invoice" },
            { "Estimate_Quotations", "Estimation/Quotation" },
            { "SalesOrder", "Sale Order" },
            { "DeliveryChallan", "Delivery Challan" },
            { "CreditNote", "Credit Note" },
            { "PaymentIn", "Payment In" },
            { "PurchaseBill", "Purchase Bill" },
            { "PurchaseOrder", "Purchase Order" },
            { "PaymentOut", "Payment Out" },
            { "Expense", "Expense" },
            { "OtherIncome", "Other Income" },
            { "Party", "Parties" },
            { "Items", "Items" },
            { "BillItem", "Items" },
            { "Godown", "Items" },
            { "Report", "Reports" },
            { "DebitNote", "Purchase Bill" },
            { "General", "Settings" },
            { "GstSettings", "Settings" },
            { "ItemSettings", "Settings" },
            { "Transaction", "Settings" },
            { "TransactionSettings", "Settings" },
            { "Print", "Settings" },
        };

        // Action names that require Create permission
        private static readonly HashSet<string> CreateActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Create", "Add", "AddUser", "AddExpense", "AddBillItem"
        };

        // Action names that require Edit permission
        private static readonly HashSet<string> EditActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Edit", "Update", "UpdateEntries", "GetById"
        };

        // Action names that require Delete permission
        private static readonly HashSet<string> DeleteActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Delete", "DeleteConfirmed", "Remove", "RemoveBillItem"
        };

        // Actions that should be excluded from permission check
        private static readonly HashSet<string> ExcludedActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "CalculateItemAmount", "GetPartyDetailsById", "GetPartyDropDownAsync",
            "GetRolePermissions", "GetPartyGroups", "DownloadInvoicePdf", "DownloadPdf"
        };

        // Controllers excluded from permission checks entirely
        private static readonly HashSet<string> ExcludedControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Account", "Home", "Company", "PdfDownload", "PdfTest",
            "State", "Units", "User", "Bank", "BusinessProfile",
            "Cashadjustment", "PlansAndPricing", "WhatsAppMarketing",
            "Loan", "Services", "test"
        };

        public GlobalPermissionFilter(IPermissionService permissionService, IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                await next();
                return;
            }

            // Owner with Admin role always has full access
            var isOwnerStr = httpContext.Session.GetString("IsOwner");
            var roleIdStr = httpContext.Session.GetString("RoleId");
            bool isOwner = !string.IsNullOrEmpty(isOwnerStr) && bool.TryParse(isOwnerStr, out bool ownerVal) && ownerVal;
            int roleId = 0;
            bool hasRoleId = !string.IsNullOrEmpty(roleIdStr) && int.TryParse(roleIdStr, out roleId) && roleId > 0;

            // Only bypass for actual owners with Admin role (roleid=1)
            if (isOwner && (!hasRoleId || roleId == 1))
            {
                await next();
                return;
            }

            // If no valid RoleId, allow (safety fallback)
            if (!hasRoleId)
            {
                await next();
                return;
            }

            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "";
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "";

            // Skip excluded controllers
            if (ExcludedControllers.Contains(controllerName))
            {
                await next();
                return;
            }

            // Skip excluded actions
            if (ExcludedActions.Contains(actionName))
            {
                await next();
                return;
            }

            // Find module name for this controller
            if (!ControllerModuleMap.TryGetValue(controllerName, out string moduleName))
            {
                // Controller not mapped — allow (settings area controllers, etc.)
                await next();
                return;
            }

            // Determine required permission type based on action name
            PermissionType requiredPermission;
            if (CreateActions.Contains(actionName))
            {
                // For POST Create, check create permission; for GET Create, also check create
                requiredPermission = PermissionType.Create;
            }
            else if (EditActions.Contains(actionName))
            {
                // GetById with typeid param: check if it's view or edit
                if (actionName.Equals("GetById", StringComparison.OrdinalIgnoreCase))
                {
                    var typeidStr = httpContext.Request.Query["typeid"].FirstOrDefault();
                    if (int.TryParse(typeidStr, out int typeid) && typeid == 2) // Edit type
                        requiredPermission = PermissionType.Edit;
                    else
                        requiredPermission = PermissionType.View;
                }
                else
                {
                    requiredPermission = PermissionType.Edit;
                }
            }
            else if (DeleteActions.Contains(actionName))
            {
                requiredPermission = PermissionType.Delete;
            }
            else
            {
                // Default: View permission (Index, Details, etc.)
                requiredPermission = PermissionType.View;
            }

            var modulePerm = await _permissionService.GetModulePermissionAsync(roleId, moduleName);

            bool hasPermission = requiredPermission switch
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
                // AJAX / JSON requests
                if (httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    httpContext.Request.ContentType?.Contains("application/json") == true ||
                    httpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        message = "Access denied. You don't have permission for this action."
                    })
                    {
                        StatusCode = 403
                    };
                    return;
                }

                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            await next();
        }
    }
}
