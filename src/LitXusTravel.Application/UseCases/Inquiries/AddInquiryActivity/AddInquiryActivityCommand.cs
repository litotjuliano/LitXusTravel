using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Inquiries.AddInquiryActivity;

public record AddInquiryActivityCommand(
    Guid TenantId,
    Guid InquiryId,
    string ActivityType,
    string Description,
    string? Notes = null
) : IRequest<Result<string>>;
