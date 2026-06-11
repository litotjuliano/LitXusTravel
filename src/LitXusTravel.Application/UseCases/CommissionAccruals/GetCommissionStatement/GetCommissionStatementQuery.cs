using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionAccruals.GetCommissionStatement;

public record GetCommissionStatementQuery(
    Guid AgentId,
    DateTime? PeriodStart = null,
    DateTime? PeriodEnd = null) : IRequest<Result<CommissionStatementDto>>;
