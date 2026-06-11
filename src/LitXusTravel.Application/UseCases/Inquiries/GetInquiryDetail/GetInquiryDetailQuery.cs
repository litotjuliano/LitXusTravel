using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Inquiries.GetInquiryDetail;

public record GetInquiryDetailQuery(Guid TenantId, Guid InquiryId) : IRequest<Result<InquiryDetailResponse>>;

public record InquiryDetailResponse(
    Guid Id,
    Guid TenantId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Message,
    string Status,
    int? NumberOfPax,
    string? PreferredTravelDates,
    string? WhatsAppGroupUrl,
    DateTime CreatedAt,
    DateTime? FirstResponseAt,
    DateTime? QuotedAt,
    DateTime? ClosedAt
);
