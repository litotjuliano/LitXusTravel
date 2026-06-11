using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Infrastructure.Identity;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/setup")]
[AllowAnonymous]
public class AdminSetupController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    /// <summary>Bootstrap: Create SuperAdmin user (safe - idempotent)</summary>
    [HttpPost("create-superadmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSuperAdmin(
        [FromBody] CreateSuperAdminRequest request,
        CancellationToken ct)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Conflict(new { message = "SuperAdmin user already exists" });

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = "Platform",
            LastName = "SuperAdmin",
            IsActive = true,
            EmailConfirmed = true,
            TenantId = null
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await userManager.AddToRoleAsync(user, "SuperAdmin");
        
        return Ok(new { 
            message = "SuperAdmin created successfully",
            email = request.Email,
            role = "SuperAdmin"
        });
    }
}

public record CreateSuperAdminRequest(
    string Email,
    string Password
);
