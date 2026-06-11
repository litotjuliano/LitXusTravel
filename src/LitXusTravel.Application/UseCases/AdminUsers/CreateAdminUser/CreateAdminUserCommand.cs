namespace LitXusTravel.Application.UseCases.AdminUsers.CreateAdminUser;

public record CreateAdminUserCommand(
    string Name,
    string Email,
    AdminRole Role,
    AdminScope Scope,
    Guid? AssignedTenantId = null) : IRequest<Result<Guid>>;
