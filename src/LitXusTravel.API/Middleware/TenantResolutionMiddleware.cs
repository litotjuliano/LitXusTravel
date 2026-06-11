using LitXusTravel.Application.Interfaces.Services;

namespace LitXusTravel.API.Middleware;

public class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantResolver resolver, ICurrentTenant currentTenant)
    {
        var host = context.Request.Host.Host;

        // Skip tenant resolution for CORS preflight requests
        if (context.Request.Method == HttpMethods.Options)
        {
            await next(context);
            return;
        }

        // Skip tenant resolution for admin routes
        if (context.Request.Path.StartsWithSegments("/api/admin"))
        {
            await next(context);
            return;
        }

        // Try header first (used by API clients), then subdomain
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader)
            && Guid.TryParse(tenantIdHeader, out var tenantIdFromHeader))
        {
            currentTenant.SetTenant(tenantIdFromHeader, host);
        }
        else
        {
            var tenantId = await resolver.ResolveFromHostAsync(host);
            if (tenantId.HasValue)
            {
                var subdomain = host.Split('.')[0];
                currentTenant.SetTenant(tenantId.Value, subdomain);
            }
        }

        await next(context);
    }
}
