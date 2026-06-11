using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.DomainEvents;

public sealed record PackagePublishedEvent(Guid PackageId, string Title) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
