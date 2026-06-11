using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid? TenantId { get; private set; }
    public string? UserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public Guid? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        string type, string title, string message,
        Guid? tenantId = null, string? userId = null,
        Guid? relatedEntityId = null, string? relatedEntityType = null)
        => new()
        {
            Type = type,
            Title = title,
            Message = message,
            TenantId = tenantId,
            UserId = userId,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType
        };

    public void MarkRead()
    {
        IsRead = true;
        ReadAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
