namespace LitXusTravel.Application.Interfaces.Services;

public interface IWebsiteProvisioner
{
    Task<string> ProvisionForTenantAsync(Guid tenantId, string tenantName, CancellationToken ct = default);
}
