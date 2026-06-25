using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Tenants.GetSubscriptionStatus;

public class GetSubscriptionStatusQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetSubscriptionStatusQuery, Result<TenantSubscriptionStatusResponse>>
{
    public async Task<Result<TenantSubscriptionStatusResponse>> Handle(
        GetSubscriptionStatusQuery request, CancellationToken ct)
    {
        var sub = (await uow.TenantSubscriptions.FindAsync(
                s => s.TenantId == request.TenantId, ct))
            .OrderByDescending(s => s.IsActive)
            .ThenByDescending(s => s.StartDate)
            .FirstOrDefault();

        if (sub is null)
            return Result<TenantSubscriptionStatusResponse>.Failure("No subscription found.");

        return Result<TenantSubscriptionStatusResponse>.Success(new TenantSubscriptionStatusResponse(
            sub.PlanName,
            sub.MonthlyPrice,
            sub.Status.ToString(),
            sub.SubscriptionHealth,
            sub.StartDate,
            sub.EndDate,
            sub.DaysRemaining,
            sub.MaxPackages,
            sub.MaxTeamMembers,
            sub.AutoRenew,
            sub.IsInGracePeriod,
            sub.IsReadOnly,
            sub.GracePeriodEndsAt,
            sub.GracePeriodDaysRemaining));
    }
}
