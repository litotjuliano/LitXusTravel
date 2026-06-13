using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Events;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public class Booking : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid TourId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public DateTime BookingDate { get; private set; }
    public DateTime TourDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public BookingStatus Status { get; private set; }
    public string? ReferralCode { get; private set; }
    public Guid? AgentId { get; private set; }
    public string? CancellationReason { get; private set; }

    private Booking() { }

    public static Booking Create(
        Guid tenantId,
        Guid tourId,
        string customerName,
        string customerEmail,
        DateTime tourDate,
        decimal totalAmount,
        string? referralCode = null,
        Guid? agentId = null)
    {
        if (tenantId == Guid.Empty) throw new DomainException("Tenant ID is required");
        if (tourId == Guid.Empty) throw new DomainException("Tour ID is required");
        if (string.IsNullOrWhiteSpace(customerName)) throw new DomainException("Customer name is required");
        if (string.IsNullOrWhiteSpace(customerEmail)) throw new DomainException("Customer email is required");
        if (totalAmount < 0) throw new DomainException("Total amount cannot be negative");

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TourId = tourId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            BookingDate = DateTime.UtcNow,
            TourDate = tourDate,
            TotalAmount = totalAmount,
            Status = BookingStatus.Pending,
            ReferralCode = referralCode,
            AgentId = agentId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        booking.RaiseDomainEvent(new BookingCreatedEvent(booking.Id, booking.TenantId, booking.TourId, booking.CustomerEmail, booking.TotalAmount));
        return booking;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("Only pending bookings can be confirmed");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new BookingConfirmedEvent(Id, TenantId));
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException("Only confirmed bookings can be completed");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new BookingCompletedEvent(Id, TenantId, AgentId));
    }

    public void Cancel(string reason = "")
    {
        if (Status == BookingStatus.Completed)
            throw new DomainException("Cannot cancel a completed booking");
        if (Status == BookingStatus.Cancelled)
            throw new DomainException("Booking is already cancelled");

        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new BookingCancelledEvent(Id, TenantId, AgentId, reason));
    }

    /// <summary>Safeguard 2: Validate staff agent is not booking their own tour with their own code.</summary>
    public void ValidateSelfBookingPrevention(Guid currentAgentId)
    {
        if (AgentId.HasValue && AgentId.Value == currentAgentId)
            throw new DomainException("Cannot use own referral code for booking");
    }
}

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}
