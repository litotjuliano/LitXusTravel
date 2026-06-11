using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Packages.CreatePackage;

public record CreatePackageCommand(
    string Title,
    string Destination,
    decimal BasePrice,
    int DurationDays,
    string? Category,
    string? Description,
    string? ShortDescription,
    string? Currency,
    string? Region,
    string? ItineraryJson,
    string? HighlightsJson,
    string? InclusionsJson,
    string? ExclusionsJson,
    string? ImagesJson,
    string? FeaturedImageUrl,
    int? MaxGroupSize
) : IRequest<Result<PackageResponse>>;
