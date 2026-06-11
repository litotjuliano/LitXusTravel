using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Packages.GetPackages;

public class GetPackagesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPackagesQuery, Result<PagedList<PackageListResponse>>>
{
    public async Task<Result<PagedList<PackageListResponse>>> Handle(GetPackagesQuery request, CancellationToken ct)
    {
        var query = (await uow.Packages.GetAllAsync(ct)).AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(p => p.Visibility.ToString() == request.Status);
        }

        if (!string.IsNullOrEmpty(request.Destination))
        {
            query = query.Where(p => p.Destination.Contains(request.Destination));
        }

        var totalCount = query.Count();
        var packages = request.SortBy?.ToLower() switch
        {
            "title" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.Title).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(p => p.Title).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            "price" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.BasePrice).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(p => p.BasePrice).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            "createdat" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(p => p.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            _ => query.OrderByDescending(p => p.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
        };

        var responses = packages.Select(p => new PackageListResponse(
            p.Id,
            p.Title,
            p.Category,
            p.Destination,
            p.BasePrice,
            p.Currency,
            p.DurationDays,
            p.Visibility.ToString(),
            syncedTenantsCount: 0,
            p.CreatedAt,
            p.UpdatedAt
        )).ToList();

        var pagedList = PagedList<PackageListResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<PackageListResponse>>.Success(pagedList);
    }
}
