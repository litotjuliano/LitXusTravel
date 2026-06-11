using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Inquiries.GetInquiries;

public record GetInquiriesQuery(
    Guid? TenantId = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "createdAt",
    string? SortOrder = "desc"
) : IRequest<Result<PagedList<InquiryResponse>>>;
