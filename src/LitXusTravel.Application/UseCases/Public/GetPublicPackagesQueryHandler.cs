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
        var syncedMasterIds = new HashSet<Guid>();

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
                // Don't show other tenants' packages on this tenant's public website
                if (tp.MasterPackage.CreatedByTenantId != null
                    && tp.MasterPackage.CreatedByTenantId != tenantInfo.Value.Id)
                    continue;

                syncedMasterIds.Add(tp.MasterPackage.Id);
                response = MapSynced(tp);
            }

            if (!PassesFilters(response, request)) continue;
            responses.Add(response);
        }

        // Include published admin packages not yet synced to this tenant
        var adminPackages = await uow.Packages.FindAsync(
            p => p.CreatedByTenantId == null && p.Visibility == PackageVisibility.Published, ct);

        foreach (var pkg in adminPackages)
        {
            if (syncedMasterIds.Contains(pkg.Id)) continue; // already shown via TenantPackage

            var response = MapAdminPackage(pkg);
            if (!PassesFilters(response, request)) continue;
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

    private static bool PassesFilters(PublicPackageResponse r, GetPublicPackagesQuery req)
    {
        if (!string.IsNullOrEmpty(req.Category) &&
            !string.Equals(r.Category, req.Category, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(req.Destination) &&
            !r.Destination.Contains(req.Destination, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
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

    private static PublicPackageResponse MapAdminPackage(Package p)
    {
        return new PublicPackageResponse(
            Id:               p.Id,
            Title:            p.Title,
            ShortDescription: p.ShortDescription ?? p.Description ?? "",
            Category:         p.Category ?? "",
            Price:            p.BasePrice,
            Currency:         p.Currency,
            DurationDays:     p.DurationDays,
            Destination:      p.Destination,
            Region:           p.Region ?? "",
            FeaturedImageUrl: p.FeaturedImageUrl ?? "",
            IsFeatured:       p.IsFeatured,
            IsPopular:        p.IsPopular
        );
    }
}
