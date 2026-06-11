using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Inquiries.GetInquiries;
using LitXusTravel.Application.UseCases.Inquiries.GetInquiryDetail;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/inquiries")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class InquiriesController(IMediator mediator) : ControllerBase
{
    /// <summary>Get aggregated inquiry stats across all tenants (SPEC-ADMIN-010)</summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var query = new GetInquiriesQuery(null, null, 1, 1000);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });

        var inquiries = result.Value?.Items ?? [];

        var stats = new
        {
            statusBreakdown = new
            {
                New = inquiries.Count(i => i.Status == "New"),
                Contacted = inquiries.Count(i => i.Status == "Contacted"),
                Quoted = inquiries.Count(i => i.Status == "Quoted"),
                Booked = inquiries.Count(i => i.Status == "Booked"),
                Lost = inquiries.Count(i => i.Status == "Lost")
            },
            totalInquiries = inquiries.Count(),
            conversionRate = inquiries.Count() > 0
                ? (inquiries.Count(i => i.Status == "Booked") / (double)inquiries.Count()) * 100
                : 0.0
        };

        return Ok(stats);
    }

    /// <summary>Get inquiry detail by id</summary>
    [HttpGet("{tenantId:guid}/{inquiryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid tenantId, Guid inquiryId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetInquiryDetailQuery(tenantId, inquiryId), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }
}
