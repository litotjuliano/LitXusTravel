using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Events;
using LitXusTravel.Domain.Exceptions;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Commission payout represents a monthly settlement of commissions to agents.
/// </summary>
public class CommissionPayout : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid? AgentId { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime PayoutPeriodStart { get; private set; }
    public DateTime PayoutPeriodEnd { get; private set; }
    public List<Guid> CommissionAccrualIds { get; private set; } = [];
    public decimal TotalAmount { get; private set; }
    public PayoutStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CommissionPayout() { }

    /// <summary>
    /// Create a new payout for a period.
    /// </summary>
    public static CommissionPayout Create(
        Guid tenantId,
        DateTime periodStart,
        DateTime periodEnd,
        List<Guid> commissionAccrualIds,
        decimal totalAmount)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required");
        if (totalAmount < 0)
            throw new DomainException("Total amount cannot be negative");

        var payout = new CommissionPayout
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PayoutPeriodStart = periodStart,
            PayoutPeriodEnd = periodEnd,
            CommissionAccrualIds = commissionAccrualIds,
            TotalAmount = totalAmount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        payout.RaiseDomainEvent(new CommissionPayoutCreatedEvent(
            payout.Id, payout.TenantId, payout.TotalAmount, payout.PayoutPeriodStart, payout.PayoutPeriodEnd));
        return payout;
    }

    /// <summary>
    /// Approve the payout.
    /// </summary>
    public void Approve()
    {
        if (Status != PayoutStatus.Pending)
            throw new DomainException("Only pending payouts can be approved");

        Status = PayoutStatus.Approved;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionPayoutApprovedEvent(Id, TenantId, TotalAmount));
    }

    /// <summary>
    /// Mark as processed (paid).
    /// </summary>
    public void MarkAsProcessed(string transactionId)
    {
        if (Status != PayoutStatus.Approved)
            throw new DomainException("Only approved payouts can be processed");

        Status = PayoutStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        TransactionId = transactionId;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionPayoutProcessedEvent(Id, TenantId, TotalAmount));
    }

    /// <summary>
    /// Mark as failed.
    /// </summary>
    public void MarkAsFailed()
    {
        Status = PayoutStatus.Failed;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CommissionPayoutFailedEvent(Id, TenantId, TotalAmount));
    }
}

/// <summary>
/// Payout status enumeration.
/// </summary>
public enum PayoutStatus
{
    Pending = 0,
    Approved = 1,
    Processed = 2,
    Failed = 3
}
