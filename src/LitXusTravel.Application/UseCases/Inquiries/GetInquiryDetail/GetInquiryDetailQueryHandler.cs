using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Inquiries.GetInquiryDetail;

public class GetInquiryDetailQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetInquiryDetailQuery, Result<InquiryDetailResponse>>
{
    public async Task<Result<InquiryDetailResponse>> Handle(
        GetInquiryDetailQuery request, CancellationToken ct)
    {
        var inquiry = await uow.Inquiries.GetByIdAsync(request.InquiryId, ct);
        if (inquiry is null || inquiry.TenantId != request.TenantId)
            return Result<InquiryDetailResponse>.Failure("Inquiry not found.");

        var response = new InquiryDetailResponse(
            Id: inquiry.Id,
            TenantId: inquiry.TenantId,
            CustomerName: inquiry.CustomerName,
            CustomerEmail: inquiry.CustomerEmail,
            CustomerPhone: inquiry.CustomerPhone,
            Message: inquiry.Message,
            Status: inquiry.Status.ToString(),
            NumberOfPax: inquiry.NumberOfPax,
            PreferredTravelDates: inquiry.PreferredTravelDates,
            WhatsAppGroupUrl: inquiry.WhatsAppGroupUrl,
            CreatedAt: inquiry.CreatedAt.UtcDateTime,
            FirstResponseAt: inquiry.FirstResponseAt?.UtcDateTime,
            QuotedAt: inquiry.QuotedAt?.UtcDateTime,
            ClosedAt: inquiry.ClosedAt?.UtcDateTime
        );

        return Result<InquiryDetailResponse>.Success(response);
    }
}
