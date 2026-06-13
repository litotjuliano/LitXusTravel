using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.DomainEvents;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public enum PackageVisibility { Draft, Published, Archived }

public class Package : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string? Subtitle { get; private set; }
    public string? Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? Category { get; private set; }
    public decimal BasePrice { get; private set; }
    public string Currency { get; private set; } = "MYR";
    public int DurationDays { get; private set; }
    public string Destination { get; private set; } = string.Empty;
    public string? Region { get; private set; }
    public string? HighlightsJson { get; private set; }
    public string? ItineraryJson { get; private set; }
    public string? InclusionsJson { get; private set; }
    public string? ExclusionsJson { get; private set; }
    public string? ImagesJson { get; private set; }
    public string? FeaturedImageUrl { get; private set; }
    public string? VideoUrl { get; private set; }
    public decimal? Rating { get; private set; }
    public int ReviewCount { get; private set; }
    public PackageVisibility Visibility { get; private set; } = PackageVisibility.Draft;
    public bool IsPopular { get; private set; }
    public bool IsFeatured { get; private set; }
    public int? MaxGroupSize { get; private set; }
    public int MinGroupSize { get; private set; } = 2;
    public Guid? CreatedById { get; private set; }
    // Null = created by platform admin; non-null = tenant extended this package to the master catalog
    public Guid? CreatedByTenantId { get; private set; }

    public ICollection<TenantPackage> TenantPackages { get; private set; } = [];

    private Package() { }

    public static Package Create(
        string title, string destination, decimal basePrice, int durationDays,
        string? category = null, string? description = null, string? currency = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Package title is required.");

        if (string.IsNullOrWhiteSpace(destination))
            throw new DomainException("Package destination is required.");

        if (basePrice <= 0)
            throw new DomainException("Base price must be greater than zero.");

        if (durationDays <= 0)
            throw new DomainException("Duration must be at least 1 day.");

        return new Package
        {
            Title = title.Trim(),
            Destination = destination.Trim(),
            BasePrice = basePrice,
            DurationDays = durationDays,
            Category = category,
            Description = description,
            Currency = currency?.ToUpperInvariant() ?? "MYR",
            Visibility = PackageVisibility.Draft
        };
    }

    public void Publish()
    {
        if (Visibility != PackageVisibility.Draft)
            throw new DomainException("Only draft packages can be published.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new DomainException("Package must have a description before publishing.");

        Visibility = PackageVisibility.Published;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new PackagePublishedEvent(Id, Title));
    }

    public void Archive()
    {
        if (Visibility == PackageVisibility.Archived)
            throw new DomainException("Package is already archived.");

        Visibility = PackageVisibility.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetFeaturedImage(string? url)
    {
        FeaturedImageUrl = url;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDetails(string title, string? description, string? shortDescription,
        string? category, decimal basePrice, int durationDays, string destination,
        string? region, string? itineraryJson, string? highlightsJson,
        string? inclusionsJson, string? exclusionsJson, string? imagesJson,
        string? featuredImageUrl, int? maxGroupSize)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Package title is required.");

        if (basePrice <= 0)
            throw new DomainException("Base price must be greater than zero.");

        Title = title.Trim();
        Description = description;
        ShortDescription = shortDescription;
        Category = category;
        BasePrice = basePrice;
        DurationDays = durationDays;
        Destination = destination;
        Region = region;
        ItineraryJson = itineraryJson;
        HighlightsJson = highlightsJson;
        InclusionsJson = inclusionsJson;
        ExclusionsJson = exclusionsJson;
        ImagesJson = imagesJson;
        FeaturedImageUrl = featuredImageUrl;
        MaxGroupSize = maxGroupSize;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCreatedByTenant(Guid tenantId)
    {
        CreatedByTenantId = tenantId;
    }

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetPopular(bool isPopular)
    {
        IsPopular = isPopular;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
