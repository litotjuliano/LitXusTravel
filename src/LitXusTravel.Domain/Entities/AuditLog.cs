using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.Entities;

public enum AuditAction { Created, Updated, Deleted, Published, Synced, StatusChanged }

public class AuditLog : BaseEntity
{
    public Guid? TenantId { get; private set; }
    public string? UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string? OldValuesJson { get; private set; }
    public string? NewValuesJson { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        AuditAction action, string entityType, Guid entityId,
        Guid? tenantId = null, string? userId = null,
        string? oldValuesJson = null, string? newValuesJson = null,
        string? ipAddress = null, string? userAgent = null)
        => new()
        {
            TenantId = tenantId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValuesJson = oldValuesJson,
            NewValuesJson = newValuesJson,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
}
