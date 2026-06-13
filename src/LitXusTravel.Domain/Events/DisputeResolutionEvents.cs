using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record DisputeResolutionTicketCreatedEvent(
    Guid TicketId,
    Guid SuperAdminId,
    Guid CommissionAccrualId,
    decimal OriginalAmount) : IDomainEvent
{
    public Guid EventId => TicketId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record DisputeResolutionApprovedEvent(
    Guid TicketId,
    Guid CommissionAccrualId,
    decimal OriginalAmount,
    decimal AdjustedAmount) : IDomainEvent
{
    public Guid EventId => TicketId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record DisputeResolutionRejectedEvent(Guid TicketId, Guid CommissionAccrualId) : IDomainEvent
{
    public Guid EventId => TicketId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record DisputeResolutionResolvedEvent(Guid TicketId, Guid CommissionAccrualId) : IDomainEvent
{
    public Guid EventId => TicketId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
