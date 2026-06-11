using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Events;
using LitXusTravel.Domain.Exceptions;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Commission rule defining how commissions are calculated for staff or independent agents.
/// Tenant admins configure these rules.
/// </summary>
public class CommissionRule : AggregateRoot
{
        public Guid TenantId { get; private set; }
    public Guid? AgentId { get; private set; }
    public CommissionTrigger Trigger { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsPercentage { get; private set; }
    public string PayoutFrequency { get; private set; } = "Monthly";
    public bool AutoApprove { get; private set; }
    public decimal MinimumThreshold { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
        
    private CommissionRule() { }

    /// <summary>
    /// Create a commission rule for a tenant (default rule applies to all agents).
    /// </summary>
    public static CommissionRule CreateDefault(
        Guid tenantId,
        CommissionTrigger trigger,
        decimal amount,
        bool isPercentage,
        decimal minimumThreshold = 100)
    {
        ValidateAmount(amount, isPercentage);

        var rule = new CommissionRule
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AgentId = null, // Default rule applies to all
            Trigger = trigger,
            Amount = amount,
            IsPercentage = isPercentage,
            AutoApprove = true,
            MinimumThreshold = minimumThreshold,
            EffectiveFrom = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        rule.RaiseDomainEvent(new CommissionRuleCreatedEvent(
            rule.Id, rule.TenantId, rule.AgentId, rule.Trigger, rule.Amount, rule.IsPercentage));
        return rule;
    }

    /// <summary>
    /// Create a custom commission rule for a specific agent.
    /// </summary>
    public static CommissionRule CreateForAgent(
        Guid tenantId,
        Guid agentId,
        CommissionTrigger trigger,
        decimal amount,
        bool isPercentage,
        decimal minimumThreshold = 100)
    {
        if (agentId == Guid.Empty)
            throw new DomainException("Agent ID is required for agent-specific rules");

        ValidateAmount(amount, isPercentage);

        var rule = new CommissionRule
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AgentId = agentId,
            Trigger = trigger,
            Amount = amount,
            IsPercentage = isPercentage,
            AutoApprove = true,
            MinimumThreshold = minimumThreshold,
            EffectiveFrom = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        rule.RaiseDomainEvent(new CommissionRuleCreatedEvent(
            rule.Id, rule.TenantId, rule.AgentId, rule.Trigger, rule.Amount, rule.IsPercentage));
        return rule;
    }

    /// <summary>
    /// Update the commission rule.
    /// </summary>
    public void Update(decimal amount, bool isPercentage, decimal minimumThreshold, bool autoApprove)
    {
        ValidateAmount(amount, isPercentage);

        Amount = amount;
        IsPercentage = isPercentage;
        MinimumThreshold = minimumThreshold;
        AutoApprove = autoApprove;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionRuleUpdatedEvent(Id, TenantId, Amount, IsPercentage));
    }

    /// <summary>
    /// Deactivate the rule.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        EffectiveTo = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionRuleDeactivatedEvent(Id, TenantId));
    }

    /// <summary>
    /// Check if the rule is currently active and applicable.
    /// </summary>
    public bool IsApplicable()
    {
        var now = DateTime.UtcNow;
        return IsActive &&
               now >= EffectiveFrom &&
               (!EffectiveTo.HasValue || now <= EffectiveTo.Value);
    }

    private static void ValidateAmount(decimal amount, bool isPercentage)
    {
        if (amount <= 0)
            throw new DomainException("Commission amount must be greater than 0");

        if (isPercentage && amount > 100)
            throw new DomainException("Commission percentage cannot exceed 100%");

        // Safeguard 9: Cap commission at 30% max
        if (isPercentage && amount > 30)
            throw new DomainException("Commission percentage cannot exceed 30% (system maximum)");
    }
}

/// <summary>
/// Commission trigger types.
/// </summary>
public enum CommissionTrigger
{
    TourBooked = 0,
    TourCompleted = 1,
    RevenueGenerated = 2
}
