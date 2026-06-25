using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.DomainEvents;
using LitXusTravel.Domain.Exceptions;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Domain.Entities;

public enum ProvisioningStatus { Pending, Completed, Failed }

public class Tenant : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Subdomain { get; private set; }
    public Email ContactEmail { get; private set; } = null!;
    public string? ContactPhone { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? BrandColorsJson { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ProvisioningStatus ProvisioningStatus { get; private set; } = ProvisioningStatus.Pending;
    public string? WebsiteUrl { get; private set; }
    public string? Country { get; private set; }
    public string DefaultCurrency { get; private set; } = "MYR";

    public ICollection<TenantSubscription> Subscriptions { get; private set; } = [];
    public ICollection<TenantPackage> TenantPackages { get; private set; } = [];
    public ICollection<Inquiry> Inquiries { get; private set; } = [];

    private Tenant() { }

    public static Tenant Create(string name, string email, string? phone = null, string? country = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tenant name is required.");

        var tenant = new Tenant
        {
            Name = name.Trim(),
            Slug = GenerateSlug(name),
            ContactEmail = new Email(email),
            ContactPhone = phone,
            Country = country,
            ProvisioningStatus = ProvisioningStatus.Pending
        };

        tenant.RaiseDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.Name, email));
        return tenant;
    }

    public void AssignSubdomain(string subdomain)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
            throw new DomainException("Subdomain cannot be empty.");

        Subdomain = subdomain.ToLowerInvariant();
        WebsiteUrl = $"https://{Subdomain}.litxustravel.com";
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkProvisioningComplete()
    {
        ProvisioningStatus = ProvisioningStatus.Completed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkProvisioningFailed()
    {
        ProvisioningStatus = ProvisioningStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateSettings(string? defaultCurrency)
    {
        if (!string.IsNullOrWhiteSpace(defaultCurrency))
            DefaultCurrency = defaultCurrency.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateBranding(string? logoUrl, string? brandColorsJson)
    {
        LogoUrl = logoUrl;
        BrandColorsJson = brandColorsJson;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string GenerateSlug(string name)
        => System.Text.RegularExpressions.Regex
            .Replace(name.ToLowerInvariant().Trim(), @"[^a-z0-9]+", "-")
            .Trim('-');
}
