using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Packages.SyncPackagesToTenant;

public class SyncPackagesToTenantCommandHandler(IUnitOfWork uow)
    : IRequestHandler<SyncPackagesToTenantCommand, Result<SyncResultResponse>>
{
    public async Task<Result<SyncResultResponse>> Handle(SyncPackagesToTenantCommand request, CancellationToken ct)
    {
        // Validate tenant exists
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<SyncResultResponse>.Failure("Tenant not found.");

        var synced = new List<SyncedPackage>();
        var failed = new List<FailedPackage>();

        foreach (var packageId in request.MasterPackageIds)
        {
            try
            {
                // Get master package
                var masterPackage = await uow.Packages.GetByIdAsync(packageId, ct);
                if (masterPackage is null)
                {
                    failed.Add(new FailedPackage(packageId, "Package not found."));
                    continue;
                }

                // Check if published
                if (masterPackage.Visibility != PackageVisibility.Published)
                {
                    failed.Add(new FailedPackage(packageId, "Package must be published before syncing."));
                    continue;
                }

                // Check if already synced (avoid duplicates)
                var existingSync = (await uow.TenantPackages.GetAllAsync(ct))
                    .FirstOrDefault(tp => tp.TenantId == request.TenantId
                                       && tp.MasterPackageId == packageId);

                if (existingSync is not null)
                {
                    failed.Add(new FailedPackage(packageId, "Package already synced to this tenant."));
                    continue;
                }

                // Create sync mapping: TenantPackage
                var tenantPackage = TenantPackage.Create(request.TenantId, packageId);

                // Create empty override (all fields NULL = inherit from master)
                var packageOverride = PackageOverride.CreateEmpty(request.TenantId, tenantPackage.Id);

                // Save
                await uow.TenantPackages.AddAsync(tenantPackage, ct);
                await uow.PackageOverrides.AddAsync(packageOverride, ct);
                await uow.SaveChangesAsync(ct);

                synced.Add(new SyncedPackage(tenantPackage.Id, packageId, DateTimeOffset.UtcNow));
            }
            catch (Exception ex)
            {
                failed.Add(new FailedPackage(packageId, $"Sync failed: {ex.Message}"));
            }
        }

        var result = new SyncResultResponse(synced, failed);
        return Result<SyncResultResponse>.Success(result);
    }
}
