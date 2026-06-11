namespace LitXusTravel.Application.DTOs.Response;

public record TenantResponse(
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
    DateTimeOffset CreatedAt
);
