using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Packages.GetTenantPackages;

public record GetTenantPackagesQuery(
    Guid TenantId,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "syncedAt",
    string? SortOrder = "desc"
) : IRequest<Result<PagedList<ResolvedPackageResponse>>>;
