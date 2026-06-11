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
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
