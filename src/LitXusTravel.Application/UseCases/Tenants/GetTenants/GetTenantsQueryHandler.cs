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
        var activeSubsByTenant = (await uow.TenantSubscriptions.FindAsync(s => tenantIds.Contains(s.TenantId), ct))
            .Where(s => s.IsActive)
            .GroupBy(s => s.TenantId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.StartDate).First());

        var responses = tenants.Select(t =>
        {
            activeSubsByTenant.TryGetValue(t.Id, out var activeSub);

            return new TenantListResponse(
                t.Id, t.Name, t.Slug, t.Subdomain,
                t.ContactEmail.Value, t.ContactPhone, t.LogoUrl,
                t.IsActive, t.ProvisioningStatus.ToString(), t.WebsiteUrl,
                syncedPackagesCount: 0, totalInquiries: 0, conversionRate: 0.0,
                t.CreatedAt,
                Plan: activeSub?.PlanName
            );
        }).ToList();

        var pagedList = PagedList<TenantListResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<TenantListResponse>>.Success(pagedList);
    }
}
