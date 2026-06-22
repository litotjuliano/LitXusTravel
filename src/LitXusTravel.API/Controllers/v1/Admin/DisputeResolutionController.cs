using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.DisputeResolution.CreateDisputeTicket;
using LitXusTravel.Application.UseCases.DisputeResolution.ReviewDisputeTicket;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/disputes")]
[Authorize(Roles = "SuperAdmin,Admin")]
[Tags("Dispute Resolution")]
public class DisputeResolutionController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// SuperAdmin creates a dispute ticket for a commission accrual.
    /// Safeguard 10: Structured workflow instead of direct override.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDispute(CreateDisputeRequest request, CancellationToken ct = default)
    {
        var command = new CreateDisputeTicketCommand(
            request.SuperAdminId,
            request.CommissionAccrualId,
            request.Description,
            request.ProposedFix,
            Enum.Parse<DisputeReasonCode>(request.ReasonCode),
            request.OriginalAmount);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(ReviewDispute), new { ticketId = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Tenant Admin approves or rejects a dispute ticket.
    /// Approve requires AdjustedAmount. Reject leaves original commission unchanged.
    /// </summary>
    [HttpPost("{ticketId:guid}/review")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewDispute(Guid ticketId, ReviewDisputeRequest request, CancellationToken ct = default)
    {
        var command = new ReviewDisputeTicketCommand(
            ticketId,
            request.TenantAdminId,
            request.IsApproved,
            request.AdjustedAmount);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
                return NotFound(new { errors = result.Errors });
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = request.IsApproved ? "Dispute approved." : "Dispute rejected." });
    }
}

public record CreateDisputeRequest(
    Guid SuperAdminId,
    Guid CommissionAccrualId,
    string Description,
    string ProposedFix,
    string ReasonCode,
    decimal OriginalAmount);

public record ReviewDisputeRequest(
    Guid TenantAdminId,
    bool IsApproved,
    decimal? AdjustedAmount = null);
