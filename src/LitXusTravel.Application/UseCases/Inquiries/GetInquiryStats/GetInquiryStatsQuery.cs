using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Inquiries.GetInquiryStats;

public record GetInquiryStatsQuery() : IRequest<Result<InquiryStatsResponse>>;

public record InquiryStatsResponse(
    Dictionary<string, int> StatusBreakdown,
    int TotalInquiries,
    double ConversionRate
);
