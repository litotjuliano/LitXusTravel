using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Public;

public class GetWebsiteMetadataQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetWebsiteMetadataQuery, Result<WebsiteMetadataResponse>>
{
    public async Task<Result<WebsiteMetadataResponse>> Handle(
        GetWebsiteMetadataQuery request, CancellationToken ct)
    {
        var packages = await uow.Packages.GetAllAsync(ct);
        var published = packages
            .Where(p => p.Visibility == LitXusTravel.Domain.Entities.PackageVisibility.Published)
            .ToList();

        if (published.Count == 0)
            return Result<WebsiteMetadataResponse>.Failure("No packages available.");

        var featured = published.Where(p => p.IsFeatured).ToList();
        var destinations = published
            .Select(p => p.Destination)
            .Distinct()
            .Take(5)
            .ToArray();

        var avgPrice = published.Average(p => p.BasePrice);
        var featuredPackages = featured
            .Take(6)
            .Select(p => new FeaturedPackageInfo(
                Id: p.Id,
                Title: p.Title,
                ShortDescription: p.ShortDescription ?? p.Description,
                Destination: p.Destination,
                Price: p.BasePrice,
                Currency: p.Currency,
                FeaturedImageUrl: p.FeaturedImageUrl
            ))
            .ToArray();

        var testimonials = new[]
        {
            new TestimonialInfo("John Doe", "Amazing experience! Will definitely book again.", 5),
            new TestimonialInfo("Jane Smith", "Great packages and excellent customer service.", 5),
            new TestimonialInfo("Ahmed Khan", "Highly recommended for family trips.", 5),
        };

        var response = new WebsiteMetadataResponse(
            TenantName: "LitXusTravel",
            TenantSubdomain: request.Subdomain ?? "nexustravel",
            TotalPackages: published.Count,
            FeaturedPackagesCount: featured.Count,
            AveragePackagePrice: (decimal)avgPrice,
            PopularDestinations: destinations,
            FeaturedPackages: featuredPackages,
            Testimonials: testimonials
        );

        return Result<WebsiteMetadataResponse>.Success(response);
    }
}
