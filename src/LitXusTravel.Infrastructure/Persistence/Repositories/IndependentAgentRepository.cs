using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class IndependentAgentRepository : Repository<IndependentAgent>, IIndependentAgentRepository
{
    public IndependentAgentRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<IndependentAgent?> GetByEmailAsync(Email email, CancellationToken ct = default)
    {
        return await _context.IndependentAgents
            .FirstOrDefaultAsync(a => a.Email == email, ct);
    }

    public async Task<IndependentAgent?> GetByDomainAsync(string domain, CancellationToken ct = default)
    {
        return await _context.IndependentAgents
            .FirstOrDefaultAsync(a => a.WhiteLabelDomain == domain, ct);
    }

    public async Task<IEnumerable<IndependentAgent>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _context.IndependentAgents
            .Where(a => a.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<IndependentAgent>> GetBySubscriptionTierAsync(string tier, CancellationToken ct = default)
    {
        return await _context.IndependentAgents
            .Where(a => a.SubscriptionTier == tier && a.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<IndependentAgent>> GetAuthorizedForTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.IndependentAgents
            .Where(a => a.AuthorizedTenantIds.Contains(tenantId) && a.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> IsDomainUniqueAsync(string domain, Guid? excludeAgentId = null, CancellationToken ct = default)
    {
        var query = _context.IndependentAgents
            .Where(a => a.WhiteLabelDomain == domain);

        if (excludeAgentId.HasValue)
            query = query.Where(a => a.Id != excludeAgentId.Value);

        return !await query.AnyAsync(ct);
    }
}
