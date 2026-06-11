namespace LitXusTravel.Domain.Events;

public record CommissionRuleCreatedEvent(
    Guid RuleId,
    Guid TenantId,
    Guid? AgentId,
    CommissionTrigger Trigger,
    decimal Amount,
    bool IsPercentage) : IDomainEvent
{
    public Guid AggregateId => RuleId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionRuleUpdatedEvent(
    Guid RuleId,
    Guid TenantId,
    decimal Amount,
    bool IsPercentage) : IDomainEvent
{
    public Guid AggregateId => RuleId;
    public DateTime OccurredAt => DateTime.UtcNow;
}

public record CommissionRuleDeactivatedEvent(Guid RuleId, Guid TenantId) : IDomainEvent
{
    public Guid AggregateId => RuleId;
    public DateTime OccurredAt => DateTime.UtcNow;
}
