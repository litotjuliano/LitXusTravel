using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Packages.SyncPackageToTenant;

public class SyncPackagesCommandHandler(IUnitOfWork uow, IAuditService audit)
    : IRequestHandler<SyncPackagesCommand, Result<SyncPackagesResult>>
{
    public async Task<Result<SyncPackagesResult>> Handle(SyncPackagesCommand request, CancellationToken ct)
    {
        if (request.MasterPackageIds.Count == 0)
            return Result<SyncPackagesResult>.Failure("At least one package ID is required.");

        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<SyncPackagesResult>.Failure("Tenant not found.");

        var synced = new List<SyncedPackageItem>();
        var failed = new List<FailedSyncItem>();

        foreach (var masterPackageId in request.MasterPackageIds)
        {
            var package = await uow.Packages.GetByIdAsync(masterPackageId, ct);

            if (package is null)
            {
                failed.Add(new(masterPackageId, "Package not found."));
                continue;
            }

            if (package.Visibility != PackageVisibility.Published)
            {
                failed.Add(new(masterPackageId, "Only published packages can be synced."));
                continue;
            }

            var alreadySynced = await uow.TenantPackages.AnyAsync(
                tp => tp.TenantId == request.TenantId && tp.MasterPackageId == masterPackageId, ct);

            if (alreadySynced)
            {
                failed.Add(new(masterPackageId, "Package is already synced to this tenant."));
                continue;
            }

            var tenantPackage = TenantPackage.Create(request.TenantId, masterPackageId);
            var emptyOverride = PackageOverride.CreateEmpty(request.TenantId, tenantPackage.Id);

            await uow.TenantPackages.AddAsync(tenantPackage, ct);
            await uow.PackageOverrides.AddAsync(emptyOverride, ct);

            synced.Add(new(tenantPackage.Id, masterPackageId, tenantPackage.LastSyncedAt!.Value));
        }

        if (synced.Count > 0)
        {
            await uow.SaveChangesAsync(ct);

            await audit.LogAsync(AuditAction.Synced, nameof(TenantPackage), request.TenantId,
                tenantId: request.TenantId,
                newValues: new { SyncedCount = synced.Count, PackageIds = synced.Select(s => s.MasterPackageId) },
                ct: ct);
        }

        return Result<SyncPackagesResult>.Success(new SyncPackagesResult(synced, failed));
    }
}
