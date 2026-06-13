namespace LitXusTravel.Application.DTOs.Response;

public record AdminUserDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Scope,
    Guid? AssignedTenantId,
    bool IsActive,
    DateTime CreatedAt);
