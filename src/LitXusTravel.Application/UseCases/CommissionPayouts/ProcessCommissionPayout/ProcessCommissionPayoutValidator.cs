namespace LitXusTravel.Application.UseCases.CommissionPayouts.ProcessCommissionPayout;

public class ProcessCommissionPayoutValidator : AbstractValidator<ProcessCommissionPayoutCommand>
{
    public ProcessCommissionPayoutValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.PeriodStart)
            .NotEmpty().WithMessage("Period start date is required");

        RuleFor(x => x.PeriodEnd)
            .NotEmpty().WithMessage("Period end date is required")
            .GreaterThan(x => x.PeriodStart).WithMessage("Period end must be after period start");
    }
}
