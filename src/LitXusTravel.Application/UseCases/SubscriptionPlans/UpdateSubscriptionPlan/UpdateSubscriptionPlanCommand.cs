using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.UpdateSubscriptionPlan;

public record UpdateSubscriptionPlanCommand(
    Guid Id,
    string Name,
    decimal Price,
    int MaxPackages,
    int MaxTeamMembers) : IRequest<Result<SubscriptionPlanResponse>>;
