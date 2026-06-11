namespace LitXusTravel.Domain.Events;

public record CommissionPayoutCreatedEvent(
    Guid PayoutId,
    Guid TenantId,
    decimal Amount,
    DateTime PeriodStart,
    DateTime PeriodEnd) : IDomainEvent
{
    public Guid AggregateId => PayoutId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionPayoutApprovedEvent(Guid PayoutId, Guid TenantId, decimal Amount) : IDomainEvent
{
    public Guid AggregateId => PayoutId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionPayoutProcessedEvent(Guid PayoutId, Guid TenantId, decimal Amount) : IDomainEvent
{
    public Guid AggregateId => PayoutId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionPayoutFailedEvent(Guid PayoutId, Guid TenantId, decimal Amount) : IDomainEvent
{
    public Guid AggregateId => PayoutId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
