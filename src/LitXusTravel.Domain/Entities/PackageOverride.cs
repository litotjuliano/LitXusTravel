using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Stores tenant-specific overrides for a synced package.
/// NULL field = use master value. NOT NULL = use agent customization.
/// </summary>
public class PackageOverride : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid TenantPackageId { get; private set; }

    // All fields nullable — NULL means "inherit from master"
    public string? Title { get; private set; }
    public decimal? Price { get; private set; }
    public string? Currency { get; private set; }
    public string? FeaturedImageUrl { get; private set; }
    public string? ImagesJson { get; private set; }
    public string? Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? ContactWhatsapp { get; private set; }

    // Only populated for tenant-owned packages (no master to inherit from)
    public string? Destination { get; private set; }
    public int? DurationDays { get; private set; }
    public string? Category { get; private set; }
    public string? Region { get; private set; }

    public TenantPackage TenantPackage { get; private set; } = null!;

    private PackageOverride() { }

    public static PackageOverride CreateEmpty(Guid tenantId, Guid tenantPackageId)
        => new() { TenantId = tenantId, TenantPackageId = tenantPackageId };

    public static PackageOverride CreateForOwned(
        Guid tenantId, Guid tenantPackageId,
        string title, string destination, int durationDays, decimal price,
        string currency, string? category, string? region,
        string? description, string? shortDescription,
        string? featuredImageUrl, string? contactPhone, string? contactWhatsapp)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Package title is required.");
        if (string.IsNullOrWhiteSpace(destination)) throw new DomainException("Destination is required.");
        if (durationDays <= 0) throw new DomainException("Duration must be at least 1 day.");
        if (price <= 0) throw new DomainException("Price must be greater than zero.");

        return new()
        {
            TenantId = tenantId,
            TenantPackageId = tenantPackageId,
            Title = title,
            Destination = destination,
            DurationDays = durationDays,
            Price = price,
            Currency = currency,
            Category = category,
            Region = region,
            Description = description,
            ShortDescription = shortDescription,
            FeaturedImageUrl = featuredImageUrl,
            ContactPhone = contactPhone,
            ContactWhatsapp = contactWhatsapp,
        };
    }

    public void Update(
        string? title = null,
        decimal? price = null,
        string? currency = null,
        string? featuredImageUrl = null,
        string? imagesJson = null,
        string? description = null,
        string? shortDescription = null,
        string? contactPhone = null,
        string? contactWhatsapp = null)
    {
        if (price.HasValue && price.Value <= 0)
            throw new DomainException("Override price must be greater than zero.");

        if (title is not null) Title = title;
        if (price is not null) Price = price;
        if (currency is not null) Currency = currency?.ToUpperInvariant();
        if (featuredImageUrl is not null) FeaturedImageUrl = featuredImageUrl;
        if (imagesJson is not null) ImagesJson = imagesJson;
        if (description is not null) Description = description;
        if (shortDescription is not null) ShortDescription = shortDescription;
        if (contactPhone is not null) ContactPhone = contactPhone;
        if (contactWhatsapp is not null) ContactWhatsapp = contactWhatsapp;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool HasAnyOverride()
        => Title is not null || Price is not null || FeaturedImageUrl is not null
           || ImagesJson is not null || Description is not null || ContactPhone is not null
           || ContactWhatsapp is not null;
}
