using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.StaffAgents.RotateStaffAgentCode;

public class RotateStaffAgentCodeCommandHandler : IRequestHandler<RotateStaffAgentCodeCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public RotateStaffAgentCodeCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<string>> Handle(RotateStaffAgentCodeCommand request, CancellationToken ct)
    {
        var agent = await _unitOfWork.StaffAgents.GetByIdAsync(request.AgentId, ct);
        if (agent is null)
            return Result<string>.Failure("Staff agent not found.");

        if (agent.TenantId != request.TenantId)
            return Result<string>.Failure("Staff agent does not belong to this tenant.");

        try
        {
            agent.RotateCode();
            _unitOfWork.StaffAgents.Update(agent);
            await _unitOfWork.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                action: AuditAction.Updated,
                entityType: nameof(StaffAgent),
                entityId: agent.Id,
                tenantId: request.TenantId,
                ct: ct);

            return Result<string>.Success(agent.UniqueCode);
        }
        catch (DomainException ex)
        {
            return Result<string>.Failure(ex.Message);
        }
    }
}
