using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Inquiries.GetInquiryStats;

public class GetInquiryStatsQueryHandler(
    IUnitOfWork uow,
    ICurrentTenant currentTenant)
    : IRequestHandler<GetInquiryStatsQuery, Result<InquiryStatsResponse>>
{
    public async Task<Result<InquiryStatsResponse>> Handle(
        GetInquiryStatsQuery request, CancellationToken ct)
    {
        var inquiries = await uow.Inquiries.GetAllAsync(ct);
        var tenantInquiries = inquiries.Where(i => i.TenantId == currentTenant.Id).ToList();

        var statusBreakdown = new Dictionary<string, int>
        {
            ["New"] = tenantInquiries.Count(i => i.Status == InquiryStatus.New),
            ["Contacted"] = tenantInquiries.Count(i => i.Status == InquiryStatus.Contacted),
            ["Quoted"] = tenantInquiries.Count(i => i.Status == InquiryStatus.Quoted),
            ["Booked"] = tenantInquiries.Count(i => i.Status == InquiryStatus.Booked),
            ["Lost"] = tenantInquiries.Count(i => i.Status == InquiryStatus.Lost),
        };

        var total = tenantInquiries.Count;
        var converted = tenantInquiries.Count(i => i.Status == InquiryStatus.Booked);
        var conversionRate = total > 0 ? (converted / (double)total) * 100 : 0;

        return Result<InquiryStatsResponse>.Success(
            new InquiryStatsResponse(statusBreakdown, total, conversionRate));
    }
}
