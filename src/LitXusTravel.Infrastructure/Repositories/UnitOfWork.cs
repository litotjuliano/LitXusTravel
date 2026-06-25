using Microsoft.EntityFrameworkCore;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Persistence.Repositories;

namespace LitXusTravel.Infrastructure.Repositories;

public class UnitOfWork(LitXusTravelDbContext context) : IUnitOfWork
{
    public IRepository<Tenant> Tenants { get; } = new Repository<Tenant>(context);
    public IRepository<TenantSubscription> TenantSubscriptions { get; } = new Repository<TenantSubscription>(context);
    public IRepository<SubscriptionPlan> SubscriptionPlans { get; } = new Repository<SubscriptionPlan>(context);
    public IRepository<Package> Packages { get; } = new Repository<Package>(context);
    public ITenantPackageRepository TenantPackages { get; } = new TenantPackageRepository(context);
    public IRepository<PackageOverride> PackageOverrides { get; } = new Repository<PackageOverride>(context);
    public IRepository<Inquiry> Inquiries { get; } = new Repository<Inquiry>(context);
    public IRepository<CrmActivity> CrmActivities { get; } = new Repository<CrmActivity>(context);
    public IRepository<Quotation> Quotations { get; } = new Repository<Quotation>(context);
    public IRepository<Notification> Notifications { get; } = new Repository<Notification>(context);
    public IRepository<AuditLog> AuditLogs { get; } = new Repository<AuditLog>(context);
    public IRepository<Invoice> Invoices { get; } = new Repository<Invoice>(context);

    // Booking & Tour Repositories
    public ITourRepository Tours { get; } = new TourRepository(context);
    public IBookingRepository Bookings { get; } = new BookingRepository(context);

    // Role Hierarchy & Commission System Repositories
    public IAdminUserRepository AdminUsers { get; } = new AdminUserRepository(context);
    public IStaffAgentRepository StaffAgents { get; } = new StaffAgentRepository(context);
    public IIndependentAgentRepository IndependentAgents { get; } = new IndependentAgentRepository(context);
    public ICommissionRuleRepository CommissionRules { get; } = new CommissionRuleRepository(context);
    public ICommissionAccrualRepository CommissionAccruals { get; } = new CommissionAccrualRepository(context);
    public ICommissionPayoutRepository CommissionPayouts { get; } = new CommissionPayoutRepository(context);
    public ICodeUsageAuditRepository CodeUsageAudits { get; } = new CodeUsageAuditRepository(context);
    public IDisputeResolutionRepository DisputeResolutionTickets { get; } = new DisputeResolutionRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        DispatchDomainEvents();
        return await context.SaveChangesAsync(ct);
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        await using var tx = await context.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    private void DispatchDomainEvents()
    {
        var aggregates = context.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();
    }
}
