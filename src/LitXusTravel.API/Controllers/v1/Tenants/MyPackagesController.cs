using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Packages.CreateTenantPackage;
using LitXusTravel.Application.UseCases.Packages.GetMarketplacePackages;
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
    /// <summary>Create a tenant-owned package (SPEC-TENANT-006)</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(Guid tenantId, [FromBody] CreateTenantPackageRequest request, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var command = new CreateTenantPackageCommand(
            tenantId,
            request.Title, request.Destination, request.DurationDays,
            request.Price, request.Currency, request.Category, request.Region,
            request.Description, request.ShortDescription,
            request.FeaturedImageUrl, request.ContactPhone, request.ContactWhatsapp,
            request.ExtendToMaster);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return StatusCode(StatusCodes.Status201Created, new { id = result.Value });
    }

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
            request.ContactPhone, request.ContactWhatsapp,
            request.Destination, request.DurationDays, request.Category, request.Region);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>List marketplace packages available to add (Extended by other tenants, not yet synced)</summary>
    [HttpGet("/api/v1/tenants/{tenantId:guid}/marketplace")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMarketplace(Guid tenantId, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var result = await mediator.Send(new GetMarketplacePackagesQuery(tenantId), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Add a marketplace package to tenant catalog</summary>
    [HttpPost("/api/v1/tenants/{tenantId:guid}/marketplace/{packageId:guid}/add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFromMarketplace(Guid tenantId, Guid packageId, CancellationToken ct)
    {
        if (!IsAuthorizedForTenant(tenantId))
            return Forbid();

        var result = await mediator.Send(new SyncPackagesCommand(tenantId, [packageId]), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(new { message = "Package added to your catalog." });
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

public record CreateTenantPackageRequest(
    string Title,
    string Destination,
    int DurationDays,
    decimal Price,
    string Currency,
    string? Category,
    string? Region,
    string? Description,
    string? ShortDescription,
    string? FeaturedImageUrl,
    string? ContactPhone,
    string? ContactWhatsapp,
    bool ExtendToMaster);

public record UpdateOverrideRequest(
    string? Title, decimal? Price, string? Currency,
    string? FeaturedImageUrl, string? ImagesJson,
    string? Description, string? ShortDescription,
    string? ContactPhone, string? ContactWhatsapp,
    string? Destination = null, int? DurationDays = null,
    string? Category = null, string? Region = null);
