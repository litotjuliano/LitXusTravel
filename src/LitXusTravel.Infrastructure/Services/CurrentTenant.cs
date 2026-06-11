using LitXusTravel.Application.Interfaces.Services;

namespace LitXusTravel.Infrastructure.Services;

public class CurrentTenant : ICurrentTenant
{
    public Guid? Id { get; private set; }
    public string? Subdomain { get; private set; }
    public bool IsResolved => Id.HasValue;

    public void SetTenant(Guid tenantId, string subdomain)
    {
        Id = tenantId;
        Subdomain = subdomain;
    }
}
