using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.API.Filters;
using LitXusTravel.Application.UseCases.Tenants.GetSubscriptionStatus;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/subscription")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
public class SubscriptionController(IMediator mediator) : ControllerBase
{
    /// <summary>Get the current subscription status for this tenant</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(Guid tenantId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSubscriptionStatusQuery(tenantId), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }
}
