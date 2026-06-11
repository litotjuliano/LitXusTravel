namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IIndependentAgentRepository : IRepository<IndependentAgent>
{
    Task<IndependentAgent?> GetByEmailAsync(Email email, CancellationToken ct = default);
    Task<IndependentAgent?> GetByDomainAsync(string domain, CancellationToken ct = default);
    Task<IEnumerable<IndependentAgent>> GetActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<IndependentAgent>> GetBySubscriptionTierAsync(string tier, CancellationToken ct = default);
    Task<IEnumerable<IndependentAgent>> GetAuthorizedForTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<bool> IsDomainUniqueAsync(string domain, Guid? excludeAgentId = null, CancellationToken ct = default);
}
