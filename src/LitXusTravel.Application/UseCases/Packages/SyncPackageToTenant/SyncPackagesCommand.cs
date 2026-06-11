using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Packages.SyncPackageToTenant;

public record SyncPackagesCommand(Guid TenantId, IReadOnlyList<Guid> MasterPackageIds)
    : IRequest<Result<SyncPackagesResult>>;

public record SyncPackagesResult(
    IReadOnlyList<SyncedPackageItem> Synced,
    IReadOnlyList<FailedSyncItem> Failed
);

public record SyncedPackageItem(Guid Id, Guid MasterPackageId, DateTimeOffset SyncedAt);
public record FailedSyncItem(Guid MasterPackageId, string Reason);
