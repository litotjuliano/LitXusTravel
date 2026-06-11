namespace LitXusTravel.Application.DTOs.Response;

public record InquiryResponse(
    Guid Id,
    Guid TenantId,
    Guid? TenantPackageId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Message,
    int? NumberOfPax,
    string? PreferredTravelDates,
    string Status,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? QuotedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public record CrmActivityResponse(
    Guid Id,
    string Type,
    string? Notes,
    DateTimeOffset CreatedAt
);

public record InquiryStatsResponse(
    int New,
    int Contacted,
    int Quoted,
    int Booked,
    int Lost,
    double ConversionRate,
    int ThisMonth
);
