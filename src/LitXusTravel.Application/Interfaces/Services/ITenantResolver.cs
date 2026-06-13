namespace LitXusTravel.Application.Interfaces.Services;

public interface ITenantResolver
{
    Task<Guid?> ResolveFromSubdomainAsync(string subdomain, CancellationToken ct = default);
    Task<Guid?> ResolveFromHostAsync(string host, CancellationToken ct = default);
    Task<(Guid Id, string Name)?> ResolveTenantInfoAsync(string subdomain, CancellationToken ct = default);
}
