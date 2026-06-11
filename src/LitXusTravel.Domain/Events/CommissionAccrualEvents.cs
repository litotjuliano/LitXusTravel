using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record CommissionAccruedEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount) : IDomainEvent
{
    public Guid EventId => AccrualId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionFinalizedEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount) : IDomainEvent
{
    public Guid EventId => AccrualId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionReversedEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount,
    string Reason) : IDomainEvent
{
    public Guid EventId => AccrualId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionPendingPayoutEvent(Guid AccrualId, Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AccrualId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionPaidEvent(
    Guid AccrualId,
    Guid AgentId,
    Guid TenantId,
    decimal Amount) : IDomainEvent
{
    public Guid EventId => AccrualId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
