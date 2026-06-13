using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionRules.GetCommissionRules;

public record GetCommissionRulesQuery(Guid TenantId, Guid? AgentId = null)
    : IRequest<Result<IEnumerable<CommissionRuleDto>>>;

public record CommissionRuleDto(
    Guid Id,
    Guid TenantId,
    Guid? AgentId,
    string RuleType,
    string Trigger,
    decimal Amount,
    bool IsPercentage,
    decimal MinimumThreshold,
    string PayoutFrequency,
    bool IsActive,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo);
