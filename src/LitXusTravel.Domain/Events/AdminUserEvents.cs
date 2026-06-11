namespace LitXusTravel.Domain.Events;

public record AdminUserCreatedEvent(Guid AdminId, string Name, AdminRole Role) : IDomainEvent
{
    public Guid AggregateId => AdminId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record AdminUserDeactivatedEvent(Guid AdminId) : IDomainEvent
{
    public Guid AggregateId => AdminId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record AdminUserActivatedEvent(Guid AdminId) : IDomainEvent
{
    public Guid AggregateId => AdminId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record TenantAssignedToAdminEvent(Guid AdminId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AdminId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
