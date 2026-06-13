using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Events;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public class Tour : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? TenantPackageId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int MaxCapacity { get; private set; }
    public int CurrentBookings { get; private set; }
    public decimal BasePrice { get; private set; }
    public string Currency { get; private set; } = "MYR";
    public TourStatus Status { get; private set; }

    private Tour() { }

    public static Tour Create(
        Guid tenantId,
        string title,
        string destination,
        DateTime startDate,
        DateTime endDate,
        int maxCapacity,
        decimal basePrice,
        string currency = "MYR",
        Guid? tenantPackageId = null)
    {
        if (tenantId == Guid.Empty) throw new DomainException("Tenant ID is required");
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Tour title is required");
        if (startDate >= endDate) throw new DomainException("Start date must be before end date");
        if (maxCapacity <= 0) throw new DomainException("Max capacity must be greater than 0");
        if (basePrice < 0) throw new DomainException("Base price cannot be negative");

        var tour = new Tour
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TenantPackageId = tenantPackageId,
            Title = title,
            Destination = destination,
            StartDate = startDate,
            EndDate = endDate,
            MaxCapacity = maxCapacity,
            CurrentBookings = 0,
            BasePrice = basePrice,
            Currency = currency,
            Status = TourStatus.Scheduled,
            CreatedAt = DateTimeOffset.UtcNow
        };

        tour.RaiseDomainEvent(new TourCreatedEvent(tour.Id, tour.TenantId, tour.Title));
        return tour;
    }

    public void AddBooking()
    {
        if (Status != TourStatus.Scheduled)
            throw new DomainException("Cannot add booking to a tour that is not scheduled");
        if (CurrentBookings >= MaxCapacity)
            throw new DomainException("Tour has reached maximum capacity");

        CurrentBookings++;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveBooking()
    {
        if (CurrentBookings > 0)
        {
            CurrentBookings--;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    public void Complete()
    {
        if (Status == TourStatus.Completed)
            throw new DomainException("Tour is already completed");
        if (Status == TourStatus.Cancelled)
            throw new DomainException("Cannot complete a cancelled tour");

        Status = TourStatus.Completed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new TourCompletedEvent(Id, TenantId));
    }

    public void Cancel(string reason = "")
    {
        if (Status == TourStatus.Completed)
            throw new DomainException("Cannot cancel a completed tour");
        if (Status == TourStatus.Cancelled)
            throw new DomainException("Tour is already cancelled");

        Status = TourStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new TourCancelledEvent(Id, TenantId, reason));
    }

    public bool HasCapacity() => Status == TourStatus.Scheduled && CurrentBookings < MaxCapacity;
}

public enum TourStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}
