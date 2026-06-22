namespace LitXusTravel.Application.DTOs.Response;

public record SubscriptionPlanResponse(
    Guid Id,
    string Name,
    decimal Price,
    int MaxPackages,
    int MaxTeamMembers,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int ActiveTenantCount = 0
);
