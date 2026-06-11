using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class StaffAgentRepository : Repository<StaffAgent>, IStaffAgentRepository
{
    public StaffAgentRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<StaffAgent?> GetByCodeAsync(string code, Guid tenantId, CancellationToken ct = default)
    {
        return await _context.StaffAgents
            .FirstOrDefaultAsync(s => s.UniqueCode == code && s.TenantId == tenantId, ct);
    }

    public async Task<StaffAgent?> GetByEmailAsync(Email email, Guid tenantId, CancellationToken ct = default)
    {
        return await _context.StaffAgents
            .FirstOrDefaultAsync(s => s.Email == email && s.TenantId == tenantId, ct);
    }

    public async Task<IEnumerable<StaffAgent>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.StaffAgents
            .Where(s => s.TenantId == tenantId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<StaffAgent>> GetActiveByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.StaffAgents
            .Where(s => s.TenantId == tenantId && s.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<StaffAgent>> GetDepartedAsync(CancellationToken ct = default)
    {
        return await _context.StaffAgents
            .Where(s => s.DepartedAt != null && !s.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid tenantId, Guid? excludeAgentId = null, CancellationToken ct = default)
    {
        var query = _context.StaffAgents
            .Where(s => s.UniqueCode == code && s.TenantId == tenantId);

        if (excludeAgentId.HasValue)
            query = query.Where(s => s.Id != excludeAgentId.Value);

        return !await query.AnyAsync(ct);
    }
}
