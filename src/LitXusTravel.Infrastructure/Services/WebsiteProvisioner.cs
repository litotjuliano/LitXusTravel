using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Infrastructure.Data.Contexts;

namespace LitXusTravel.Infrastructure.Services;

public class WebsiteProvisioner(
    LitXusTravelDbContext context,
    ILogger<WebsiteProvisioner> logger) : IWebsiteProvisioner
{
    public async Task<string> ProvisionForTenantAsync(Guid tenantId, string tenantName, CancellationToken ct = default)
    {
        var subdomain = await GenerateUniqueSubdomainAsync(tenantName, ct);
        logger.LogInformation("Provisioned subdomain {Subdomain} for tenant {TenantId}", subdomain, tenantId);
        return subdomain;
    }

    private async Task<string> GenerateUniqueSubdomainAsync(string tenantName, CancellationToken ct)
    {
        var base64 = System.Text.RegularExpressions.Regex
            .Replace(tenantName.ToLowerInvariant().Trim(), @"[^a-z0-9]+", "-")
            .Trim('-');

        if (base64.Length > 30) base64 = base64[..30].TrimEnd('-');

        var candidate = base64;
        var counter = 1;

        while (await context.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Subdomain == candidate, ct))
        {
            candidate = $"{base64}-{counter++}";
        }

        return candidate;
    }
}
