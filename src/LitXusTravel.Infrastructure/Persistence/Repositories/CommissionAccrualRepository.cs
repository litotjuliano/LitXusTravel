using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class CommissionAccrualRepository : Repository<CommissionAccrual>, ICommissionAccrualRepository
{
    public CommissionAccrualRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CommissionAccrual>> GetByAgentAsync(Guid agentId, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.AgentId == agentId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionAccrual>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.TenantId == tenantId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionAccrual>> GetBySourceAsync(Guid sourceId, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.SourceId == sourceId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionAccrual>> GetFinalizedAsync(Guid tenantId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.TenantId == tenantId &&
                        c.Status == CommissionStatus.Finalized &&
                        c.AccruedAt >= periodStart &&
                        c.AccruedAt <= periodEnd)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionAccrual>> GetByStatusAsync(CommissionStatus status, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.Status == status)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalFinalizedAsync(Guid agentId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.AgentId == agentId &&
                        c.Status == CommissionStatus.Finalized &&
                        c.AccruedAt >= periodStart &&
                        c.AccruedAt <= periodEnd)
            .SumAsync(c => c.CommissionAmount, ct);
    }

    public async Task<IEnumerable<CommissionAccrual>> GetPendingPayoutAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.TenantId == tenantId && c.Status == CommissionStatus.PendingPayout)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionAccrual>> GetByDisputeAsync(Guid disputeTicketId, CancellationToken ct = default)
    {
        return await _context.CommissionAccruals
            .Where(c => c.DisputeTicketId == disputeTicketId)
            .ToListAsync(ct);
    }
}
