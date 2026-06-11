using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Public;

public record GetPublicPackageDetailQuery(
    Guid PackageId
) : IRequest<Result<PublicPackageDetailResponse>>;

public record PublicPackageDetailResponse(
    Guid Id,
    string Title,
    string Description,
    string ShortDescription,
    string Category,
    decimal Price,
    string Currency,
    int DurationDays,
    string Destination,
    string Region,
    string FeaturedImageUrl,
    string[] ImagesJson,
    string[] ItineraryJson,
    string[] HighlightsJson,
    string[] InclusionsJson,
    string[] ExclusionsJson,
    bool IsFeatured,
    bool IsPopular
);
