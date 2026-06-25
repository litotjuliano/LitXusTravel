using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.StaffAgents.CreateStaffAgent;
using LitXusTravel.Application.UseCases.StaffAgents.GetStaffAgents;
using LitXusTravel.Application.UseCases.StaffAgents.RotateStaffAgentCode;

using LitXusTravel.API.Filters;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/staff-agents")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
[SubscriptionWriteGuard]
[Tags("Staff Agents")]
public class StaffAgentsController(IMediator mediator) : ControllerBase
{
    /// <summary>List all staff agents for a tenant.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StaffAgentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStaffAgents(Guid tenantId, [FromQuery] bool activeOnly = false, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetStaffAgentsQuery(tenantId, activeOnly), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new staff agent for a tenant.
    /// Automatically generates a unique referral code (STAFF-{FirstName}-{Sequence}).
    /// Code expires after 1 month and must be rotated. Email must be unique within the tenant.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStaffAgent(Guid tenantId, CreateStaffAgentRequest request, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CreateStaffAgentCommand(tenantId, request.Name, request.Email), ct);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetStaffAgents), new { tenantId }, new { id = result.Value });
    }

    /// <summary>
    /// Rotate a staff agent's referral code. Generates a new code and resets the expiry to 1 month.
    /// </summary>
    [HttpPost("{agentId:guid}/rotate-code")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RotateCode(Guid tenantId, Guid agentId, CancellationToken ct = default)
    {
        var result = await mediator.Send(new RotateStaffAgentCodeCommand(tenantId, agentId), ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
                return NotFound(new { errors = result.Errors });
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { newCode = result.Value });
    }
}

public record CreateStaffAgentRequest(string Name, string Email);
