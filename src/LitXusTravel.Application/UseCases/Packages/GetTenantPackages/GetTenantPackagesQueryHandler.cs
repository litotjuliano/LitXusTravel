using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.GetTenantPackages;

public class GetTenantPackagesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTenantPackagesQuery, Result<PagedList<ResolvedPackageResponse>>>
{
    public async Task<Result<PagedList<ResolvedPackageResponse>>> Handle(GetTenantPackagesQuery request, CancellationToken ct)
    {
        var tenantPackages = (await uow.TenantPackages
            .GetByTenantWithDetailsAsync(request.TenantId, ct)).ToList();

        // Exclude packages from other tenants — only show: portal-only, system, and this tenant's own extended
        tenantPackages = tenantPackages.Where(tp =>
            tp.IsOwnedPackage ||
            tp.MasterPackage?.CreatedByTenantId == null ||
            tp.MasterPackage?.CreatedByTenantId == request.TenantId
        ).ToList();

        if (tenantPackages.Count == 0)
            return Result<PagedList<ResolvedPackageResponse>>.Success(
                PagedList<ResolvedPackageResponse>.Create([], request.Page, request.PageSize, 0));

        var totalCount = tenantPackages.Count;

        // Apply sorting (owned packages use Override for title/price; extended/synced use MasterPackage)
        static string TitleOf(LitXusTravel.Domain.Entities.TenantPackage tp)
            => tp.IsOwnedPackage ? (tp.Override?.Title ?? "") : (tp.MasterPackage?.Title ?? "");
        static decimal PriceOf(LitXusTravel.Domain.Entities.TenantPackage tp)
            => tp.IsOwnedPackage ? (tp.Override?.Price ?? 0) : (tp.MasterPackage?.BasePrice ?? 0);

        var sorted = request.SortBy?.ToLower() switch
        {
            "title" => request.SortOrder?.ToLower() == "asc"
                ? tenantPackages.OrderBy(TitleOf).ToList()
                : tenantPackages.OrderByDescending(TitleOf).ToList(),
            "price" => request.SortOrder?.ToLower() == "asc"
                ? tenantPackages.OrderBy(PriceOf).ToList()
                : tenantPackages.OrderByDescending(PriceOf).ToList(),
            "syncedat" => request.SortOrder?.ToLower() == "asc"
                ? tenantPackages.OrderBy(tp => tp.LastSyncedAt).ToList()
                : tenantPackages.OrderByDescending(tp => tp.LastSyncedAt).ToList(),
            _ => tenantPackages.OrderByDescending(tp => tp.LastSyncedAt).ToList(),
        };

        // Apply pagination
        var paginated = sorted
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build tenant name lookup for SyncSource resolution
        var allTenants = (await uow.Tenants.GetAllAsync(ct))
            .ToDictionary(t => t.Id, t => t.Name);

        // Merge master + override using NULL semantics
        var responses = paginated.Select(tp => MergePackage(tp, allTenants)).ToList();

        var pagedList = PagedList<ResolvedPackageResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<ResolvedPackageResponse>>.Success(pagedList);
    }

    private ResolvedPackageResponse MergePackage(
        LitXusTravel.Domain.Entities.TenantPackage tenantPackage,
        Dictionary<Guid, string> tenantNames)
    {
        if (tenantPackage.IsOwnedPackage)
        {
            var own = tenantPackage.Override!;
            return new ResolvedPackageResponse(
                Id: tenantPackage.Id,
                MasterPackageId: null,
                IsOwnedPackage: true,
                Visibility: "Published",
                Title: own.Title ?? string.Empty,
                Description: own.Description,
                ShortDescription: own.ShortDescription,
                Category: own.Category,
                Price: own.Price ?? 0,
                Currency: own.Currency ?? "MYR",
                DurationDays: own.DurationDays ?? 0,
                Destination: own.Destination ?? string.Empty,
                Region: own.Region,
                FeaturedImageUrl: own.FeaturedImageUrl,
                ImagesJson: own.ImagesJson,
                ItineraryJson: null,
                HighlightsJson: null,
                InclusionsJson: null,
                ExclusionsJson: null,
                ContactPhone: own.ContactPhone,
                ContactWhatsapp: own.ContactWhatsapp,
                IsCustomized: true,
                LastSyncedAt: tenantPackage.LastSyncedAt ?? DateTimeOffset.UtcNow,
                SyncSource: null
            );
        }

        var master = tenantPackage.MasterPackage!;
        var @override = tenantPackage.Override;

        // Packages extended to the catalog BY this tenant should display as "Owned"
        var createdByCurrentTenant = master.CreatedByTenantId == tenantPackage.TenantId;

        string? syncSource = null;
        if (!createdByCurrentTenant)
        {
            if (master.CreatedByTenantId.HasValue
                && tenantNames.TryGetValue(master.CreatedByTenantId.Value, out var cn))
                syncSource = cn;
            else
                syncSource = "System";
        }

        return new ResolvedPackageResponse(
            Id: tenantPackage.Id,
            MasterPackageId: master.Id,
            IsOwnedPackage: createdByCurrentTenant,
            Visibility: master.Visibility.ToString(),
            Title: @override?.Title ?? master.Title,
            Description: @override?.Description ?? master.Description,
            ShortDescription: @override?.ShortDescription ?? master.ShortDescription,
            Category: master.Category,
            Price: @override?.Price ?? master.BasePrice,
            Currency: @override?.Currency ?? master.Currency,
            DurationDays: master.DurationDays,
            Destination: master.Destination,
            Region: master.Region,
            FeaturedImageUrl: @override?.FeaturedImageUrl ?? master.FeaturedImageUrl,
            ImagesJson: @override?.ImagesJson ?? master.ImagesJson,
            ItineraryJson: master.ItineraryJson,
            HighlightsJson: master.HighlightsJson,
            InclusionsJson: master.InclusionsJson,
            ExclusionsJson: master.ExclusionsJson,
            ContactPhone: @override?.ContactPhone,
            ContactWhatsapp: @override?.ContactWhatsapp,
            IsCustomized: tenantPackage.IsCustomized,
            LastSyncedAt: tenantPackage.LastSyncedAt ?? DateTimeOffset.UtcNow,
            SyncSource: createdByCurrentTenant ? null : syncSource
        );
    }
}
