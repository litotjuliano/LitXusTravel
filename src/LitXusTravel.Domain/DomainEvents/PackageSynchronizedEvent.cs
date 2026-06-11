using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.DomainEvents;

public sealed record PackageSynchronizedEvent(Guid TenantId, Guid MasterPackageId, Guid TenantPackageId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
