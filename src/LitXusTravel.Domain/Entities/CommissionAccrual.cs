namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Commission accrual tracks earned commissions from bookings.
/// Commissions progress through states: Accrued → Finalized → Paid.
/// Safeguard 1: Commission only finalizes on tour completion, not booking.
/// </summary>
public class CommissionAccrual : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid AgentId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CommissionRuleId { get; private set; }
    public CommissionTriggerType TriggerType { get; private set; }
    public Guid SourceId { get; private set; }
    public decimal CommissionAmount { get; private set; }
    public decimal? CommissionPercentage { get; private set; }
    public decimal BaseAmount { get; private set; }
    public CommissionStatus Status { get; private set; }
    public DateTime AccruedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid? PayoutId { get; private set; }
    public Guid? DisputeTicketId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CommissionAccrual() { }

    /// <summary>
    /// Create a new accrued commission from a booking.
    /// Status starts as "Accrued" and only transitions to "Finalized" when tour completes.
    /// Safeguard 1: Commission finalizes only on tour completion.
    /// </summary>
    public static CommissionAccrual CreateFromBooking(
        Guid agentId,
        Guid tenantId,
        Guid commissionRuleId,
        Guid bookingId,
        CommissionTriggerType triggerType,
        decimal commissionAmount,
        decimal? commissionPercentage,
        decimal baseAmount)
    {
        if (agentId == Guid.Empty)
            throw new DomainException("Agent ID is required");
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required");
        if (commissionAmount < 0)
            throw new DomainException("Commission amount cannot be negative");

        var accrual = new CommissionAccrual
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            TenantId = tenantId,
            CommissionRuleId = commissionRuleId,
            TriggerType = triggerType,
            SourceId = bookingId,
            CommissionAmount = commissionAmount,
            CommissionPercentage = commissionPercentage,
            BaseAmount = baseAmount,
            Status = CommissionStatus.Accrued,
            AccruedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        accrual.RaiseDomainEvent(new CommissionAccruedEvent(
            accrual.Id, accrual.AgentId, accrual.TenantId, accrual.CommissionAmount));
        return accrual;
    }

    /// <summary>
    /// Finalize the commission when tour completes.
    /// Safeguard 1: Only finalized commissions are included in payouts.
    /// </summary>
    public void Finalize()
    {
        if (Status != CommissionStatus.Accrued)
            throw new DomainException("Only accrued commissions can be finalized");

        Status = CommissionStatus.Finalized;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionFinalizedEvent(Id, AgentId, TenantId, CommissionAmount));
    }

    /// <summary>
    /// Reverse the commission (used for cancellations and refunds).
    /// Safeguard 1: Cancelled bookings automatically reverse commissions.
    /// </summary>
    public void Reverse(string reason = "")
    {
        if (Status == CommissionStatus.Paid)
            throw new DomainException("Cannot reverse a commission that has already been paid. Create a refund record instead.");

        Status = CommissionStatus.Reversed;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionReversedEvent(Id, AgentId, TenantId, CommissionAmount, reason));
    }

    /// <summary>
    /// Mark as pending payout.
    /// </summary>
    public void MarkPendingPayout()
    {
        if (Status != CommissionStatus.Finalized)
            throw new DomainException("Only finalized commissions can be marked for payout");

        Status = CommissionStatus.PendingPayout;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionPendingPayoutEvent(Id, AgentId, TenantId));
    }

    /// <summary>
    /// Mark as paid.
    /// </summary>
    public void MarkAsPaid(Guid payoutId)
    {
        if (Status != CommissionStatus.PendingPayout)
            throw new DomainException("Only pending commissions can be marked as paid");

        Status = CommissionStatus.Paid;
        PaidAt = DateTime.UtcNow;
        PayoutId = payoutId;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionPaidEvent(Id, AgentId, TenantId, CommissionAmount));
    }

    /// <summary>
    /// Create a reversal record for a refund that occurred after payment.
    /// Safeguard 8: Refund reversals tracked and applied to next payout.
    /// </summary>
    public CommissionAccrual CreateRefundReversal()
    {
        if (Status != CommissionStatus.Paid)
            throw new DomainException("Can only create refund reversals for paid commissions");

        var reversal = new CommissionAccrual
        {
            Id = Guid.NewGuid(),
            AgentId = AgentId,
            TenantId = TenantId,
            CommissionRuleId = CommissionRuleId,
            TriggerType = TriggerType,
            SourceId = SourceId,
            CommissionAmount = -CommissionAmount, // Negative amount for reversal
            CommissionPercentage = CommissionPercentage,
            BaseAmount = BaseAmount,
            Status = CommissionStatus.Accrued, // Will be included in next payout
            AccruedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        return reversal;
    }
}

/// <summary>
/// Commission trigger type enumeration.
/// </summary>
public enum CommissionTriggerType
{
    TourBooked = 0,
    TourCompleted = 1,
    RevenueGenerated = 2
}

/// <summary>
/// Commission status enumeration.
/// </summary>
public enum CommissionStatus
{
    Accrued = 0,
    Finalized = 1,
    PendingPayout = 2,
    Paid = 3,
    Reversed = 4,
    Cancelled = 5,
    Refunded = 6
}
