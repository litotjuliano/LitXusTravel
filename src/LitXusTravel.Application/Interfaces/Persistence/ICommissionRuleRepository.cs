using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
namespace LitXusTravel.Application.Interfaces.Persistence;

public interface ICommissionRuleRepository : IRepository<CommissionRule>
{
    Task<CommissionRule?> GetDefaultRuleAsync(Guid tenantId, CommissionTrigger trigger, CancellationToken ct = default);
    Task<CommissionRule?> GetAgentRuleAsync(Guid tenantId, Guid agentId, CommissionTrigger trigger, CancellationToken ct = default);
    Task<IEnumerable<CommissionRule>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<CommissionRule>> GetActiveByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<CommissionRule>> GetByAgentAsync(Guid agentId, CancellationToken ct = default);
    Task<CommissionRule?> GetApplicableRuleAsync(Guid tenantId, Guid? agentId, CommissionTrigger trigger, CancellationToken ct = default);
}
