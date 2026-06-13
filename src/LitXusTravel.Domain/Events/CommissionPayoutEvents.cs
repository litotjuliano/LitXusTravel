using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record CommissionPayoutCreatedEvent(
    Guid PayoutId,
    Guid TenantId,
    decimal Amount,
    DateTime PeriodStart,
    DateTime PeriodEnd) : IDomainEvent
{
    public Guid EventId => PayoutId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionPayoutApprovedEvent(Guid PayoutId, Guid TenantId, decimal Amount) : IDomainEvent
{
    public Guid EventId => PayoutId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionPayoutProcessedEvent(Guid PayoutId, Guid TenantId, decimal Amount) : IDomainEvent
{
    public Guid EventId => PayoutId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionPayoutFailedEvent(Guid PayoutId, Guid TenantId, decimal Amount) : IDomainEvent
{
    public Guid EventId => PayoutId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
