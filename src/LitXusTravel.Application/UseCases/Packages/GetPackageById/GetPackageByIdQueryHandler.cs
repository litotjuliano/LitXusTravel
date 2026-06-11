using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Packages.GetPackageById;

public class GetPackageByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPackageByIdQuery, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(
        GetPackageByIdQuery request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<PackageResponse>.Failure("Package not found.");

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
