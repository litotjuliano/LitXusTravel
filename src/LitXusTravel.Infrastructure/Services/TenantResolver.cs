using Microsoft.EntityFrameworkCore;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Infrastructure.Data.Contexts;

namespace LitXusTravel.Infrastructure.Services;

public class TenantResolver(LitXusTravelDbContext context) : ITenantResolver
{
    public async Task<Guid?> ResolveFromSubdomainAsync(string subdomain, CancellationToken ct = default)
    {
        var tenant = await context.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(t => t.Subdomain == subdomain.ToLowerInvariant() && t.IsActive)
            .Select(t => new { t.Id })
            .FirstOrDefaultAsync(ct);

        return tenant?.Id;
    }

    public async Task<Guid?> ResolveFromHostAsync(string host, CancellationToken ct = default)
    {
        // Extract subdomain from host (e.g., "agency.litxustravel.com" → "agency")
        var parts = host.Split('.');
        if (parts.Length >= 2)
            return await ResolveFromSubdomainAsync(parts[0], ct);

        return null;
    }

    public async Task<(Guid Id, string Name)?> ResolveTenantInfoAsync(string subdomain, CancellationToken ct = default)
    {
        var tenant = await context.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(t => t.Subdomain == subdomain.ToLowerInvariant() && t.IsActive)
            .Select(t => new { t.Id, t.Name })
            .FirstOrDefaultAsync(ct);

        return tenant is null ? null : (tenant.Id, tenant.Name);
    }
}
