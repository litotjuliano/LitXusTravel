namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Staff Agent represents internal employees of a tenant.
/// They have unique referral codes and earn commissions on sales they make.
/// </summary>
public class StaffAgent : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string UniqueCode { get; private set; } = string.Empty;
    public DateTime CodeIssuedAt { get; private set; }
    public DateTime? CodeExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? DepartedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private StaffAgent() { }

    /// <summary>
    /// Create a new staff agent for a tenant.
    /// </summary>
    public static StaffAgent Create(Guid tenantId, string name, Email email)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff agent name is required");

        var code = GenerateUniqueCode(name);
        var agent = new StaffAgent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Email = email,
            UniqueCode = code,
            CodeIssuedAt = DateTime.UtcNow,
            CodeExpiresAt = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        agent.RaiseDomainEvent(new StaffAgentCreatedEvent(agent.Id, agent.TenantId, agent.Name, agent.UniqueCode));
        return agent;
    }

    /// <summary>
    /// Rotate the staff agent's referral code (typically monthly).
    /// </summary>
    public void RotateCode()
    {
        if (!IsActive)
            throw new DomainException("Cannot rotate code for inactive agent");

        var newCode = GenerateUniqueCode(Name);
        UniqueCode = newCode;
        CodeIssuedAt = DateTime.UtcNow;
        CodeExpiresAt = DateTime.UtcNow.AddMonths(1);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new StaffAgentCodeRotatedEvent(Id, TenantId, newCode));
    }

    /// <summary>
    /// Mark the agent as departed.
    /// </summary>
    public void MarkAsDeparted()
    {
        if (!IsActive)
            throw new DomainException("Agent is already inactive");

        IsActive = false;
        DepartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new StaffAgentDepartedEvent(Id, TenantId));
    }

    /// <summary>
    /// Deactivate the agent without marking as departed.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new StaffAgentDeactivatedEvent(Id, TenantId));
    }

    /// <summary>
    /// Reactivate the agent.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new StaffAgentActivatedEvent(Id, TenantId));
    }

    /// <summary>
    /// Verify if a code belongs to this agent and is still valid.
    /// </summary>
    public bool IsCodeValid(string code)
    {
        if (!IsActive) return false;
        if (code != UniqueCode) return false;
        if (CodeExpiresAt.HasValue && DateTime.UtcNow > CodeExpiresAt.Value) return false;
        return true;
    }

    /// <summary>
    /// Verify that this agent is not trying to use their own code (self-booking prevention).
    /// </summary>
    public void ValidateSelfBookingPrevention(string referralCode)
    {
        if (referralCode == UniqueCode)
            throw new DomainException("Staff agent cannot use their own referral code");
    }

    private static string GenerateUniqueCode(string name)
    {
        // Format: STAFF-{FirstName}-{Sequence}
        var firstNamePart = name.Split(' ')[0].ToUpperInvariant();
        var sequence = Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant();
        return $"STAFF-{firstNamePart}-{sequence}";
    }
}
