namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class CodeUsageAuditRepository : RepositoryBase<CodeUsageAudit>, ICodeUsageAuditRepository
{
    public CodeUsageAuditRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CodeUsageAudit>> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.CodeUsageAudits
            .Where(c => c.Code == code)
            .OrderByDescending(c => c.UsedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CodeUsageAudit>> GetByCodeAndPeriodAsync(string code, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _context.CodeUsageAudits
            .Where(c => c.Code == code && c.UsedAt >= startDate && c.UsedAt <= endDate)
            .OrderByDescending(c => c.UsedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CodeUsageAudit>> GetByAgentAsync(Guid agentId, CancellationToken ct = default)
    {
        return await _context.CodeUsageAudits
            .Where(c => c.StaffAgentId == agentId)
            .OrderByDescending(c => c.UsedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CodeUsageAudit>> GetAnomalousUsageAsync(Guid tenantId, DateTime startDate, CancellationToken ct = default)
    {
        // Safeguard 3: Code sharing prevention - detect anomalous patterns
        return await _context.CodeUsageAudits
            .Where(c => c.TenantId == tenantId && c.UsedAt >= startDate)
            .OrderByDescending(c => c.UsedAt)
            .ToListAsync(ct);
    }

    public async Task<int> GetUsageCountAsync(string code, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _context.CodeUsageAudits
            .CountAsync(c => c.Code == code && c.UsedAt >= startDate && c.UsedAt <= endDate, ct);
    }

    public async Task<IEnumerable<CodeUsageAudit>> GetByLocationAnomalyAsync(string code, DateTime lookbackDays = 7, CancellationToken ct = default)
    {
        // Safeguard 3: Detect geographic impossibilities
        var cutoffDate = DateTime.UtcNow.AddDays(-lookbackDays);
        return await _context.CodeUsageAudits
            .Where(c => c.Code == code && c.UsedAt >= cutoffDate)
            .OrderByDescending(c => c.UsedAt)
            .ToListAsync(ct);
    }
}
