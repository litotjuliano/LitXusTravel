using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.StaffAgents.GetStaffAgents;

public record GetStaffAgentsQuery(Guid TenantId, bool ActiveOnly = false)
    : IRequest<Result<IEnumerable<StaffAgentDto>>>;

public record StaffAgentDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string Email,
    string UniqueCode,
    DateTime CodeIssuedAt,
    DateTime? CodeExpiresAt,
    bool IsActive,
    DateTime JoinedAt,
    DateTime? DepartedAt);
