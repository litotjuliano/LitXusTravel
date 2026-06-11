namespace LitXusTravel.Application.Interfaces.Persistence;

public interface ICommissionPayoutRepository : IRepository<CommissionPayout>
{
    Task<CommissionPayout?> GetByPeriodAsync(Guid tenantId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default);
    Task<IEnumerable<CommissionPayout>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<CommissionPayout>> GetByStatusAsync(PayoutStatus status, CancellationToken ct = default);
    Task<IEnumerable<CommissionPayout>> GetProcessedAsync(Guid tenantId, int monthsBack = 12, CancellationToken ct = default);
    Task<IEnumerable<CommissionPayout>> GetPendingAsync(CancellationToken ct = default);
    Task<decimal> GetTotalPaidAsync(Guid agentId, int monthsBack = 12, CancellationToken ct = default);
}
