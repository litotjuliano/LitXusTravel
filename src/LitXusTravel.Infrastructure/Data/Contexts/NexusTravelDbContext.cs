using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Identity;

namespace LitXusTravel.Infrastructure.Data.Contexts;

public class LitXusTravelDbContext(
    DbContextOptions<LitXusTravelDbContext> options,
    ICurrentTenant currentTenant)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<TenantPackage> TenantPackages => Set<TenantPackage>();
    public DbSet<PackageOverride> PackageOverrides => Set<PackageOverride>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<CrmActivity> CrmActivities => Set<CrmActivity>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Booking & Tour
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Booking> Bookings => Set<Booking>();

    // Role Hierarchy & Commission System
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<StaffAgent> StaffAgents => Set<StaffAgent>();
    public DbSet<IndependentAgent> IndependentAgents => Set<IndependentAgent>();
    public DbSet<CommissionRule> CommissionRules => Set<CommissionRule>();
    public DbSet<CommissionAccrual> CommissionAccruals => Set<CommissionAccrual>();
    public DbSet<CommissionPayout> CommissionPayouts => Set<CommissionPayout>();
    public DbSet<CodeUsageAudit> CodeUsageAudits => Set<CodeUsageAudit>();
    public DbSet<DisputeResolutionTicket> DisputeResolutionTickets => Set<DisputeResolutionTicket>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(LitXusTravelDbContext).Assembly);

        // Soft-delete filters for admin-accessible tables (no tenant scope)
        builder.Entity<Tenant>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        builder.Entity<Package>().HasQueryFilter(e => !e.DeletedAt.HasValue);

        // Tenant-scoped filters — closure captures currentTenant so the filter
        // is re-evaluated per query. When Id is null (admin / design-time) the
        // condition short-circuits and all rows are visible.
        // NOTE: .GetValueOrDefault() is used instead of .Value to avoid
        // InvalidOperationException when EF Core evaluates the expression tree
        // for SQL parameter extraction (it does not respect || short-circuit).
        builder.Entity<TenantPackage>()
            .HasQueryFilter(e => !e.DeletedAt.HasValue
                && (!currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault()));

        builder.Entity<PackageOverride>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<Inquiry>()
            .HasQueryFilter(e => !e.DeletedAt.HasValue
                && (!currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault()));

        builder.Entity<CrmActivity>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<Quotation>()
            .HasQueryFilter(e => !e.DeletedAt.HasValue
                && (!currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault()));

        builder.Entity<Notification>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        // Commission & booking entities — tenant-scoped, no soft-delete
        builder.Entity<Tour>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<Booking>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<CommissionRule>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<CommissionAccrual>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<CommissionPayout>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());

        builder.Entity<StaffAgent>()
            .HasQueryFilter(e =>
                !currentTenant.Id.HasValue || e.TenantId == currentTenant.Id.GetValueOrDefault());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdatedBy(entry.Entity.UpdatedBy ?? "system");
        }
        return await base.SaveChangesAsync(ct);
    }
}
