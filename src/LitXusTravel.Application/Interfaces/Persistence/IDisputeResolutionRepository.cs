namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IDisputeResolutionRepository : IRepository<DisputeResolutionTicket>
{
    Task<IEnumerable<DisputeResolutionTicket>> GetOpenAsync(CancellationToken ct = default);
    Task<IEnumerable<DisputeResolutionTicket>> GetByStatusAsync(DisputeStatus status, CancellationToken ct = default);
    Task<IEnumerable<DisputeResolutionTicket>> GetByCommissionAccrualAsync(Guid commissionAccrualId, CancellationToken ct = default);
    Task<IEnumerable<DisputeResolutionTicket>> GetByCreatorAsync(Guid superAdminId, CancellationToken ct = default);
    Task<IEnumerable<DisputeResolutionTicket>> GetPendingReviewAsync(CancellationToken ct = default);
    Task<IEnumerable<DisputeResolutionTicket>> GetResolvedAsync(int daysBack = 30, CancellationToken ct = default);
}
