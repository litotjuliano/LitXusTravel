using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Inquiries.SubmitInquiry;

public record SubmitInquiryCommand(
    Guid TenantId,
    Guid PackageId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Message,
    int TravelersCount,
    DateTime? PreferredDate = null
) : IRequest<Result<InquirySubmissionResponse>>;

public record InquirySubmissionResponse(
    Guid InquiryId,
    string Status,
    DateTime SubmittedAt
);
