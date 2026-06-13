using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.PublishTenantPackage;

public class PublishTenantPackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<PublishTenantPackageCommand, Result>
{
    public async Task<Result> Handle(PublishTenantPackageCommand request, CancellationToken ct)
    {
        var tenantPackage = await uow.TenantPackages.GetByIdAsync(request.TenantPackageId, ct);
        if (tenantPackage is null)
            return Result.Failure("Package not found.");

        if (tenantPackage.TenantId != request.TenantId)
            return Result.Failure("Unauthorized.");

        if (tenantPackage.IsOwnedPackage || !tenantPackage.MasterPackageId.HasValue)
            return Result.Failure("Portal-only packages cannot be published through this endpoint.");

        var master = await uow.Packages.GetByIdAsync(tenantPackage.MasterPackageId.Value, ct);
        if (master is null)
            return Result.Failure("Master package not found.");

        if (master.CreatedByTenantId != request.TenantId)
            return Result.Failure("You can only publish packages you own.");

        try
        {
            master.Publish();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        uow.Packages.Update(master);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
