using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Packages.SyncPackageToTenant;

public class SyncPackagesCommandHandler(IUnitOfWork uow)
    : IRequestHandler<SyncPackagesCommand, Result<SyncPackagesResult>>
{
    public async Task<Result<SyncPackagesResult>> Handle(SyncPackagesCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<SyncPackagesResult>.Failure("Tenant not found.");

        var synced = new List<SyncedPackageItem>();
        var failed = new List<FailedSyncItem>();

        foreach (var packageId in request.MasterPackageIds)
        {
            try
            {
                var masterPackage = await uow.Packages.GetByIdAsync(packageId, ct);
                if (masterPackage is null)
                {
                    failed.Add(new FailedSyncItem(packageId, "Package not found."));
                    continue;
                }

                if (masterPackage.Visibility != PackageVisibility.Published)
                {
                    failed.Add(new FailedSyncItem(packageId, "Package must be published before syncing."));
                    continue;
                }

                var existingSync = (await uow.TenantPackages.GetAllAsync(ct))
                    .FirstOrDefault(tp => tp.TenantId == request.TenantId
                                       && tp.MasterPackageId == packageId);

                if (existingSync is not null)
                {
                    failed.Add(new FailedSyncItem(packageId, "Package already synced to this tenant."));
                    continue;
                }

                var tenantPackage = TenantPackage.Create(request.TenantId, packageId);
                var packageOverride = PackageOverride.CreateEmpty(request.TenantId, tenantPackage.Id);

                await uow.TenantPackages.AddAsync(tenantPackage, ct);
                await uow.PackageOverrides.AddAsync(packageOverride, ct);
                await uow.SaveChangesAsync(ct);

                synced.Add(new SyncedPackageItem(tenantPackage.Id, packageId, DateTimeOffset.UtcNow));
            }
            catch (Exception ex)
            {
                failed.Add(new FailedSyncItem(packageId, $"Sync failed: {ex.Message}"));
            }
        }

        return Result<SyncPackagesResult>.Success(new SyncPackagesResult(synced, failed));
    }
}
