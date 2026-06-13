using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionRules.GetCommissionRules;

public class GetCommissionRulesQueryHandler : IRequestHandler<GetCommissionRulesQuery, Result<IEnumerable<CommissionRuleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommissionRulesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<CommissionRuleDto>>> Handle(GetCommissionRulesQuery request, CancellationToken ct)
    {
        var rules = request.AgentId.HasValue
            ? await _unitOfWork.CommissionRules.GetByAgentAsync(request.AgentId.Value, ct)
            : await _unitOfWork.CommissionRules.GetByTenantAsync(request.TenantId, ct);

        var dtos = rules.Select(r => new CommissionRuleDto(
            r.Id,
            r.TenantId,
            r.AgentId,
            r.AgentId.HasValue ? "AgentSpecific" : "Default",
            r.Trigger.ToString(),
            r.Amount,
            r.IsPercentage,
            r.MinimumThreshold,
            r.PayoutFrequency,
            r.IsActive,
            r.EffectiveFrom,
            r.EffectiveTo));

        return Result<IEnumerable<CommissionRuleDto>>.Success(dtos);
    }
}
