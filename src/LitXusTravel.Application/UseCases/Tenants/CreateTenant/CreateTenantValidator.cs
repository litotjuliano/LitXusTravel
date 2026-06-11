using FluentValidation;

namespace LitXusTravel.Application.UseCases.Tenants.CreateTenant;

public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(255).WithMessage("Company name must not exceed 255 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(255);

        RuleFor(x => x.Phone)
            .MaximumLength(20).When(x => x.Phone is not null);
    }
}
