using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Public;

namespace LitXusTravel.API.Controllers.v1.Public;

[ApiController]
[Route("api/v1/public/websites/{subdomain}/packages")]
public class PackagesController(ISender mediator) : ControllerBase
{
    /// <summary>List packages for a public website (SPEC-PUBLIC-002)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(
        string subdomain,
        [FromQuery] string? category,
        [FromQuery] string? destination,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var query = new GetPublicPackagesQuery(subdomain, page, pageSize, destination, category, sortBy);
        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess) return NotFound(new { message = result.Errors.FirstOrDefault() });
        return Ok(result.Value);
    }

    /// <summary>Get resolved package detail for public website (SPEC-PUBLIC-003)</summary>
    [HttpGet("{packageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string subdomain, Guid packageId, CancellationToken ct)
    {
        // TODO: Implement GetPublicPackageDetailQuery
        return NotFound();
    }
}
