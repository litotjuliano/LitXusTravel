using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Inquiries.UpdateInquiryStatus;

public record UpdateInquiryStatusCommand(
    Guid TenantId,
    Guid InquiryId,
    string Status
) : IRequest<Result<string>>;
