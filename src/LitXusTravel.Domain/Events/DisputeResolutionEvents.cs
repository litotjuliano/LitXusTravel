namespace LitXusTravel.Domain.Events;

public record DisputeResolutionTicketCreatedEvent(
    Guid TicketId,
    Guid SuperAdminId,
    Guid CommissionAccrualId,
    decimal OriginalAmount) : IDomainEvent
{
    public Guid AggregateId => TicketId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record DisputeResolutionApprovedEvent(
    Guid TicketId,
    Guid CommissionAccrualId,
    decimal OriginalAmount,
    decimal AdjustedAmount) : IDomainEvent
{
    public Guid AggregateId => TicketId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record DisputeResolutionRejectedEvent(Guid TicketId, Guid CommissionAccrualId) : IDomainEvent
{
    public Guid AggregateId => TicketId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record DisputeResolutionResolvedEvent(Guid TicketId, Guid CommissionAccrualId) : IDomainEvent
{
    public Guid AggregateId => TicketId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
