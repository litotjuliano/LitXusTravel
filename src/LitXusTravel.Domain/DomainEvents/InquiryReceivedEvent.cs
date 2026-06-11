using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.DomainEvents;

public sealed record InquiryReceivedEvent(Guid InquiryId, Guid TenantId, string CustomerName, string CustomerEmail) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
