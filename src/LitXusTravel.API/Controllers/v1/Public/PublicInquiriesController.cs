using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Inquiries.SubmitInquiry;
using LitXusTravel.Application.UseCases.Inquiries.UpdateInquiryStatus;
using LitXusTravel.Application.UseCases.Inquiries.AddInquiryActivity;

namespace LitXusTravel.API.Controllers.v1.Public;

[ApiController]
[Route("api/v1/public/tenants/{tenantId:guid}/inquiries")]
public class PublicInquiriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitInquiry(
        Guid tenantId,
        [FromBody] SubmitInquiryCommand command,
        CancellationToken ct)
    {
        var cmd = command with { TenantId = tenantId };
        var result = await mediator.Send(cmd, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return CreatedAtAction(nameof(SubmitInquiry), new { tenantId }, result.Value);
    }

    [HttpPut("{inquiryId:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid tenantId,
        Guid inquiryId,
        [FromBody] UpdateStatusRequest request,
        CancellationToken ct)
    {
        var command = new UpdateInquiryStatusCommand(tenantId, inquiryId, request.Status);
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    [HttpPost("{inquiryId:guid}/activity")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddActivity(
        Guid tenantId,
        Guid inquiryId,
        [FromBody] AddActivityRequest request,
        CancellationToken ct)
    {
        var command = new AddInquiryActivityCommand(tenantId, inquiryId, request.ActivityType, request.Description, request.Notes);
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }
}

public record UpdateStatusRequest(string Status);
public record AddActivityRequest(string ActivityType, string Description, string? Notes = null);
