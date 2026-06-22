using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.AssignPlan;

public record AssignPlanCommand(Guid TenantId, string PlanName) : IRequest<Result<string>>;
