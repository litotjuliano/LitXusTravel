using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.StaffAgents.GetStaffAgents;

public class GetStaffAgentsQueryHandler : IRequestHandler<GetStaffAgentsQuery, Result<IEnumerable<StaffAgentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStaffAgentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<StaffAgentDto>>> Handle(GetStaffAgentsQuery request, CancellationToken ct)
    {
        var agents = request.ActiveOnly
            ? await _unitOfWork.StaffAgents.GetActiveByTenantAsync(request.TenantId, ct)
            : await _unitOfWork.StaffAgents.GetByTenantAsync(request.TenantId, ct);

        var dtos = agents.Select(a => new StaffAgentDto(
            a.Id,
            a.TenantId,
            a.Name,
            a.Email.Value,
            a.UniqueCode,
            a.CodeIssuedAt,
            a.CodeExpiresAt,
            a.IsActive,
            a.JoinedAt,
            a.DepartedAt));

        return Result<IEnumerable<StaffAgentDto>>.Success(dtos);
    }
}
