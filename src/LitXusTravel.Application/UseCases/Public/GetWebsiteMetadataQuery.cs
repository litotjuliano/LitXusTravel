using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Public;

public record GetWebsiteMetadataQuery(
    string? Subdomain = null
) : IRequest<Result<WebsiteMetadataResponse>>;

public record WebsiteMetadataResponse(
    string TenantName,
    string TenantSubdomain,
    int TotalPackages,
    int FeaturedPackagesCount,
    decimal AveragePackagePrice,
    string[] PopularDestinations,
    FeaturedPackageInfo[] FeaturedPackages,
    TestimonialInfo[] Testimonials
);

public record FeaturedPackageInfo(
    Guid Id,
    string Title,
    string ShortDescription,
    string Destination,
    decimal Price,
    string Currency,
    string FeaturedImageUrl
);

public record TestimonialInfo(
    string CustomerName,
    string Message,
    int Rating
);
