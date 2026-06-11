using FluentValidation;

namespace LitXusTravel.Application.UseCases.Packages.CreatePackage;

public class CreatePackageValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(255);
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("Base price must be greater than zero.");
        RuleFor(x => x.DurationDays).GreaterThan(0).WithMessage("Duration must be at least 1 day.");
        RuleFor(x => x.Currency).Length(3).When(x => x.Currency is not null)
            .WithMessage("Currency must be a 3-letter ISO code.");
    }
}
