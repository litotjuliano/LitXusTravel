using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Tenants.CreateTenant;
using LitXusTravel.Application.UseCases.Tenants.GetTenantById;
using LitXusTravel.Application.UseCases.Tenants.GetTenants;
using LitXusTravel.Application.UseCases.Tenants.GetTenantSettings;
using LitXusTravel.Application.UseCases.Tenants.UpdateTenantSettings;
using LitXusTravel.Application.UseCases.Packages.SyncPackagesToTenant;
using LitXusTravel.Application.UseCases.Packages.GetTenantPackages;
using LitXusTravel.Application.UseCases.Packages.UpdatePackageOverride;
using LitXusTravel.Application.UseCases.Packages.UnsyncPackage;
using LitXusTravel.Application.UseCases.Tenants.AssignPlan;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/tenants")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class TenantsController(IMediator mediator) : ControllerBase
{
    /// <summary>Create a new tenant with auto-provisioning (SPEC-ADMIN-004)</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("already exists")))
                return Conflict(new { result.Errors });
            return BadRequest(new { result.Errors });
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Get tenant by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTenantByIdQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Get tenant settings</summary>
    [HttpGet("{id:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSettings(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTenantSettingsQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Update tenant settings</summary>
    [HttpPut("{id:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSettings(Guid id,
        [FromBody] UpdateTenantSettingsRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateTenantSettingsCommand(id, request.DefaultCurrency), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok();
    }

    /// <summary>List tenants with pagination (SPEC-ADMIN-005)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetTenantsQuery(status, page, pageSize);
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

    /// <summary>Sync packages to tenant (SPEC-ADMIN-006)</summary>
    [HttpPost("{tenantId:guid}/packages/sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SyncPackages(
        Guid tenantId,
        [FromBody] SyncPackagesToTenantCommand command,
        CancellationToken ct)
    {
        var cmd = command with { TenantId = tenantId };
        var result = await mediator.Send(cmd, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Get tenant packages with overrides (SPEC-ADMIN-007)</summary>
    [HttpGet("{tenantId:guid}/packages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantPackages(
        Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "syncedAt",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken ct = default)
    {
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

    /// <summary>Update package override (SPEC-ADMIN-008)</summary>
    [HttpPut("{tenantId:guid}/packages/{tenantPackageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePackageOverride(
        Guid tenantId,
        Guid tenantPackageId,
        [FromBody] UpdatePackageOverrideCommand command,
        CancellationToken ct)
    {
        var cmd = command with { TenantId = tenantId, TenantPackageId = tenantPackageId };
        var result = await mediator.Send(cmd, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Unsync package from tenant (SPEC-ADMIN-009)</summary>
    [HttpDelete("{tenantId:guid}/packages/{tenantPackageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnsyncPackage(
        Guid tenantId,
        Guid tenantPackageId,
        CancellationToken ct)
    {
        var command = new UnsyncPackageCommand(tenantId, tenantPackageId);
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Assign a subscription plan to a tenant</summary>
    [HttpPost("{tenantId:guid}/assign-plan")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPlan(Guid tenantId, [FromBody] AssignPlanRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new AssignPlanCommand(tenantId, request.PlanName), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(new { message = result.Value });
    }
}

public record UpdateTenantSettingsRequest(string DefaultCurrency);
public record AssignPlanRequest(string PlanName);
