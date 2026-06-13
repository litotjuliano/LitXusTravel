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
