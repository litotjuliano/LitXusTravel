using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Events;
using LitXusTravel.Domain.Exceptions;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Admin user entity representing SuperAdmin or Admin roles in the system.
/// SuperAdmin has full platform control, Admin can be Platform or Tenant scoped.
/// </summary>
public class AdminUser : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public AdminRole Role { get; private set; }
    public AdminScope Scope { get; private set; }
    public Guid? AssignedTenantId { get; private set; }
    public List<Guid> ManagedTenantIds { get; private set; } = [];
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private AdminUser() { }

    /// <summary>
    /// Create a new SuperAdmin user. Only one SuperAdmin allowed per system.
    /// </summary>
    public static AdminUser CreateSuperAdmin(string name, Email email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Admin name is required");

        var admin = new AdminUser
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Role = AdminRole.Admin,
            Scope = AdminScope.Platform,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.RaiseDomainEvent(new AdminUserCreatedEvent(admin.Id, admin.Name, admin.Role));
        return admin;
    }

    /// <summary>
    /// Create a Platform Admin. Can manage multiple tenants.
    /// </summary>
    public static AdminUser CreatePlatformAdmin(string name, Email email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Admin name is required");

        var admin = new AdminUser
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Role = AdminRole.Admin,
            Scope = AdminScope.Platform,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.RaiseDomainEvent(new AdminUserCreatedEvent(admin.Id, admin.Name, admin.Role));
        return admin;
    }

    /// <summary>
    /// Create a Tenant Admin. Scoped to a specific tenant.
    /// </summary>
    public static AdminUser CreateTenantAdmin(string name, Email email, Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Admin name is required");
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required for Tenant Admin");

        var admin = new AdminUser
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Role = AdminRole.Admin,
            Scope = AdminScope.Tenant,
            AssignedTenantId = tenantId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.RaiseDomainEvent(new AdminUserCreatedEvent(admin.Id, admin.Name, admin.Role));
        return admin;
    }

    /// <summary>
    /// Assign a tenant to a Platform Admin.
    /// </summary>
    public void AssignTenant(Guid tenantId)
    {
        if (Scope != AdminScope.Platform)
            throw new DomainException("Only Platform Admins can be assigned multiple tenants");
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required");
        if (ManagedTenantIds.Contains(tenantId))
            throw new DomainException("Tenant is already assigned to this admin");

        ManagedTenantIds.Add(tenantId);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new TenantAssignedToAdminEvent(Id, tenantId));
    }

    /// <summary>
    /// Deactivate the admin user.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AdminUserDeactivatedEvent(Id));
    }

    /// <summary>
    /// Activate the admin user.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AdminUserActivatedEvent(Id));
    }
}

/// <summary>
/// Admin role enumeration.
/// </summary>
public enum AdminRole
{
    SuperAdmin = 0,
    Admin = 1
}

/// <summary>
/// Admin scope enumeration.
/// </summary>
public enum AdminScope
{
    Platform = 0,
    Tenant = 1
}
