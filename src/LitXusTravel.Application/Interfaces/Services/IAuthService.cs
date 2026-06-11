using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Request;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> RegisterAgentAsync(RegisterAgentRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> GetCurrentUserAsync(string userId, CancellationToken ct = default);
    Task<Result<AuthResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken ct = default);
}
