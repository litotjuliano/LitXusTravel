using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IStaffAgentRepository : IRepository<StaffAgent>
{
    Task<StaffAgent?> GetByCodeAsync(string code, Guid tenantId, CancellationToken ct = default);
    Task<StaffAgent?> GetByEmailAsync(Email email, Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<StaffAgent>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<StaffAgent>> GetActiveByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<StaffAgent>> GetDepartedAsync(CancellationToken ct = default);
    Task<bool> IsCodeUniqueAsync(string code, Guid tenantId, Guid? excludeAgentId = null, CancellationToken ct = default);
}
