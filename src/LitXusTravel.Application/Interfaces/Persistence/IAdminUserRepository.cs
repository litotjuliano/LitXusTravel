using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IAdminUserRepository : IRepository<AdminUser>
{
    Task<AdminUser?> GetByEmailAsync(Email email, CancellationToken ct = default);
    Task<IEnumerable<AdminUser>> GetPlatformAdminsAsync(CancellationToken ct = default);
    Task<IEnumerable<AdminUser>> GetTenantAdminsAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<AdminUser>> GetActiveAdminsAsync(CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
}
