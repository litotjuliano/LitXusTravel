namespace LitXusTravel.Application.DTOs.Response;

public record PackageListResponse(
    Guid Id,
    string Title,
    string? Category,
    string Destination,
    decimal BasePrice,
    string Currency,
    int DurationDays,
    string Visibility,
    int syncedTenantsCount,
    IReadOnlyList<string> Tenants,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
