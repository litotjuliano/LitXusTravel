using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record StaffAgentCreatedEvent(Guid AgentId, Guid TenantId, string Name, string UniqueCode) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record StaffAgentCodeRotatedEvent(Guid AgentId, Guid TenantId, string NewCode) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record StaffAgentDepartedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record StaffAgentDeactivatedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record StaffAgentActivatedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
