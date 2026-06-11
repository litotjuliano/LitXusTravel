using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Packages.SyncPackagesToTenant;

public record SyncPackagesToTenantCommand(
    Guid TenantId,
    List<Guid> MasterPackageIds
) : IRequest<Result<SyncResultResponse>>;

public record SyncResultResponse(
    List<SyncedPackage> Synced,
    List<FailedPackage> Failed
);

public record SyncedPackage(
    Guid TenantPackageId,
    Guid MasterPackageId,
    DateTimeOffset SyncedAt
);

public record FailedPackage(
    Guid MasterPackageId,
    string Reason
);
