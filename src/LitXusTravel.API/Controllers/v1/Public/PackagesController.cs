using Microsoft.AspNetCore.Mvc;

namespace LitXusTravel.API.Controllers.v1.Public;

[ApiController]
[Route("api/v1/public/websites/{subdomain}/packages")]
public class PackagesController() : ControllerBase
{
    /// <summary>List packages for a public website (SPEC-PUBLIC-002)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(
        string subdomain,
        [FromQuery] string? category,
        [FromQuery] string? destination,
        CancellationToken ct = default)
    {
        // TODO: Implement GetPublicPackagesQuery (resolve subdomain → tenant, merge overrides)
        return Ok(new { data = Array.Empty<object>() });
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
