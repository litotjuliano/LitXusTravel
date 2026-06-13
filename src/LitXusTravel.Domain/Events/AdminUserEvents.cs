using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record AdminUserCreatedEvent(Guid AdminId, string Name, AdminRole Role) : IDomainEvent
{
    public Guid EventId => AdminId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record AdminUserDeactivatedEvent(Guid AdminId) : IDomainEvent
{
    public Guid EventId => AdminId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record AdminUserActivatedEvent(Guid AdminId) : IDomainEvent
{
    public Guid EventId => AdminId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record TenantAssignedToAdminEvent(Guid AdminId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AdminId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
