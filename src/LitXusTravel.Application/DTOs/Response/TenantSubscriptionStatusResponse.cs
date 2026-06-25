namespace LitXusTravel.Application.DTOs.Response;

public record TenantSubscriptionStatusResponse(
    string PlanName,
    decimal MonthlyPrice,
    string Status,
    string SubscriptionHealth,
    DateTime StartDate,
    DateTime? EndDate,
    int? DaysRemaining,
    int MaxPackages,
    int MaxTeamMembers,
    bool AutoRenew,
    bool IsInGracePeriod,
    bool IsReadOnly,
    DateTime? GracePeriodEndsAt,
    int? GracePeriodDaysRemaining
);
