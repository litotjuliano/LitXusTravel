using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.UseCases.AdminUsers.CreateAdminUser;
using LitXusTravel.Application.UseCases.AdminUsers.GetAdminById;
using LitXusTravel.Application.UseCases.AdminUsers.GetAdminUsers;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/users")]
[Tags("Admin Users")]
public class AdminUsersController(IMediator mediator) : ControllerBase
{
    /// <summary>List admin users. Optionally filter by tenant or active status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AdminUserListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminUsers(
        [FromQuery] Guid? tenantId = null,
        [FromQuery] bool activeOnly = false,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminUsersQuery(tenantId, activeOnly), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new admin user (SuperAdmin or Platform Admin).
    /// Only SuperAdmin can create other admins. Triggers an audit log entry.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAdmin(CreateAdminUserRequest request, CancellationToken ct = default)
    {
        var command = new CreateAdminUserCommand(
            request.Name,
            request.Email,
            Enum.Parse<AdminRole>(request.Role),
            Enum.Parse<AdminScope>(request.Scope),
            request.AssignedTenantId);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetAdminById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>Get admin user by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdminById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminByIdQuery(id), ct);
        if (!result.IsSuccess)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Value);
    }
}

public record CreateAdminUserRequest(
    string Name,
    string Email,
    string Role,
    string Scope,
    Guid? AssignedTenantId = null);
