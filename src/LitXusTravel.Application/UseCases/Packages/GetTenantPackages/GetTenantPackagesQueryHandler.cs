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

        if (tenantPackages.Count == 0)
            return Result<PagedList<ResolvedPackageResponse>>.Success(
                PagedList<ResolvedPackageResponse>.Create([], request.Page, request.PageSize, 0));

        var totalCount = tenantPackages.Count;

        // Apply sorting
        var sorted = request.SortBy?.ToLower() switch
        {
            "title" => request.SortOrder?.ToLower() == "asc"
                ? tenantPackages.OrderBy(tp => tp.MasterPackage.Title).ToList()
                : tenantPackages.OrderByDescending(tp => tp.MasterPackage.Title).ToList(),
            "price" => request.SortOrder?.ToLower() == "asc"
                ? tenantPackages.OrderBy(tp => tp.MasterPackage.BasePrice).ToList()
                : tenantPackages.OrderByDescending(tp => tp.MasterPackage.BasePrice).ToList(),
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

        var syncSource = master.CreatedByTenantId.HasValue
            && tenantNames.TryGetValue(master.CreatedByTenantId.Value, out var creatorName)
            ? creatorName
            : "System";

        return new ResolvedPackageResponse(
            Id: tenantPackage.Id,
            MasterPackageId: master.Id,
            IsOwnedPackage: false,
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
            SyncSource: syncSource
        );
    }
}
