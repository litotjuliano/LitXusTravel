using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.DomainEvents;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public enum InquiryStatus { New, Contacted, Quoted, Booked, Lost }

public class Inquiry : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? TenantPackageId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public int? NumberOfPax { get; private set; }
    public string? PreferredTravelDates { get; private set; }
    public InquiryStatus Status { get; private set; } = InquiryStatus.New;
    public Guid? AssignedToUserId { get; private set; }
    public string? WhatsAppGroupUrl { get; private set; }
    public DateTimeOffset? FirstResponseAt { get; private set; }
    public DateTimeOffset? QuotedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    public TenantPackage? TenantPackage { get; private set; }
    public ICollection<CrmActivity> Activities { get; private set; } = [];
    public ICollection<Quotation> Quotations { get; private set; } = [];

    private Inquiry() { }

    public static Inquiry Create(
        Guid tenantId, string customerName, string customerEmail,
        string customerPhone, string message,
        Guid? tenantPackageId = null, int? numberOfPax = null,
        string? preferredTravelDates = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer name is required.");

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new DomainException("Customer email is required.");

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new DomainException("Customer phone is required.");

        if (string.IsNullOrWhiteSpace(message) || message.Trim().Length < 10)
            throw new DomainException("Message must be at least 10 characters.");

        var inquiry = new Inquiry
        {
            TenantId = tenantId,
            TenantPackageId = tenantPackageId,
            CustomerName = customerName.Trim(),
            CustomerEmail = customerEmail.Trim().ToLowerInvariant(),
            CustomerPhone = customerPhone.Trim(),
            Message = message.Trim(),
            NumberOfPax = numberOfPax,
            PreferredTravelDates = preferredTravelDates,
            Status = InquiryStatus.New
        };

        inquiry.RaiseDomainEvent(new InquiryReceivedEvent(inquiry.Id, tenantId, customerName, customerEmail));
        return inquiry;
    }

    public void UpdateStatus(InquiryStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;

        if (newStatus == InquiryStatus.Contacted && FirstResponseAt == null)
            FirstResponseAt = DateTimeOffset.UtcNow;
        else if (newStatus == InquiryStatus.Quoted && QuotedAt == null)
            QuotedAt = DateTimeOffset.UtcNow;
        else if (newStatus is InquiryStatus.Booked or InquiryStatus.Lost)
            ClosedAt = DateTimeOffset.UtcNow;
    }

    public void AssignTo(Guid userId)
    {
        AssignedToUserId = userId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
