using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.ProcessMockPayment;

public record ProcessMockPaymentCommand(Guid TenantId, string PlanName) : IRequest<Result<string>>;
