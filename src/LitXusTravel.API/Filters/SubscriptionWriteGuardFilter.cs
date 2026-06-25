using LitXusTravel.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LitXusTravel.API.Filters;

/// <summary>
/// Blocks all non-GET requests when the tenant's subscription is in read-only mode
/// (expired beyond the 7-day grace period). Returns HTTP 402 Payment Required.
/// Apply at the class level to all tenant-scoped write controllers.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SubscriptionWriteGuardAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (HttpMethods.IsGet(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        if (!context.RouteData.Values.TryGetValue("tenantId", out var raw)
            || !Guid.TryParse(raw?.ToString(), out var tenantId))
        {
            await next();
            return;
        }

        var uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var subs = await uow.TenantSubscriptions.FindAsync(s => s.TenantId == tenantId, default);
        var sub = subs
            .OrderByDescending(s => s.IsActive)
            .ThenByDescending(s => s.StartDate)
            .FirstOrDefault();

        if (sub?.IsReadOnly == true)
        {
            context.Result = new ObjectResult(new
            {
                error = "SubscriptionExpired",
                message = "Your subscription has expired and is in read-only mode. Contact your administrator to renew.",
                subscriptionHealth = sub.SubscriptionHealth
            })
            { StatusCode = StatusCodes.Status402PaymentRequired };
            return;
        }

        await next();
    }
}
