using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.DTOs;

public record AdminUserDto(
    Guid Id,
    string Name,
    string Email,
    AdminRole Role,
    AdminScope? Scope,
    Guid? AssignedTenantId,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
