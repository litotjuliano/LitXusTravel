using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Packages.CreatePackage;
using LitXusTravel.Application.UseCases.Packages.GetPackages;
using LitXusTravel.Application.UseCases.Packages.GetPackageById;
using LitXusTravel.Application.UseCases.Packages.PublishPackage;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/packages")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class PackagesController(IMediator mediator) : ControllerBase
{
    /// <summary>Create a new master package (SPEC-ADMIN-001)</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePackageCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>List packages with pagination (SPEC-ADMIN-002)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? status,
        [FromQuery] string? destination,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetPackagesQuery(status, destination, page, pageSize);
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

    /// <summary>Get package by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPackageByIdQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Publish a master package (SPEC-ADMIN-003)</summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new PublishPackageCommand(id), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }
}
