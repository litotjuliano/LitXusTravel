using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Request;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Infrastructure.Identity;

namespace LitXusTravel.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration)
    : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
            return Result<AuthResponse>.Failure("Invalid email or password.");

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return Result<AuthResponse>.Failure("Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Agent";

        var token = GenerateJwt(user, role);
        return Result<AuthResponse>.Success(BuildResponse(user, role, token));
    }

    public async Task<Result<AuthResponse>> RegisterAgentAsync(RegisterAgentRequest request, CancellationToken ct = default)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = request.TenantId,
            IsActive = true,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponse>.Failure(createResult.Errors.Select(e => e.Description).ToArray());

        await userManager.AddToRoleAsync(user, "Agent");

        var token = GenerateJwt(user, "Agent");
        return Result<AuthResponse>.Success(BuildResponse(user, "Agent", token));
    }

    public async Task<Result<AuthResponse>> GetCurrentUserAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<AuthResponse>.Failure("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Agent";

        var token = GenerateJwt(user, role);
        return Result<AuthResponse>.Success(BuildResponse(user, role, token));
    }

    public async Task<Result<AuthResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<AuthResponse>.Failure("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<AuthResponse>.Failure(result.Errors.Select(e => e.Description).ToArray());

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Agent";

        var token = GenerateJwt(user, role);
        return Result<AuthResponse>.Success(BuildResponse(user, role, token));
    }

    private AuthResponse BuildResponse(ApplicationUser user, string role, (string token, int expiresIn) jwt)
        => new(jwt.token, "Bearer", jwt.expiresIn, user.Id, user.Email!, role, user.TenantId);

    private (string token, int expiresIn) GenerateJwt(ApplicationUser user, string role)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var m) ? m : 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, role),
            new("role", role)
        };

        if (user.TenantId.HasValue)
            claims.Add(new Claim("tenantId", user.TenantId.Value.ToString()));

        if (!string.IsNullOrEmpty(user.FirstName))
            claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName));

        if (!string.IsNullOrEmpty(user.LastName))
            claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiryMinutes * 60);
    }
}
