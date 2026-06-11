namespace LitXusTravel.Application.DTOs.Response;

public record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string UserId,
    string Email,
    string Role,
    Guid? TenantId
);
