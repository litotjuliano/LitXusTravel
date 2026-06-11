namespace LitXusTravel.Application.UseCases.StaffAgents.CreateStaffAgent;

public class CreateStaffAgentCommandHandler : IRequestHandler<CreateStaffAgentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public CreateStaffAgentCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateStaffAgentCommand request, CancellationToken ct)
    {
        // Check if email already exists for this tenant
        var existingAgent = await _unitOfWork.StaffAgents.GetByEmailAsync(
            new Email(request.Email), request.TenantId, ct);
        if (existingAgent != null)
            return Result.Failure($"Staff agent with email {request.Email} already exists for this tenant");

        try
        {
            var agent = StaffAgent.Create(request.TenantId, request.Name, new Email(request.Email));

            await _unitOfWork.StaffAgents.AddAsync(agent, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log audit trail
            await _auditService.LogActionAsync(
                action: AuditActions.CreateAdmin,
                affectedEntityType: nameof(StaffAgent),
                affectedEntityId: agent.Id,
                affectedTenantId: request.TenantId,
                reason: $"Created staff agent with code {agent.UniqueCode}",
                ct: ct);

            return Result.Success(agent.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
