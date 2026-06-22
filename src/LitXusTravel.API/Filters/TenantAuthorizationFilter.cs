using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LitXusTravel.API.Filters;

/// <summary>
/// Validates that the tenantId route parameter matches the JWT tenantId claim.
/// Admin/SuperAdmin bypass — they can access any tenant's data.
/// Apply to any action that has a {tenantId:guid} route parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TenantAuthorizationFilter : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true) return;

        // Admins and SuperAdmins are exempt
        if (user.IsInRole("Admin") || user.IsInRole("SuperAdmin")) return;

        // Extract tenantId from route
        if (!context.RouteData.Values.TryGetValue("tenantId", out var routeTenantIdObj)
            || !Guid.TryParse(routeTenantIdObj?.ToString(), out var routeTenantId))
            return;

        // Extract tenantId from JWT claim
        var jwtTenantId = user.FindFirstValue("tenantId");
        if (string.IsNullOrEmpty(jwtTenantId) || !Guid.TryParse(jwtTenantId, out var claimTenantId))
        {
            context.Result = new ForbidResult();
            return;
        }

        if (routeTenantId != claimTenantId)
            context.Result = new ForbidResult();
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
