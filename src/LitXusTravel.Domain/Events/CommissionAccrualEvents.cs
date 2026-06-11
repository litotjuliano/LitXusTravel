namespace LitXusTravel.Domain.Events;

public record CommissionAccruedEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount) : IDomainEvent
{
    public Guid AggregateId => AccrualId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionFinalizedEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount) : IDomainEvent
{
    public Guid AggregateId => AccrualId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionReversedEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount,
    string Reason) : IDomainEvent
{
    public Guid AggregateId => AccrualId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionPendingPayoutEvent(Guid AccrualId, Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AccrualId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionPaidEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount) : IDomainEvent
{
    public Guid AggregateId => AccrualId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
