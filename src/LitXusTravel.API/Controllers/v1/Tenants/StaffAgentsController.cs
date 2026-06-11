using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.StaffAgents.CreateStaffAgent;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/staff-agents")]
[Tags("Staff Agents")]
public class StaffAgentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StaffAgentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new staff agent for a tenant.
    /// Staff agents are internal employees who earn commissions on sales.
    /// </summary>
    /// <remarks>
    /// - Automatically generates a unique referral code (STAFF-{FirstName}-{Sequence})
    /// - Code expires after 1 month and must be rotated
    /// - Email must be unique within the tenant
    /// - Only Tenant Admins can create staff agents
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStaffAgent(
        Guid tenantId,
        CreateStaffAgentRequest request)
    {
        var command = new CreateStaffAgentCommand(tenantId, request.Name, request.Email);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(null, new { id = result.Value });
    }
}

public record CreateStaffAgentRequest(
    string Name,
    string Email);
