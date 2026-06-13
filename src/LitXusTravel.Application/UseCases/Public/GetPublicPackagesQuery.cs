using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Public;

public record GetPublicPackagesQuery(
    string Subdomain,
    int Page = 1,
    int PageSize = 50,
    string? Destination = null,
    string? Category = null,
    string? SortBy = null,
    string? SortOrder = "asc"
) : IRequest<Result<PagedList<PublicPackageResponse>>>;

public record PublicPackageResponse(
    Guid Id,
    string Title,
    string ShortDescription,
    string Category,
    decimal Price,
    string Currency,
    int DurationDays,
    string Destination,
    string Region,
    string FeaturedImageUrl,
    bool IsFeatured,
    bool IsPopular
);
