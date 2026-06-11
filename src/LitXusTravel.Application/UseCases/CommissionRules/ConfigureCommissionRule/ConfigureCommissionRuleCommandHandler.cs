using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.CommissionRules.ConfigureCommissionRule;

public class ConfigureCommissionRuleCommandHandler : IRequestHandler<ConfigureCommissionRuleCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public ConfigureCommissionRuleCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(ConfigureCommissionRuleCommand request, CancellationToken ct)
    {
        try
        {
            CommissionRule rule = request.AgentId.HasValue
                ? CommissionRule.CreateForAgent(
                    request.TenantId,
                    request.AgentId.Value,
                    request.Trigger,
                    request.Amount,
                    request.IsPercentage,
                    request.MinimumThreshold)
                : CommissionRule.CreateDefault(
                    request.TenantId,
                    request.Trigger,
                    request.Amount,
                    request.IsPercentage,
                    request.MinimumThreshold);

            await _unitOfWork.CommissionRules.AddAsync(rule, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log audit trail
            var ruleType = request.AgentId.HasValue ? "Agent-specific" : "Default";
            await _auditService.LogAsync(
                action: AuditAction.CreateCommissionRule,
                affectedEntityType: nameof(CommissionRule),
                affectedEntityId: rule.Id,
                affectedTenantId: request.TenantId,
                affectedAgentId: request.AgentId,
                reason: $"Configured {ruleType} commission rule: {(request.IsPercentage ? request.Amount + "%" : "$" + request.Amount)}",
                ct: ct);

            return Result<Guid>.Success(rule.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
