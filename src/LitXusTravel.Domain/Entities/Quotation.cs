using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public enum QuotationStatus { Draft, Sent, Accepted, Rejected, Expired }

public class Quotation : BaseEntity
{
    public Guid InquiryId { get; private set; }
    public Guid TenantId { get; private set; }
    public string PackageTitle { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public decimal Markup { get; private set; }
    public decimal FinalPrice { get; private set; }
    public string Currency { get; private set; } = "MYR";
    public int NumberOfTravelers { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string? CustomNotes { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public QuotationStatus Status { get; private set; } = QuotationStatus.Draft;
    public DateTimeOffset? SentAt { get; private set; }

    public Inquiry Inquiry { get; private set; } = null!;

    private Quotation() { }

    public static Quotation Create(
        Guid inquiryId, Guid tenantId, string packageTitle,
        decimal basePrice, decimal markup, int numberOfTravelers,
        string currency = "MYR", string? customNotes = null,
        DateTime? validUntil = null)
    {
        if (string.IsNullOrWhiteSpace(packageTitle))
            throw new DomainException("Package title is required for quotation.");

        if (basePrice <= 0)
            throw new DomainException("Base price must be greater than zero.");

        if (numberOfTravelers <= 0)
            throw new DomainException("Number of travelers must be at least 1.");

        var finalPrice = basePrice + markup;

        return new Quotation
        {
            InquiryId = inquiryId,
            TenantId = tenantId,
            PackageTitle = packageTitle,
            BasePrice = basePrice,
            Markup = markup,
            FinalPrice = finalPrice,
            Currency = currency.ToUpperInvariant(),
            NumberOfTravelers = numberOfTravelers,
            TotalPrice = finalPrice * numberOfTravelers,
            CustomNotes = customNotes,
            ValidUntil = validUntil,
            Status = QuotationStatus.Draft
        };
    }

    public void MarkSent()
    {
        if (Status != QuotationStatus.Draft)
            throw new DomainException("Only draft quotations can be sent.");

        Status = QuotationStatus.Sent;
        SentAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Accept()
    {
        if (Status != QuotationStatus.Sent)
            throw new DomainException("Only sent quotations can be accepted.");

        Status = QuotationStatus.Accepted;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Reject()
    {
        if (Status != QuotationStatus.Sent)
            throw new DomainException("Only sent quotations can be rejected.");

        Status = QuotationStatus.Rejected;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
