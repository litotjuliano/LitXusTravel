using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.DTOs.Request;
using LitXusTravel.Application.Interfaces.Services;
using System.Security.Claims;

namespace LitXusTravel.API.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Login and receive a JWT token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Errors.FirstOrDefault() });

        return Ok(result.Value);
    }

    /// <summary>Register a new agent user for an existing tenant (Admin only)</summary>
    [HttpPost("register-agent")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAgent([FromBody] RegisterAgentRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAgentAsync(request, ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("already exists")))
                return Conflict(new { result.Errors });
            return BadRequest(new { result.Errors });
        }

        return CreatedAtAction(nameof(Me), null, result.Value);
    }

    /// <summary>Get the currently authenticated user's info and a fresh token</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await authService.GetCurrentUserAsync(userId, ct);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Errors.FirstOrDefault() });

        return Ok(result.Value);
    }

    /// <summary>Update the currently authenticated user's profile</summary>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await authService.UpdateProfileAsync(userId, request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { result.Errors });

        return Ok(result.Value);
    }
}
