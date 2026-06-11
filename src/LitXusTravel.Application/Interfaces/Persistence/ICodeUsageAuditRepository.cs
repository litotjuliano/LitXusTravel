namespace LitXusTravel.Application.Interfaces.Persistence;

public interface ICodeUsageAuditRepository : IRepository<CodeUsageAudit>
{
    Task<IEnumerable<CodeUsageAudit>> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IEnumerable<CodeUsageAudit>> GetByCodeAndPeriodAsync(string code, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IEnumerable<CodeUsageAudit>> GetByAgentAsync(Guid agentId, CancellationToken ct = default);
    Task<IEnumerable<CodeUsageAudit>> GetAnomalousUsageAsync(Guid tenantId, DateTime startDate, CancellationToken ct = default);
    Task<int> GetUsageCountAsync(string code, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IEnumerable<CodeUsageAudit>> GetByLocationAnomalyAsync(string code, DateTime lookbackDays = 7, CancellationToken ct = default);
}
