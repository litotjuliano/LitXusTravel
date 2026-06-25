using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.AdminUsers.GetAdminUsers;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, Result<IEnumerable<AdminUserListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<AdminUserListDto>>> Handle(GetAdminUsersQuery request, CancellationToken ct)
    {
        var admins = request.TenantId.HasValue
            ? await _unitOfWork.AdminUsers.GetTenantAdminsAsync(request.TenantId.Value, ct)
            : await _unitOfWork.AdminUsers.GetActiveAdminsAsync(ct);

        var dtos = admins.Select(a => new AdminUserListDto(
            a.Id,
            a.Name,
            a.Email.Value,
            a.Role.ToString(),
            a.Scope.ToString(),
            a.AssignedTenantId,
            a.IsActive,
            a.CreatedAt));

        return Result<IEnumerable<AdminUserListDto>>.Success(dtos);
    }
}
