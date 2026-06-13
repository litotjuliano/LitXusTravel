using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Interfaces.Persistence;

public interface ITourRepository : IRepository<Tour>
{
    Task<IEnumerable<Tour>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<Tour>> GetScheduledByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<Tour>> GetCompletedByTenantAsync(Guid tenantId, CancellationToken ct = default);
}
