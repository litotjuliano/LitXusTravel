using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.DeletePackage;

public class DeletePackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeletePackageCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeletePackageCommand request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.Id, ct);
        if (package is null)
            return Result<string>.Failure("Package not found.");

        // Remove all TenantPackage records (and their overrides) that reference this master package
        var tenantPackages = (await uow.TenantPackages.GetAllAsync(ct))
            .Where(tp => tp.MasterPackageId == request.Id)
            .ToList();

        foreach (var tp in tenantPackages)
        {
            var ov = await uow.PackageOverrides
                .FirstOrDefaultAsync(o => o.TenantPackageId == tp.Id, ct);
            if (ov is not null)
                uow.PackageOverrides.Remove(ov);

            uow.TenantPackages.Remove(tp);
        }

        uow.Packages.Remove(package);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success("Package deleted.");
    }
}
