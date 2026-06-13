using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Packages.GetMarketplacePackages;

public class GetMarketplacePackagesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMarketplacePackagesQuery, Result<IReadOnlyList<MarketplacePackageResponse>>>
{
    public async Task<Result<IReadOnlyList<MarketplacePackageResponse>>> Handle(
        GetMarketplacePackagesQuery request, CancellationToken ct)
    {
        var allPackages = await uow.Packages.GetAllAsync(ct);
        var tenantPackages = await uow.TenantPackages.GetAllAsync(ct);
        var allTenants = (await uow.Tenants.GetAllAsync(ct)).ToDictionary(t => t.Id, t => t.Name);

        var alreadySyncedIds = tenantPackages
            .Where(tp => tp.TenantId == request.TenantId)
            .Select(tp => tp.MasterPackageId)
            .ToHashSet();

        var marketplace = allPackages
            .Where(p =>
                p.CreatedByTenantId != null &&
                p.CreatedByTenantId != request.TenantId &&
                !alreadySyncedIds.Contains(p.Id))
            .Select(p => new MarketplacePackageResponse(
                Id: p.Id,
                Title: p.Title,
                Category: p.Category,
                Destination: p.Destination,
                Region: p.Region,
                BasePrice: p.BasePrice,
                Currency: p.Currency,
                DurationDays: p.DurationDays,
                FeaturedImageUrl: p.FeaturedImageUrl,
                SourceTenantName: allTenants.TryGetValue(p.CreatedByTenantId!.Value, out var name) ? name : "Unknown"
            ))
            .ToList();

        return Result<IReadOnlyList<MarketplacePackageResponse>>.Success(marketplace);
    }
}
