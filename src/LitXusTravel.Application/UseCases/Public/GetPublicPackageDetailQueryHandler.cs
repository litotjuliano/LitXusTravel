using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using System.Text.Json;

namespace LitXusTravel.Application.UseCases.Public;

public class GetPublicPackageDetailQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPublicPackageDetailQuery, Result<PublicPackageDetailResponse>>
{
    public async Task<Result<PublicPackageDetailResponse>> Handle(
        GetPublicPackageDetailQuery request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<PublicPackageDetailResponse>.Failure("Package not found.");

        if (package.Visibility != LitXusTravel.Domain.Entities.PackageVisibility.Published)
            return Result<PublicPackageDetailResponse>.Failure("Package not available for public view.");

        var images = ParseJsonArray(package.ImagesJson);
        var itinerary = ParseJsonArray(package.ItineraryJson);
        var highlights = ParseJsonArray(package.HighlightsJson);
        var inclusions = ParseJsonArray(package.InclusionsJson);
        var exclusions = ParseJsonArray(package.ExclusionsJson);

        var response = new PublicPackageDetailResponse(
            Id: package.Id,
            Title: package.Title,
            Description: package.Description ?? "",
            ShortDescription: package.ShortDescription ?? package.Description ?? "",
            Category: package.Category ?? "",
            Price: package.BasePrice,
            Currency: package.Currency,
            DurationDays: package.DurationDays,
            Destination: package.Destination,
            Region: package.Region ?? "",
            FeaturedImageUrl: package.FeaturedImageUrl ?? "",
            ImagesJson: images,
            ItineraryJson: itinerary,
            HighlightsJson: highlights,
            InclusionsJson: inclusions,
            ExclusionsJson: exclusions,
            IsFeatured: package.IsFeatured,
            IsPopular: package.IsPopular
        );

        return Result<PublicPackageDetailResponse>.Success(response);
    }

    private string[] ParseJsonArray(string? json)
    {
        if (string.IsNullOrEmpty(json)) return [];
        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
