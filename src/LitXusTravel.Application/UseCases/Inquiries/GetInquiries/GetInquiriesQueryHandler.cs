using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Inquiries.GetInquiries;

public class GetInquiriesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetInquiriesQuery, Result<PagedList<InquiryResponse>>>
{
    public async Task<Result<PagedList<InquiryResponse>>> Handle(GetInquiriesQuery request, CancellationToken ct)
    {
        var query = (await uow.Inquiries.GetAllAsync(ct)).AsQueryable();

        if (request.TenantId.HasValue)
        {
            query = query.Where(i => i.TenantId == request.TenantId.Value);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(i => i.Status.ToString() == request.Status);
        }

        var totalCount = query.Count();
        var inquiries = request.SortBy?.ToLower() switch
        {
            "updatedat" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(i => i.UpdatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(i => i.UpdatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            "createdat" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(i => i.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : query.OrderByDescending(i => i.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            _ => query.OrderByDescending(i => i.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
        };

        var responses = inquiries.Select(i => new InquiryResponse(
            i.Id,
            i.TenantId,
            i.TenantPackageId,
            i.CustomerName,
            i.CustomerEmail,
            i.CustomerPhone,
            i.Message,
            i.NumberOfPax,
            i.PreferredTravelDates,
            i.Status.ToString(),
            i.FirstResponseAt,
            i.QuotedAt,
            i.ClosedAt,
            i.CreatedAt,
            i.UpdatedAt
        )).ToList();

        var pagedList = PagedList<InquiryResponse>.Create(responses, request.Page, request.PageSize, totalCount);
        return Result<PagedList<InquiryResponse>>.Success(pagedList);
    }
}
