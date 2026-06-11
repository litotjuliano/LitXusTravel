using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Packages.GetTenantPackages;
using LitXusTravel.Application.UseCases.Packages.SyncPackageToTenant;
using LitXusTravel.Application.UseCases.Packages.UnsyncPackage;
using LitXusTravel.Application.UseCases.Packages.UpdatePackageOverride;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/packages")]
[Authorize(Roles = "Agent,Admin")]
public class MyPackagesController(IMediator mediator) : ControllerBase
{
    /// <summary>Sync master packages to tenant (SPEC-TENANT-001)</summary>
    [HttpPost("sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Sync(Guid tenantId, [FromBody] SyncRequest request, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var result = await mediator.Send(new SyncPackagesCommand(tenantId, request.MasterPackageIds), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Get synced packages (SPEC-TENANT-002)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "syncedAt",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken ct = default)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var query = new GetTenantPackagesQuery(tenantId, page, pageSize, sortBy, sortOrder);
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

    /// <summary>Update package override (SPEC-TENANT-004)</summary>
    [HttpPut("{packageId:guid}/override")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOverride(Guid tenantId, Guid packageId,
        [FromBody] UpdateOverrideRequest request, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var command = new UpdatePackageOverrideCommand(
            tenantId, packageId,
            request.Title, request.Price, request.Currency,
            request.FeaturedImageUrl, request.ImagesJson,
            request.Description, request.ShortDescription,
            request.ContactPhone, request.ContactWhatsapp);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Unsync a package (SPEC-TENANT-005)</summary>
    [HttpDelete("{packageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unsync(Guid tenantId, Guid packageId, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var result = await mediator.Send(new UnsyncPackageCommand(tenantId, packageId), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(new { message = result.Value });
    }

    private bool IsAuthorizedForTenant(Guid tenantId)
    {
        var claimTenantId = User.FindFirst("tenantId")?.Value;
        return User.IsInRole("Admin")
               || (claimTenantId != null && Guid.Parse(claimTenantId) == tenantId);
    }
}

public record SyncRequest(IReadOnlyList<Guid> MasterPackageIds);

public record UpdateOverrideRequest(
    string? Title, decimal? Price, string? Currency,
    string? FeaturedImageUrl, string? ImagesJson,
    string? Description, string? ShortDescription,
    string? ContactPhone, string? ContactWhatsapp);
