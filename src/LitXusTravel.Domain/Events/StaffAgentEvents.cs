namespace LitXusTravel.Domain.Events;

public record StaffAgentCreatedEvent(Guid AgentId, Guid TenantId, string Name, string UniqueCode) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record StaffAgentCodeRotatedEvent(Guid AgentId, Guid TenantId, string NewCode) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record StaffAgentDepartedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record StaffAgentDeactivatedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record StaffAgentActivatedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
