using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class AdminUserRepository : Repository<AdminUser>, IAdminUserRepository
{
    public AdminUserRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<AdminUser?> GetByEmailAsync(Email email, CancellationToken ct = default)
    {
        return await _context.AdminUsers
            .FirstOrDefaultAsync(a => a.Email == email, ct);
    }

    public async Task<IEnumerable<AdminUser>> GetPlatformAdminsAsync(CancellationToken ct = default)
    {
        return await _context.AdminUsers
            .Where(a => a.Role == AdminRole.Admin && a.Scope == AdminScope.Platform && a.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<AdminUser>> GetTenantAdminsAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.AdminUsers
            .Where(a => a.Scope == AdminScope.Tenant && a.AssignedTenantId == tenantId && a.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<AdminUser>> GetActiveAdminsAsync(CancellationToken ct = default)
    {
        return await _context.AdminUsers
            .Where(a => a.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default)
    {
        return await _context.AdminUsers
            .AnyAsync(a => a.Email == email, ct);
    }
}
