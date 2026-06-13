using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Public;

public class GetPublicPackagesQueryHandler(IUnitOfWork uow, ITenantResolver tenantResolver)
    : IRequestHandler<GetPublicPackagesQuery, Result<PagedList<PublicPackageResponse>>>
{
    public async Task<Result<PagedList<PublicPackageResponse>>> Handle(
        GetPublicPackagesQuery request, CancellationToken ct)
    {
        var tenantInfo = await tenantResolver.ResolveTenantInfoAsync(request.Subdomain, ct);
        if (tenantInfo is null)
            return Result<PagedList<PublicPackageResponse>>.Failure("Website not found.");

        var tenantPackages = await uow.TenantPackages
            .GetByTenantWithDetailsAsync(tenantInfo.Value.Id, ct);

        var responses = new List<PublicPackageResponse>();

        foreach (var tp in tenantPackages)
        {
            PublicPackageResponse? response;

            if (tp.IsOwnedPackage)
            {
                // Portal-only package — all data lives in the Override
                if (tp.Override is null) continue;
                response = MapOwned(tp);
            }
            else
            {
                // Synced from master catalog
                if (tp.MasterPackage is null) continue;
                if (tp.MasterPackage.Visibility != PackageVisibility.Published) continue;
                response = MapSynced(tp);
            }

            if (!string.IsNullOrEmpty(request.Category) &&
                !string.Equals(response.Category, request.Category, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!string.IsNullOrEmpty(request.Destination) &&
                !response.Destination.Contains(request.Destination, StringComparison.OrdinalIgnoreCase))
                continue;

            responses.Add(response);
        }

        var sorted = request.SortBy?.ToLower() switch
        {
            "price_asc"  => responses.OrderBy(p => p.Price).ToList(),
            "price_desc" => responses.OrderByDescending(p => p.Price).ToList(),
            "rating"     => responses.OrderByDescending(p => p.IsFeatured).ToList(),
            _            => responses.OrderByDescending(p => p.IsFeatured)
                                     .ThenByDescending(p => p.IsPopular).ToList(),
        };

        var total = sorted.Count;
        var paged = sorted.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

        return Result<PagedList<PublicPackageResponse>>.Success(
            PagedList<PublicPackageResponse>.Create(paged, request.Page, request.PageSize, total));
    }

    private static PublicPackageResponse MapSynced(TenantPackage tp)
    {
        var m = tp.MasterPackage;
        var o = tp.Override;
        return new PublicPackageResponse(
            Id:               tp.Id,
            Title:            o?.Title ?? m.Title,
            ShortDescription: o?.ShortDescription ?? m.ShortDescription ?? m.Description ?? "",
            Category:         m.Category ?? "",
            Price:            o?.Price ?? m.BasePrice,
            Currency:         o?.Currency ?? m.Currency,
            DurationDays:     m.DurationDays,
            Destination:      m.Destination,
            Region:           m.Region ?? "",
            FeaturedImageUrl: o?.FeaturedImageUrl ?? m.FeaturedImageUrl ?? "",
            IsFeatured:       m.IsFeatured,
            IsPopular:        m.IsPopular
        );
    }

    private static PublicPackageResponse MapOwned(TenantPackage tp)
    {
        var o = tp.Override!;
        return new PublicPackageResponse(
            Id:               tp.Id,
            Title:            o.Title ?? "",
            ShortDescription: o.ShortDescription ?? "",
            Category:         o.Category ?? "",
            Price:            o.Price ?? 0,
            Currency:         o.Currency ?? "MYR",
            DurationDays:     o.DurationDays ?? 1,
            Destination:      o.Destination ?? "",
            Region:           o.Region ?? "",
            FeaturedImageUrl: o.FeaturedImageUrl ?? "",
            IsFeatured:       false,
            IsPopular:        false
        );
    }
}
