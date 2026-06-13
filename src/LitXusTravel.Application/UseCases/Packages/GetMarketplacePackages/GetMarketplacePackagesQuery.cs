using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Packages.GetMarketplacePackages;

public record GetMarketplacePackagesQuery(Guid TenantId) : IRequest<Result<IReadOnlyList<MarketplacePackageResponse>>>;

public record MarketplacePackageResponse(
    Guid Id,
    string Title,
    string? Category,
    string Destination,
    string? Region,
    decimal BasePrice,
    string Currency,
    int DurationDays,
    string? FeaturedImageUrl,
    string SourceTenantName
);
