using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.DomainEvents;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public enum SyncStatus { Synced, Conflict, Pending }

public class TenantPackage : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? MasterPackageId { get; private set; }
    public bool IsOwnedPackage { get; private set; }
    public SyncStatus SyncStatus { get; private set; } = SyncStatus.Synced;
    public DateTimeOffset? LastSyncedAt { get; private set; }
    public DateTimeOffset? LastMasterSyncAt { get; private set; }
    public bool IsCustomized { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsLocked { get; private set; }

    public Tenant Tenant { get; private set; } = null!;
    public Package MasterPackage { get; private set; } = null!;
    public PackageOverride? Override { get; private set; }
    public ICollection<Inquiry> Inquiries { get; private set; } = [];

    private TenantPackage() { }

    public static TenantPackage Create(Guid tenantId, Guid masterPackageId)
    {
        var tp = new TenantPackage
        {
            TenantId = tenantId,
            MasterPackageId = masterPackageId,
            IsOwnedPackage = false,
            SyncStatus = SyncStatus.Synced,
            LastSyncedAt = DateTimeOffset.UtcNow
        };

        tp.RaiseDomainEvent(new PackageSynchronizedEvent(tenantId, masterPackageId, tp.Id));
        return tp;
    }

    public static TenantPackage CreateOwned(Guid tenantId)
    {
        return new TenantPackage
        {
            TenantId = tenantId,
            MasterPackageId = null,
            IsOwnedPackage = true,
            SyncStatus = SyncStatus.Synced,
            LastSyncedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarkMasterSynced()
    {
        if (IsLocked) return;
        LastMasterSyncAt = DateTimeOffset.UtcNow;
        SyncStatus = SyncStatus.Synced;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCustomized()
    {
        IsCustomized = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Lock()
    {
        IsLocked = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Unlock()
    {
        IsLocked = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
