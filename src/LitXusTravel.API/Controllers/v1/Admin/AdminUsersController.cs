using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.UseCases.AdminUsers.CreateAdminUser;
using LitXusTravel.Application.UseCases.AdminUsers.GetAdminById;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/users")]
[Tags("Admin Users")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new admin user (SuperAdmin or Platform Admin).
    /// Tenant Admins are created separately.
    /// </summary>
    /// <remarks>
    /// Only SuperAdmin can create other admins.
    /// - Role must be "Admin"
    /// - Scope must be "Platform" (for Platform Admins)
    /// - This triggers an audit log entry
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAdmin(CreateAdminUserRequest request)
    {
        var command = new CreateAdminUserCommand(
            request.Name,
            request.Email,
            Enum.Parse<AdminRole>(request.Role),
            Enum.Parse<AdminScope>(request.Scope),
            request.AssignedTenantId);

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetAdminById), new { id = result.Data }, result.Data);
    }

    /// <summary>
    /// Get admin user by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdminById(Guid id)
    {
        var query = new GetAdminByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
}

public record CreateAdminUserRequest(
    string Name,
    string Email,
    string Role,
    string Scope,
    Guid? AssignedTenantId = null);
