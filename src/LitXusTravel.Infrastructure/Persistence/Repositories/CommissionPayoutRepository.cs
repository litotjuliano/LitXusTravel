namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class CommissionPayoutRepository : RepositoryBase<CommissionPayout>, ICommissionPayoutRepository
{
    public CommissionPayoutRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<CommissionPayout?> GetByPeriodAsync(Guid tenantId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default)
    {
        return await _context.CommissionPayouts
            .FirstOrDefaultAsync(p => p.TenantId == tenantId &&
                                       p.PayoutPeriodStart == periodStart &&
                                       p.PayoutPeriodEnd == periodEnd, ct);
    }

    public async Task<IEnumerable<CommissionPayout>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CommissionPayouts
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.PayoutPeriodEnd)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionPayout>> GetByStatusAsync(PayoutStatus status, CancellationToken ct = default)
    {
        return await _context.CommissionPayouts
            .Where(p => p.Status == status)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionPayout>> GetProcessedAsync(Guid tenantId, int monthsBack = 12, CancellationToken ct = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-monthsBack);
        return await _context.CommissionPayouts
            .Where(p => p.TenantId == tenantId &&
                        p.Status == PayoutStatus.Processed &&
                        p.ProcessedAt >= cutoffDate)
            .OrderByDescending(p => p.ProcessedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionPayout>> GetPendingAsync(CancellationToken ct = default)
    {
        return await _context.CommissionPayouts
            .Where(p => p.Status == PayoutStatus.Pending || p.Status == PayoutStatus.Approved)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalPaidAsync(Guid agentId, int monthsBack = 12, CancellationToken ct = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-monthsBack);
        return await _context.CommissionPayouts
            .Where(p => p.AgentId == agentId &&
                        p.Status == PayoutStatus.Processed &&
                        p.ProcessedAt >= cutoffDate)
            .SumAsync(p => p.TotalAmount, ct);
    }
}
