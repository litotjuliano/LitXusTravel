using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.AdminUsers.GetAdminUsers;

public record GetAdminUsersQuery(Guid? TenantId = null, bool ActiveOnly = false)
    : IRequest<Result<IEnumerable<AdminUserListDto>>>;

public record AdminUserListDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Scope,
    Guid? AssignedTenantId,
    bool IsActive,
    DateTimeOffset CreatedAt);
