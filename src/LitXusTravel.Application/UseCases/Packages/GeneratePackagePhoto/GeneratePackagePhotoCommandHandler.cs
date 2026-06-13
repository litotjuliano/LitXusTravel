using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.GeneratePackagePhoto;

public class GeneratePackagePhotoCommandHandler(IUnitOfWork uow, IPhotoService photoService)
    : IRequestHandler<GeneratePackagePhotoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GeneratePackagePhotoCommand request, CancellationToken ct)
    {
        var tenantPackage = await uow.TenantPackages.GetByIdAsync(request.TenantPackageId, ct);
        if (tenantPackage is null)
            return Result<string>.Failure("Package not found.");

        if (tenantPackage.TenantId != request.TenantId)
            return Result<string>.Failure("Unauthorized.");

        var @override = await uow.PackageOverrides
            .FirstOrDefaultAsync(o => o.TenantPackageId == request.TenantPackageId, ct);

        string? destination;
        string? category;

        if (tenantPackage.IsOwnedPackage)
        {
            destination = @override?.Destination;
            category = @override?.Category;
        }
        else
        {
            var master = await uow.Packages.GetByIdAsync(tenantPackage.MasterPackageId!.Value, ct);
            destination = master?.Destination;
            category = master?.Category;
        }

        if (string.IsNullOrWhiteSpace(destination))
            return Result<string>.Failure("Cannot determine destination for photo search.");

        var url = await photoService.GetPhotoUrlAsync(destination, category, ct);
        if (url is null)
            return Result<string>.Failure("Photo generation unavailable. Configure the Unsplash API key.");

        if (@override is null)
        {
            @override = PackageOverride.CreateEmpty(request.TenantId, request.TenantPackageId);
            await uow.PackageOverrides.AddAsync(@override, ct);
        }

        @override.Update(featuredImageUrl: url);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success(url);
    }
}
