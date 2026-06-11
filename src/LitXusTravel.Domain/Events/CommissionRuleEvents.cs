using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
namespace LitXusTravel.Domain.Events;

public record CommissionRuleCreatedEvent(
    Guid RuleId,
    Guid TenantId,
    Guid? AgentId,
    CommissionTrigger Trigger,
    decimal Amount,
    bool IsPercentage) : IDomainEvent
{
    public Guid EventId => RuleId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionRuleUpdatedEvent(
    Guid RuleId,
    Guid TenantId,
    decimal Amount,
    bool IsPercentage) : IDomainEvent
{
    public Guid EventId => RuleId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}

public record CommissionRuleDeactivatedEvent(Guid RuleId, Guid TenantId) : IDomainEvent
{
    public Guid EventId => RuleId;
    public DateTimeOffset OccurredAt => DateTimeOffset.UtcNow;
}
