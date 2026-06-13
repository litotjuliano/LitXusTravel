using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Packages.UploadPackageImage;

public class UploadPackageImageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UploadPackageImageCommand, Result>
{
    public async Task<Result> Handle(UploadPackageImageCommand request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result.Failure("Package not found.");

        package.SetFeaturedImage(request.ImageUrl);
        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
