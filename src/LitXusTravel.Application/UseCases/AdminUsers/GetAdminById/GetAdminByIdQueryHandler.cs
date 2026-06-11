namespace LitXusTravel.Application.UseCases.AdminUsers.GetAdminById;

public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, Result<AdminUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AdminUserDto>> Handle(GetAdminByIdQuery request, CancellationToken ct)
    {
        var admin = await _unitOfWork.AdminUsers.GetByIdAsync(request.AdminId, ct);
        if (admin == null)
            return Result.Failure($"Admin user with ID {request.AdminId} not found");

        var dto = new AdminUserDto(
            admin.Id,
            admin.Name,
            admin.Email.Value,
            admin.Role.ToString(),
            admin.Scope.ToString(),
            admin.AssignedTenantId,
            admin.IsActive,
            admin.CreatedAt);

        return Result.Success(dto);
    }
}
