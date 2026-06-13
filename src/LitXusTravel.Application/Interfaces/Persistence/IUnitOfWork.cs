using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<Tenant> Tenants { get; }
    IRepository<TenantSubscription> TenantSubscriptions { get; }
    IRepository<Package> Packages { get; }
    ITenantPackageRepository TenantPackages { get; }
    IRepository<PackageOverride> PackageOverrides { get; }
    IRepository<Inquiry> Inquiries { get; }
    IRepository<CrmActivity> CrmActivities { get; }
    IRepository<Quotation> Quotations { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }

    // Booking & Tour
    ITourRepository Tours { get; }
    IBookingRepository Bookings { get; }

    // Role Hierarchy & Commission System Repositories
    IAdminUserRepository AdminUsers { get; }
    IStaffAgentRepository StaffAgents { get; }
    IIndependentAgentRepository IndependentAgents { get; }
    ICommissionRuleRepository CommissionRules { get; }
    ICommissionAccrualRepository CommissionAccruals { get; }
    ICommissionPayoutRepository CommissionPayouts { get; }
    ICodeUsageAuditRepository CodeUsageAudits { get; }
    IDisputeResolutionRepository DisputeResolutionTickets { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}
