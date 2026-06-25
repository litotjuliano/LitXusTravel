using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Billing.GetInvoices;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/billing")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class BillingController(IMediator mediator) : ControllerBase
{
    [HttpGet("invoices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoices([FromQuery] Guid? tenantId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetInvoicesQuery(tenantId), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }
}
