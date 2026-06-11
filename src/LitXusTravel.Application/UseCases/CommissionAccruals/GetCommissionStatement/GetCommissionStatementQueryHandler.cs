using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionAccruals.GetCommissionStatement;

public class GetCommissionStatementQueryHandler : IRequestHandler<GetCommissionStatementQuery, Result<CommissionStatementDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommissionStatementQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CommissionStatementDto>> Handle(GetCommissionStatementQuery request, CancellationToken ct)
    {
        var agent = await _unitOfWork.StaffAgents.GetByIdAsync(request.AgentId, ct);
        if (agent == null)
            return Result<CommissionStatementDto>.Failure($"Staff agent with ID {request.AgentId} not found");

        var periodStart = request.PeriodStart ?? DateTime.UtcNow.AddMonths(-1).AddDays(-(DateTime.UtcNow.Day - 1));
        var periodEnd = request.PeriodEnd ?? DateTime.UtcNow;

        var accruals = await _unitOfWork.CommissionAccruals.GetByAgentAsync(request.AgentId, ct);
        var periodAccruals = accruals
            .Where(a => a.AccruedAt >= periodStart && a.AccruedAt <= periodEnd)
            .ToList();

        var accrued = periodAccruals.Where(a => a.Status == CommissionStatus.Accrued).Sum(a => a.CommissionAmount);
        var finalized = periodAccruals.Where(a => a.Status == CommissionStatus.Finalized).Sum(a => a.CommissionAmount);
        var paid = periodAccruals.Where(a => a.Status == CommissionStatus.Paid).Sum(a => a.CommissionAmount);
        var reversed = periodAccruals.Where(a => a.Status == CommissionStatus.Reversed).Sum(a => a.CommissionAmount);

        var lineItems = periodAccruals
            .OrderByDescending(a => a.AccruedAt)
            .Select(a => new CommissionAccrualLineItemDto(
                a.Id,
                a.SourceId,
                a.CommissionAmount,
                a.CommissionPercentage,
                a.Status.ToString(),
                a.AccruedAt,
                $"Booking {a.SourceId.ToString().Substring(0, 8)}... - {a.Status}"))
            .ToList();

        var statement = new CommissionStatementDto(
            request.AgentId,
            agent.Name,
            periodStart,
            periodEnd,
            accrued,
            finalized,
            paid,
            reversed,
            periodAccruals.Count,
            periodAccruals.Count(a => a.Status == CommissionStatus.Finalized || a.Status == CommissionStatus.Paid),
            lineItems);

        return Result<CommissionStatementDto>.Success(statement);
    }
}
