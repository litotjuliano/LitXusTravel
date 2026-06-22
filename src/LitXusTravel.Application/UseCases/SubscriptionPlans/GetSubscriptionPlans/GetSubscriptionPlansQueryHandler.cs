using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.GetSubscriptionPlans;

public class GetSubscriptionPlansQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetSubscriptionPlansQuery, Result<List<SubscriptionPlanResponse>>>
{
    public async Task<Result<List<SubscriptionPlanResponse>>> Handle(GetSubscriptionPlansQuery request, CancellationToken ct)
    {
        var plans = (await uow.SubscriptionPlans.GetAllAsync(ct))
            .OrderBy(p => p.Price)
            .ToList();

        var activeCountByPlan = (await uow.TenantSubscriptions.GetAllAsync(ct))
            .Where(s => s.IsActive)
            .GroupBy(s => s.PlanName)
            .ToDictionary(g => g.Key, g => g.Count());

        var responses = plans.Select(p => new SubscriptionPlanResponse(
            p.Id, p.Name, p.Price, p.MaxPackages, p.MaxTeamMembers, p.IsActive, p.CreatedAt,
            activeCountByPlan.GetValueOrDefault(p.Name, 0)
        )).ToList();

        return Result<List<SubscriptionPlanResponse>>.Success(responses);
    }
}
