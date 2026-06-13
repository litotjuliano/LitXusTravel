using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
namespace LitXusTravel.Application.Interfaces.Persistence;

public interface ICommissionAccrualRepository : IRepository<CommissionAccrual>
{
    Task<IEnumerable<CommissionAccrual>> GetByAgentAsync(Guid agentId, CancellationToken ct = default);
    Task<IEnumerable<CommissionAccrual>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<CommissionAccrual>> GetBySourceAsync(Guid sourceId, CancellationToken ct = default);
    Task<IEnumerable<CommissionAccrual>> GetFinalizedAsync(Guid tenantId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default);
    Task<IEnumerable<CommissionAccrual>> GetByStatusAsync(CommissionStatus status, CancellationToken ct = default);
    Task<decimal> GetTotalFinalizedAsync(Guid agentId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default);
    Task<IEnumerable<CommissionAccrual>> GetPendingPayoutAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<CommissionAccrual>> GetByDisputeAsync(Guid disputeTicketId, CancellationToken ct = default);
}
