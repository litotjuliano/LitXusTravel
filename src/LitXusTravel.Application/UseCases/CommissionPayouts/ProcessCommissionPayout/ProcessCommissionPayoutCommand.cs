using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionPayouts.ProcessCommissionPayout;

public record ProcessCommissionPayoutCommand(
    Guid TenantId,
    DateTime PeriodStart,
    DateTime PeriodEnd) : IRequest<Result<Guid>>;
