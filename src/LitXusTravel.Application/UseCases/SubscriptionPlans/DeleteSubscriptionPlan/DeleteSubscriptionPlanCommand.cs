using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.DeleteSubscriptionPlan;

public record DeleteSubscriptionPlanCommand(Guid Id) : IRequest<Result<string>>;
