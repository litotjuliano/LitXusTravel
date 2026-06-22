using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.SubscriptionPlans.CreateSubscriptionPlan;
using LitXusTravel.Application.UseCases.SubscriptionPlans.UpdateSubscriptionPlan;
using LitXusTravel.Application.UseCases.SubscriptionPlans.DeleteSubscriptionPlan;
using LitXusTravel.Application.UseCases.SubscriptionPlans.GetSubscriptionPlans;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/subscription-plans")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class SubscriptionPlansController(IMediator mediator) : ControllerBase
{
    /// <summary>List all subscription plans</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSubscriptionPlansQuery(), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(new { data = result.Value });
    }

    /// <summary>Create a new subscription plan</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionPlanCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return CreatedAtAction(nameof(List), result.Value);
    }

    /// <summary>Update a subscription plan</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubscriptionPlanCommand command, CancellationToken ct)
    {
        var cmd = command with { Id = id };
        var result = await mediator.Send(cmd, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Delete a subscription plan</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteSubscriptionPlanCommand(id), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(new { message = result.Value });
    }
}
