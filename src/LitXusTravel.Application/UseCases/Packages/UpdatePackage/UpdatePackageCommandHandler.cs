using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.UpdatePackage;

public class UpdatePackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdatePackageCommand, Result<object>>
{
    public async Task<Result<object>> Handle(UpdatePackageCommand request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.Id, ct);
        if (package is null)
            return Result<object>.Failure("Package not found.");

        try
        {
            package.UpdateDetails(
                title: request.Title,
                description: request.Description,
                shortDescription: request.ShortDescription,
                category: request.Category,
                basePrice: request.BasePrice,
                durationDays: request.DurationDays,
                destination: request.Destination,
                region: request.Region,
                itineraryJson: null,
                highlightsJson: null,
                inclusionsJson: null,
                exclusionsJson: null,
                imagesJson: null,
                featuredImageUrl: request.FeaturedImageUrl,
                maxGroupSize: null
            );
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ex.Message);
        }

        // Apply visibility change if requested (Draft→Archived only; publish via dedicated endpoint)
        if (request.Visibility == "Archived" && package.Visibility != PackageVisibility.Archived)
            package.Archive();

        uow.Packages.Update(package);
        await uow.SaveChangesAsync(ct);

        return Result<object>.Success(new
        {
            package.Id,
            package.Title,
            package.Destination,
            package.BasePrice,
            package.DurationDays,
            package.Category,
            package.Description,
            package.ShortDescription,
            package.Region,
            package.FeaturedImageUrl,
            Visibility = package.Visibility.ToString()
        });
    }
}
