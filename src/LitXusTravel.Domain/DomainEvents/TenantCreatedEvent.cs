using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.DomainEvents;

public sealed record TenantCreatedEvent(Guid TenantId, string Name, string Email) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
