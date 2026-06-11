using System.Text.Json;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Data.Contexts;

namespace LitXusTravel.Infrastructure.Services;

public class AuditService(LitXusTravelDbContext context) : IAuditService
{
    public async Task LogAsync(AuditAction action, string entityType, Guid entityId,
        Guid? tenantId = null, string? userId = null,
        object? oldValues = null, object? newValues = null,
        CancellationToken ct = default)
    {
        var log = AuditLog.Create(
            action, entityType, entityId, tenantId, userId,
            oldValues is not null ? JsonSerializer.Serialize(oldValues) : null,
            newValues is not null ? JsonSerializer.Serialize(newValues) : null);

        await context.AuditLogs.AddAsync(log, ct);
        await context.SaveChangesAsync(ct);
    }
}
