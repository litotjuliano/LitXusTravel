namespace LitXusTravel.Domain.Events;

public record IndependentAgentCreatedEvent(Guid AgentId, string Name, string SubscriptionTier, string WhiteLabelDomain) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record IndependentAgentAuthorizedForTenantEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record IndependentAgentAuthorizationRevokedEvent(Guid AgentId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record IndependentAgentSubscriptionUpdatedEvent(Guid AgentId, string OldTier, string NewTier) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record IndependentAgentDeactivatedEvent(Guid AgentId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record IndependentAgentActivatedEvent(Guid AgentId) : IDomainEvent
{
    public Guid AggregateId => AgentId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
