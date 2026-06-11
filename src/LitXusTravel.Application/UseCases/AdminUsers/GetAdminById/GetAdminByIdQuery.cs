namespace LitXusTravel.Application.UseCases.AdminUsers.GetAdminById;

public record GetAdminByIdQuery(Guid AdminId) : IRequest<Result<AdminUserDto>>;
