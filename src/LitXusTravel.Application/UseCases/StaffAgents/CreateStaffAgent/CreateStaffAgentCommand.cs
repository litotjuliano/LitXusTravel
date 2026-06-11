namespace LitXusTravel.Application.UseCases.StaffAgents.CreateStaffAgent;

public record CreateStaffAgentCommand(
    Guid TenantId,
    string Name,
    string Email) : IRequest<Result<Guid>>;
