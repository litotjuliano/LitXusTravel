using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Tenants.GetTenants;

public class GetTenantsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTenantsQuery, Result<PagedList<TenantListResponse>>>
{
    public async Task<Result<PagedList<TenantListResponse>>> Handle(GetTenantsQuery request, CancellationToken ct)
    {
        var query = (await uow.Tenants.GetAllAsync(ct)).AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(t => t.ProvisioningStatus.ToString() == request.Status);
        }

        var totalCount = query.Count();
        var tenants = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(t => t.Name).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(t => t.Name).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            "createdat" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(t => t.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(t => t.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            _ => query.OrderByDescending(t => t.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
        };

        var tenantIds = tenants.Select(t => t.Id).ToList();

        // Fetch all subscriptions (not just active) so expired ones are visible to admins.
        // Prefer active/trial subs; fall back to the most recent historical record.
        var allSubsByTenant = (await uow.TenantSubscriptions.FindAsync(s => tenantIds.Contains(s.TenantId), ct))
            .GroupBy(s => s.TenantId)
            .ToDictionary(g => g.Key, g =>
                g.OrderByDescending(s => s.IsActive)
                 .ThenByDescending(s => s.StartDate)
                 .First());

        var packageCountsByTenant = (await uow.TenantPackages.FindAsync(p => tenantIds.Contains(p.TenantId) && p.IsActive, ct))
            .GroupBy(p => p.TenantId)
            .ToDictionary(g => g.Key, g => g.Count());

        var responses = tenants.Select(t =>
        {
            allSubsByTenant.TryGetValue(t.Id, out var sub);
            packageCountsByTenant.TryGetValue(t.Id, out var packageCount);

            return new TenantListResponse(
                t.Id, t.Name, t.Slug, t.Subdomain,
                t.ContactEmail.Value, t.ContactPhone, t.LogoUrl,
                t.IsActive, t.ProvisioningStatus.ToString(), t.WebsiteUrl,
                syncedPackagesCount: packageCount, totalInquiries: 0, conversionRate: 0.0,
                t.CreatedAt,
                Plan:                  sub?.PlanName,
                SubscriptionHealth:    sub?.SubscriptionHealth,
                DaysRemaining:         sub?.DaysRemaining,
                SubscriptionEndDate:   sub?.EndDate,
                SubscriptionStartDate: sub?.StartDate
            );
        }).ToList();

        var pagedList = PagedList<TenantListResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<TenantListResponse>>.Success(pagedList);
    }
}
