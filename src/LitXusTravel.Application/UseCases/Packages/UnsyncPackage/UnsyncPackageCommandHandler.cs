using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Packages.UnsyncPackage;

public class UnsyncPackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UnsyncPackageCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UnsyncPackageCommand request, CancellationToken ct)
    {
        var tenantPackage = await uow.TenantPackages.GetByIdAsync(request.TenantPackageId, ct);
        if (tenantPackage is null)
            return Result<string>.Failure("Tenant package not found.");

        if (tenantPackage.TenantId != request.TenantId)
            return Result<string>.Failure("Unauthorized.");

        // Hard-delete the override first (FK dependency)
        var @override = await uow.PackageOverrides
            .FirstOrDefaultAsync(o => o.TenantPackageId == request.TenantPackageId, ct);
        if (@override is not null)
            uow.PackageOverrides.Remove(@override);

        uow.TenantPackages.Remove(tenantPackage);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success("Package unsynced successfully.");
    }
}
