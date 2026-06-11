namespace LitXusTravel.Application.UseCases.CommissionRules.ConfigureCommissionRule;

public record ConfigureCommissionRuleCommand(
    Guid TenantId,
    Guid? AgentId,
    CommissionTrigger Trigger,
    decimal Amount,
    bool IsPercentage,
    decimal MinimumThreshold = 100,
    string PayoutFrequency = "Monthly") : IRequest<Result<Guid>>;
