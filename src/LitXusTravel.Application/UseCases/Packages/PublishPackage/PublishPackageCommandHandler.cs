using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Application.UseCases.Packages.PublishPackage;

public class PublishPackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<PublishPackageCommand, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(PublishPackageCommand request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<PackageResponse>.Failure("Package not found.");

        try
        {
            package.Publish();
        }
        catch (DomainException ex)
        {
            return Result<PackageResponse>.Failure(ex.Message);
        }

        uow.Packages.Update(package);
        await uow.SaveChangesAsync(ct);

        var response = new PackageResponse(
            Id: package.Id,
            Title: package.Title,
            Description: package.Description,
            ShortDescription: package.ShortDescription,
            Category: package.Category,
            BasePrice: package.BasePrice,
            Currency: package.Currency ?? "USD",
            DurationDays: package.DurationDays,
            Destination: package.Destination,
            Region: package.Region,
            FeaturedImageUrl: package.FeaturedImageUrl,
            ImagesJson: package.ImagesJson,
            ItineraryJson: package.ItineraryJson,
            HighlightsJson: package.HighlightsJson,
            InclusionsJson: package.InclusionsJson,
            ExclusionsJson: package.ExclusionsJson,
            Rating: package.Rating,
            ReviewCount: package.ReviewCount,
            Visibility: package.Visibility.ToString(),
            IsPopular: package.IsPopular,
            IsFeatured: package.IsFeatured,
            CreatedAt: package.CreatedAt,
            UpdatedAt: package.UpdatedAt
        );

        return Result<PackageResponse>.Success(response);
    }
}
