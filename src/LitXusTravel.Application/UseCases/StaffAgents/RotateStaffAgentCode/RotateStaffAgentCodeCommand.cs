using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.StaffAgents.RotateStaffAgentCode;

public record RotateStaffAgentCodeCommand(Guid TenantId, Guid AgentId)
    : IRequest<Result<string>>;
