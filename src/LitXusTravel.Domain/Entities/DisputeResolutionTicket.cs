namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Dispute resolution ticket for addressing commission discrepancies.
/// Safeguard 10: Dispute resolution workflow instead of direct SuperAdmin override.
/// </summary>
public class DisputeResolutionTicket : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid SuperAdminId { get; private set; }
    public Guid CommissionAccrualId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string ProposedFix { get; private set; } = string.Empty;
    public DisputeReasonCode ReasonCode { get; private set; }
    public DisputeStatus Status { get; private set; }
    public Guid? ReviewedByTenantAdminId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public decimal? OriginalAmount { get; private set; }
    public decimal? AdjustedAmount { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private DisputeResolutionTicket() { }

    /// <summary>
    /// Create a new dispute ticket.
    /// Safeguard 10: Structured workflow instead of direct override.
    /// </summary>
    public static DisputeResolutionTicket Create(
        Guid superAdminId,
        Guid commissionAccrualId,
        string description,
        string proposedFix,
        DisputeReasonCode reasonCode,
        decimal originalAmount)
    {
        if (superAdminId == Guid.Empty)
            throw new DomainException("SuperAdmin ID is required");
        if (commissionAccrualId == Guid.Empty)
            throw new DomainException("Commission accrual ID is required");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required");

        var ticket = new DisputeResolutionTicket
        {
            Id = Guid.NewGuid(),
            SuperAdminId = superAdminId,
            CommissionAccrualId = commissionAccrualId,
            Description = description,
            ProposedFix = proposedFix,
            ReasonCode = reasonCode,
            OriginalAmount = originalAmount,
            Status = DisputeStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        ticket.RaiseDomainEvent(new DisputeResolutionTicketCreatedEvent(
            ticket.Id, ticket.SuperAdminId, ticket.CommissionAccrualId, originalAmount));
        return ticket;
    }

    /// <summary>
    /// Tenant Admin approves the dispute resolution.
    /// </summary>
    public void Approve(Guid tenantAdminId, decimal adjustedAmount)
    {
        if (Status != DisputeStatus.Open)
            throw new DomainException("Only open disputes can be approved");

        Status = DisputeStatus.Approved;
        ReviewedByTenantAdminId = tenantAdminId;
        AdjustedAmount = adjustedAmount;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DisputeResolutionApprovedEvent(
            Id, CommissionAccrualId, OriginalAmount ?? 0, adjustedAmount));
    }

    /// <summary>
    /// Tenant Admin rejects the dispute resolution.
    /// </summary>
    public void Reject(Guid tenantAdminId)
    {
        if (Status != DisputeStatus.Open)
            throw new DomainException("Only open disputes can be rejected");

        Status = DisputeStatus.Rejected;
        ReviewedByTenantAdminId = tenantAdminId;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DisputeResolutionRejectedEvent(Id, CommissionAccrualId));
    }

    /// <summary>
    /// Mark as resolved (after adjustment is applied).
    /// </summary>
    public void MarkAsResolved()
    {
        if (Status != DisputeStatus.Approved)
            throw new DomainException("Only approved disputes can be marked as resolved");

        Status = DisputeStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DisputeResolutionResolvedEvent(Id, CommissionAccrualId));
    }
}

/// <summary>
/// Dispute reason code enumeration.
/// </summary>
public enum DisputeReasonCode
{
    Miscalculation = 0,
    IncorrectTrigger = 1,
    WrongRate = 2,
    SystemError = 3,
    Other = 4
}

/// <summary>
/// Dispute status enumeration.
/// </summary>
public enum DisputeStatus
{
    Open = 0,
    PendingReview = 1,
    Approved = 2,
    Rejected = 3,
    Resolved = 4
}
