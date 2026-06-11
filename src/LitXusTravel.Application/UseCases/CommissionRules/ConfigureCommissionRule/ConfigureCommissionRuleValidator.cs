using FluentValidation;

namespace LitXusTravel.Application.UseCases.CommissionRules.ConfigureCommissionRule;

public class ConfigureCommissionRuleValidator : AbstractValidator<ConfigureCommissionRuleCommand>
{
    public ConfigureCommissionRuleValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Trigger)
            .IsInEnum().WithMessage("Invalid commission trigger");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Commission amount must be greater than 0");

        RuleFor(x => x.Amount)
            .LessThanOrEqualTo(30).WithMessage("Commission percentage cannot exceed 30% (system maximum)")
            .When(x => x.IsPercentage)
            .WithName("Commission Percentage Cap (Safeguard 9)");

        RuleFor(x => x.Amount)
            .LessThanOrEqualTo(100).WithMessage("Commission percentage cannot exceed 100%")
            .When(x => x.IsPercentage);

        RuleFor(x => x.MinimumThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum threshold cannot be negative");

        RuleFor(x => x.PayoutFrequency)
            .NotEmpty().WithMessage("Payout frequency is required")
            .Must(x => x == "Daily" || x == "Weekly" || x == "Monthly")
            .WithMessage("Payout frequency must be Daily, Weekly, or Monthly");
    }
}
