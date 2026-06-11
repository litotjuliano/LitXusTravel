namespace LitXusTravel.Application.Interfaces.Services;

public interface ICurrentTenant
{
    Guid? Id { get; }
    string? Subdomain { get; }
    bool IsResolved { get; }
    void SetTenant(Guid tenantId, string subdomain);
}
