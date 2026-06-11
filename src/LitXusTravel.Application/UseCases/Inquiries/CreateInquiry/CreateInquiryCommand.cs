using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Inquiries.CreateInquiry;

public record CreateInquiryCommand(
    Guid TenantId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Message,
    Guid? TenantPackageId,
    int? NumberOfPax,
    string? PreferredTravelDates
) : IRequest<Result<InquiryResponse>>;
