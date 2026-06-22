using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.CommissionAccruals.GetCommissionStatement;
using LitXusTravel.Application.UseCases.CommissionPayouts.ProcessCommissionPayout;

using LitXusTravel.API.Filters;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/commission-payouts")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
[Tags("Commission Payouts")]
public class CommissionPayoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommissionPayoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Process monthly commission payouts for a tenant.
    /// </summary>
    /// <remarks>
    /// This endpoint:
    /// 1. Finalizes all accrued commissions for the period
    /// 2. Groups commissions by agent
    /// 3. Creates a payout record
    /// 4. Marks commissions as pending payout
    /// 5. Logs an audit trail entry
    ///
    /// Important:
    /// - Only Tenant Admins can process payouts
    /// - Commissions must be in Finalized status (not Accrued)
    /// - A payout can only be created once per period per tenant
    /// - Safeguard 1: Only completed tours generate final commissions
    /// - Safeguard 8: Refund reversals are applied to next payout
    /// </remarks>
    /// <summary>Get commission statement for a staff agent</summary>
    [HttpGet("statement/{agentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatement(
        Guid tenantId, Guid agentId,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCommissionStatementQuery(agentId, from, to), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Process monthly commission payouts for a tenant</summary>
    [HttpPost("process")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayout(
        Guid tenantId,
        ProcessPayoutRequest request)
    {
        var command = new ProcessCommissionPayoutCommand(
            tenantId,
            request.PeriodStart,
            request.PeriodEnd);

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(null, new { id = result.Value });
    }
}

public record ProcessPayoutRequest(
    DateTime PeriodStart,
    DateTime PeriodEnd);
