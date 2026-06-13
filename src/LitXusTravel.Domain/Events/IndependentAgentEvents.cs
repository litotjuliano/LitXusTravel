using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record IndependentAgentCreatedEvent(Guid AgentId, string Name, string SubscriptionTier, string WhiteLabelDomain) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record IndependentAgentAuthorizedForTenantEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record IndependentAgentAuthorizationRevokedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record IndependentAgentSubscriptionUpdatedEvent(Guid AgentId, string OldTier, string NewTier) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record IndependentAgentDeactivatedEvent(Guid AgentId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record IndependentAgentActivatedEvent(Guid AgentId) : IDomainEvent
{
    public Guid EventId => AgentId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
