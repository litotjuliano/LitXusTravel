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
        // Get all tenant packages for this tenant
        var allTenantPackages = await uow.TenantPackages.GetAllAsync(ct);
        var tenantPackages = allTenantPackages
            .Where(tp => tp.TenantId == request.TenantId && tp.IsActive)
            .ToList();

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

        // Merge master + override using NULL semantics
        var responses = paginated.Select(tp => MergePackage(tp)).ToList();

        var pagedList = PagedList<ResolvedPackageResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<ResolvedPackageResponse>>.Success(pagedList);
    }

    private ResolvedPackageResponse MergePackage(LitXusTravel.Domain.Entities.TenantPackage tenantPackage)
    {
        var master = tenantPackage.MasterPackage;
        var @override = tenantPackage.Override;

        // NULL semantics: override.field IS NULL → use master.field, else use override.field
        return new ResolvedPackageResponse(
            Id: tenantPackage.Id,
            MasterPackageId: master.Id,
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
            LastSyncedAt: tenantPackage.LastSyncedAt ?? DateTimeOffset.UtcNow
        );
    }
}
