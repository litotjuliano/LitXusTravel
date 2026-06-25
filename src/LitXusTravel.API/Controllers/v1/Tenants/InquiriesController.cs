using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.API.Filters;
using LitXusTravel.Application.UseCases.Inquiries.GetInquiries;
using LitXusTravel.Application.UseCases.Inquiries.GetInquiryDetail;
using LitXusTravel.Application.UseCases.Inquiries.GetInquiryStats;
using LitXusTravel.Application.UseCases.Inquiries.UpdateInquiryStatus;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/inquiries")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
[SubscriptionWriteGuard]
public class InquiriesController(IMediator mediator) : ControllerBase
{
    /// <summary>List inquiries (SPEC-TENANT-006)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        Guid tenantId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        if (!IsAuthorizedForTenant(tenantId)) return Forbid();

        var query = new GetInquiriesQuery(tenantId, status, page, pageSize);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });

        var list = result.Value!;
        return Ok(new {
            data = list.Items,
            pagination = new {
                page = list.Page,
                pageSize = list.PageSize,
                totalCount = list.TotalCount,
                totalPages = list.TotalPages,
                hasNextPage = list.HasNextPage,
                hasPreviousPage = list.HasPreviousPage
            }
        });
    }

    /// <summary>Get inquiry detail (SPEC-TENANT-007)</summary>
    [HttpGet("{inquiryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid tenantId, Guid inquiryId, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId)) return Forbid();

        var result = await mediator.Send(new GetInquiryDetailQuery(tenantId, inquiryId), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Update inquiry status (SPEC-TENANT-008)</summary>
    [HttpPut("{inquiryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid tenantId, Guid inquiryId,
        [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId)) return Forbid();

        var result = await mediator.Send(
            new UpdateInquiryStatusCommand(tenantId, inquiryId, request.Status), ct);

        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Get inquiry stats (SPEC-TENANT-010)</summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Stats(Guid tenantId, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId)) return Forbid();

        var result = await mediator.Send(new GetInquiryStatsQuery(), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });

        var stats = result.Value!;
        return Ok(new {
            statusBreakdown = stats.StatusBreakdown,
            totalInquiries = stats.TotalInquiries,
            conversionRate = stats.ConversionRate
        });
    }

    private bool IsAuthorizedForTenant(Guid tenantId)
    {
        var claimTenantId = User.FindFirst("tenantId")?.Value;
        return User.IsInRole("Admin")
               || (claimTenantId != null && Guid.Parse(claimTenantId) == tenantId);
    }
}

public record UpdateStatusRequest(string Status);
