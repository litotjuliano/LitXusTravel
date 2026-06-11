using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(AuditAction action, string entityType, Guid entityId,
        Guid? tenantId = null, string? userId = null,
        object? oldValues = null, object? newValues = null,
        CancellationToken ct = default);
}
