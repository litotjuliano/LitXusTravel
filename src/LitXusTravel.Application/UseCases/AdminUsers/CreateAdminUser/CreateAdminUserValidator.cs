using FluentValidation;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.AdminUsers.CreateAdminUser;

public class CreateAdminUserValidator : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Admin name is required")
            .MaximumLength(100).WithMessage("Admin name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");

        RuleFor(x => x.Scope)
            .IsInEnum().WithMessage("Invalid scope");

        RuleFor(x => x.AssignedTenantId)
            .NotEmpty().WithMessage("Tenant ID is required for Tenant-scoped admins")
            .When(x => x.Scope == AdminScope.Tenant);

        RuleFor(x => x.AssignedTenantId)
            .Empty().WithMessage("Tenant ID should not be set for Platform-scoped admins")
            .When(x => x.Scope == AdminScope.Platform);
    }
}
