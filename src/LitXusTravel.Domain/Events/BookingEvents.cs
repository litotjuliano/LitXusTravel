using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.Events;

public record BookingCreatedEvent(Guid BookingId, Guid TenantId, Guid TourId, string CustomerEmail, decimal TotalAmount) : IDomainEvent
{
    public Guid EventId => BookingId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record BookingConfirmedEvent(Guid BookingId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => BookingId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record BookingCompletedEvent(Guid BookingId, Guid TenantId, Guid? AgentId) : IDomainEvent
{
    public Guid EventId => BookingId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record BookingCancelledEvent(Guid BookingId, Guid TenantId, Guid? AgentId, string Reason) : IDomainEvent
{
    public Guid EventId => BookingId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record TourCreatedEvent(Guid TourId, Guid TenantId, string Title) : IDomainEvent
{
    public Guid EventId => TourId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record TourCompletedEvent(Guid TourId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => TourId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record TourCancelledEvent(Guid TourId, Guid TenantId, string Reason) : IDomainEvent
{
    public Guid EventId => TourId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
