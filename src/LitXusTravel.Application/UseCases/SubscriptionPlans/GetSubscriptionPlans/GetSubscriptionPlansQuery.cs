using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.GetSubscriptionPlans;

public record GetSubscriptionPlansQuery : IRequest<Result<List<SubscriptionPlanResponse>>>;
