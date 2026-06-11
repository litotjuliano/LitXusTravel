using FluentValidation;

namespace LitXusTravel.Application.UseCases.Inquiries.CreateInquiry;

public class CreateInquiryValidator : AbstractValidator<CreateInquiryCommand>
{
    public CreateInquiryValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Message).NotEmpty().MinimumLength(10)
            .WithMessage("Message must be at least 10 characters.");
        RuleFor(x => x.NumberOfPax).GreaterThan(0).When(x => x.NumberOfPax.HasValue);
    }
}
