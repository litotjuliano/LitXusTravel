using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.GenerateAdminPackagePhoto;

public class GenerateAdminPackagePhotoCommandHandler(IUnitOfWork uow, IPhotoService photoService)
    : IRequestHandler<GenerateAdminPackagePhotoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GenerateAdminPackagePhotoCommand request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<string>.Failure("Package not found.");

        if (string.IsNullOrWhiteSpace(package.Destination))
            return Result<string>.Failure("Cannot determine destination for photo search.");

        var url = await photoService.GetPhotoUrlAsync(package.Destination, package.Category, ct);
        if (url is null)
            return Result<string>.Failure("Photo generation unavailable. Configure the Unsplash API key.");

        package.SetFeaturedImage(url);
        uow.Packages.Update(package);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success(url);
    }
}
