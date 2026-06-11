using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Public;

public class GetPublicPackagesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPublicPackagesQuery, Result<PagedList<PublicPackageResponse>>>
{
    public async Task<Result<PagedList<PublicPackageResponse>>> Handle(
        GetPublicPackagesQuery request, CancellationToken ct)
    {
        var allPackages = await uow.Packages.GetAllAsync(ct);

        var filtered = allPackages
            .Where(p => p.Visibility == LitXusTravel.Domain.Entities.PackageVisibility.Published)
            .Where(p => string.IsNullOrEmpty(request.Destination) ||
                   p.Destination.Contains(request.Destination, StringComparison.OrdinalIgnoreCase))
            .Where(p => string.IsNullOrEmpty(request.Category) ||
                   p.Category == request.Category)
            .ToList();

        if (filtered.Count == 0)
            return Result<PagedList<PublicPackageResponse>>.Success(
                PagedList<PublicPackageResponse>.Create([], request.Page, request.PageSize, 0));

        var totalCount = filtered.Count;

        var sorted = request.SortBy?.ToLower() switch
        {
            "price" => request.SortOrder?.ToLower() == "asc"
                ? filtered.OrderBy(p => p.BasePrice).ToList()
                : filtered.OrderByDescending(p => p.BasePrice).ToList(),
            "duration" => request.SortOrder?.ToLower() == "asc"
                ? filtered.OrderBy(p => p.DurationDays).ToList()
                : filtered.OrderByDescending(p => p.DurationDays).ToList(),
            "destination" => request.SortOrder?.ToLower() == "asc"
                ? filtered.OrderBy(p => p.Destination).ToList()
                : filtered.OrderByDescending(p => p.Destination).ToList(),
            _ => request.SortOrder?.ToLower() == "asc"
                ? filtered.OrderBy(p => p.Title).ToList()
                : filtered.OrderByDescending(p => p.Title).ToList(),
        };

        var paginated = sorted
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var responses = paginated.Select(p => new PublicPackageResponse(
            Id: p.Id,
            Title: p.Title,
            ShortDescription: p.ShortDescription ?? p.Description ?? "",
            Category: p.Category ?? "",
            Price: p.BasePrice,
            Currency: p.Currency,
            DurationDays: p.DurationDays,
            Destination: p.Destination,
            Region: p.Region ?? "",
            FeaturedImageUrl: p.FeaturedImageUrl ?? "",
            IsFeatured: p.IsFeatured,
            IsPopular: p.IsPopular
        )).ToList();

        var pagedList = PagedList<PublicPackageResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<PublicPackageResponse>>.Success(pagedList);
    }
}
