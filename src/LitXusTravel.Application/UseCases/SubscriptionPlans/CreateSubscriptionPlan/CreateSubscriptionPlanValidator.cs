using FluentValidation;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.CreateSubscriptionPlan;

public class CreateSubscriptionPlanValidator : AbstractValidator<CreateSubscriptionPlanCommand>
{
    public CreateSubscriptionPlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required")
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative");

        RuleFor(x => x.MaxPackages)
            .GreaterThan(0).WithMessage("Max packages must be greater than 0");

        RuleFor(x => x.MaxTeamMembers)
            .GreaterThan(0).WithMessage("Max team members must be greater than 0");
    }
}
