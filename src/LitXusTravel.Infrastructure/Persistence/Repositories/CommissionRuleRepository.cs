namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class CommissionRuleRepository : RepositoryBase<CommissionRule>, ICommissionRuleRepository
{
    public CommissionRuleRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<CommissionRule?> GetDefaultRuleAsync(Guid tenantId, CommissionTrigger trigger, CancellationToken ct = default)
    {
        return await _context.CommissionRules
            .Where(r => r.TenantId == tenantId && r.AgentId == null && r.Trigger == trigger && r.IsActive && r.IsApplicable())
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CommissionRule?> GetAgentRuleAsync(Guid tenantId, Guid agentId, CommissionTrigger trigger, CancellationToken ct = default)
    {
        return await _context.CommissionRules
            .Where(r => r.TenantId == tenantId && r.AgentId == agentId && r.Trigger == trigger && r.IsActive && r.IsApplicable())
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<CommissionRule>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CommissionRules
            .Where(r => r.TenantId == tenantId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionRule>> GetActiveByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CommissionRules
            .Where(r => r.TenantId == tenantId && r.IsActive && r.IsApplicable())
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommissionRule>> GetByAgentAsync(Guid agentId, CancellationToken ct = default)
    {
        return await _context.CommissionRules
            .Where(r => r.AgentId == agentId && r.IsActive)
            .ToListAsync(ct);
    }

    public async Task<CommissionRule?> GetApplicableRuleAsync(Guid tenantId, Guid? agentId, CommissionTrigger trigger, CancellationToken ct = default)
    {
        // Check for agent-specific rule first
        if (agentId.HasValue)
        {
            var agentRule = await GetAgentRuleAsync(tenantId, agentId.Value, trigger, ct);
            if (agentRule != null)
                return agentRule;
        }

        // Fall back to default rule
        return await GetDefaultRuleAsync(tenantId, trigger, ct);
    }
}
