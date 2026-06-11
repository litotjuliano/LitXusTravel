using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionPayouts.ProcessCommissionPayout;

public class ProcessCommissionPayoutCommandHandler : IRequestHandler<ProcessCommissionPayoutCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public ProcessCommissionPayoutCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(ProcessCommissionPayoutCommand request, CancellationToken ct)
    {
        try
        {
            // Get all finalized commissions for the period
            var accruals = await _unitOfWork.CommissionAccruals.GetFinalizedAsync(
                request.TenantId, request.PeriodStart, request.PeriodEnd, ct);

            if (!accruals.Any())
                return Result<Guid>.Failure($"No finalized commissions found for period {request.PeriodStart:yyyy-MM-dd} to {request.PeriodEnd:yyyy-MM-dd}");

            // Group by agent and calculate totals
            var commissionsByAgent = accruals
                .GroupBy(a => a.AgentId)
                .ToDictionary(g => g.Key, g => (Accruals: g.ToList(), Total: g.Sum(a => a.CommissionAmount)));

            var allAccrualIds = accruals.Select(a => a.Id).ToList();
            var totalAmount = commissionsByAgent.Values.Sum(x => x.Total);

            // Create payout record
            var payout = CommissionPayout.Create(
                request.TenantId,
                request.PeriodStart,
                request.PeriodEnd,
                allAccrualIds,
                totalAmount);

            await _unitOfWork.CommissionPayouts.AddAsync(payout, ct);

            // Mark all accruals as pending payout
            foreach (var accrual in accruals)
            {
                accrual.MarkPendingPayout();
            }

            await _unitOfWork.SaveChangesAsync(ct);

            // Log audit trail
            await _auditService.LogAsync(
                action: AuditActions.ProcessCommissionPayout,
                affectedEntityType: nameof(CommissionPayout),
                affectedEntityId: payout.Id,
                affectedTenantId: request.TenantId,
                reason: $"Processed payout for {commissionsByAgent.Count} agents, total: ${totalAmount:F2}",
                ct: ct);

            return Result<Guid>.Success(payout.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error processing payout: {ex.Message}");
        }
    }
}
