using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Interfaces.Persistence;

public interface ITenantPackageRepository : IRepository<TenantPackage>
{
    Task<IEnumerable<TenantPackage>> GetByTenantWithDetailsAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<TenantPackage>> GetAllWithTenantsAsync(CancellationToken ct = default);
}
