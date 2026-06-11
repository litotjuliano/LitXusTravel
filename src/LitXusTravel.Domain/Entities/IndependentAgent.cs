using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Events;
using LitXusTravel.Domain.Exceptions;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Independent Agent represents freelance agents/resellers.
/// They have their own subscription and white-label website.
/// They can resell tours from multiple tenants.
/// </summary>
public class IndependentAgent : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string SubscriptionTier { get; private set; } = string.Empty;
    public string WhiteLabelDomain { get; private set; } = string.Empty;
    public List<Guid> AuthorizedTenantIds { get; private set; } = [];
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private IndependentAgent() { }

    /// <summary>
    /// Create a new independent agent with white-label website.
    /// </summary>
    public static IndependentAgent Create(string name, Email email, string subscriptionTier, string whiteLabelDomain)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Agent name is required");
        if (string.IsNullOrWhiteSpace(subscriptionTier))
            throw new DomainException("Subscription tier is required");
        if (string.IsNullOrWhiteSpace(whiteLabelDomain))
            throw new DomainException("White-label domain is required");

        var agent = new IndependentAgent
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            SubscriptionTier = subscriptionTier,
            WhiteLabelDomain = whiteLabelDomain,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        agent.RaiseDomainEvent(new IndependentAgentCreatedEvent(
            agent.Id, agent.Name, agent.SubscriptionTier, agent.WhiteLabelDomain));
        return agent;
    }

    /// <summary>
    /// Authorize the agent to resell a tenant's tours.
    /// </summary>
    public void AuthorizeForTenant(Guid tenantId)
    {
        if (!IsActive)
            throw new DomainException("Cannot authorize inactive agent");
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required");
        if (AuthorizedTenantIds.Contains(tenantId))
            throw new DomainException("Agent is already authorized for this tenant");

        AuthorizedTenantIds.Add(tenantId);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new IndependentAgentAuthorizedForTenantEvent(Id, tenantId));
    }

    /// <summary>
    /// Revoke authorization for a tenant.
    /// </summary>
    public void RevokeAuthorizationForTenant(Guid tenantId)
    {
        if (!AuthorizedTenantIds.Contains(tenantId))
            throw new DomainException("Agent is not authorized for this tenant");

        AuthorizedTenantIds.Remove(tenantId);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new IndependentAgentAuthorizationRevokedEvent(Id, tenantId));
    }

    /// <summary>
    /// Verify if the agent is authorized to resell a tenant's tours.
    /// </summary>
    public bool IsAuthorizedForTenant(Guid tenantId)
    {
        if (!IsActive) return false;
        return AuthorizedTenantIds.Contains(tenantId);
    }

    /// <summary>
    /// Upgrade or downgrade subscription tier.
    /// </summary>
    public void UpdateSubscription(string newTier)
    {
        if (string.IsNullOrWhiteSpace(newTier))
            throw new DomainException("Subscription tier is required");

        var oldTier = SubscriptionTier;
        SubscriptionTier = newTier;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new IndependentAgentSubscriptionUpdatedEvent(Id, oldTier, newTier));
    }

    /// <summary>
    /// Deactivate the agent.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new IndependentAgentDeactivatedEvent(Id));
    }

    /// <summary>
    /// Reactivate the agent.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new IndependentAgentActivatedEvent(Id));
    }
}
