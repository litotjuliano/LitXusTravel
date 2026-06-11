namespace LitXusTravel.Application.DTOs.Response;

public record TenantListResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Subdomain,
    string ContactEmail,
    string? ContactPhone,
    string? LogoUrl,
    bool IsActive,
    string ProvisioningStatus,
    string? WebsiteUrl,
    int syncedPackagesCount,
    int totalInquiries,
    double conversionRate,
    DateTimeOffset CreatedAt
);
