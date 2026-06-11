using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<Tenant> Tenants { get; }
    IRepository<TenantSubscription> TenantSubscriptions { get; }
    IRepository<Package> Packages { get; }
    IRepository<TenantPackage> TenantPackages { get; }
    IRepository<PackageOverride> PackageOverrides { get; }
    IRepository<Inquiry> Inquiries { get; }
    IRepository<CrmActivity> CrmActivities { get; }
    IRepository<Quotation> Quotations { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}
