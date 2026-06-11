using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Public;

namespace LitXusTravel.API.Controllers.v1.Public;

[ApiController]
[Route("api/v1/public")]
public class PublicPackagesController(IMediator mediator) : ControllerBase
{
    [HttpGet("metadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetadata([FromQuery] string? subdomain = null, CancellationToken ct = default)
    {
        var query = new GetWebsiteMetadataQuery(subdomain);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    [HttpGet("packages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPackages(
        [FromQuery] string? destination = null,
        [FromQuery] string? category = null,
        [FromQuery] string? sortBy = "title",
        [FromQuery] string? sortOrder = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetPublicPackagesQuery(page, pageSize, destination, category, sortBy, sortOrder);
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

    [HttpGet("packages/{packageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPackageDetail(Guid packageId, CancellationToken ct = default)
    {
        var query = new GetPublicPackageDetailQuery(packageId);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }
}
