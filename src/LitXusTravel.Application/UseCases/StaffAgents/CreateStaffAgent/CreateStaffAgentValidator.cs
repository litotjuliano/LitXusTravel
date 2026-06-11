namespace LitXusTravel.Application.UseCases.StaffAgents.CreateStaffAgent;

public class CreateStaffAgentValidator : AbstractValidator<CreateStaffAgentCommand>
{
    public CreateStaffAgentValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Staff agent name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
