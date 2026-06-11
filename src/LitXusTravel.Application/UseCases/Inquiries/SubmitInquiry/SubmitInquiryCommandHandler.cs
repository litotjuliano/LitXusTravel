using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Inquiries.SubmitInquiry;

public class SubmitInquiryCommandHandler(IUnitOfWork uow)
    : IRequestHandler<SubmitInquiryCommand, Result<InquirySubmissionResponse>>
{
    public async Task<Result<InquirySubmissionResponse>> Handle(
        SubmitInquiryCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<InquirySubmissionResponse>.Failure("Tenant not found.");

        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<InquirySubmissionResponse>.Failure("Package not found.");

        if (package.Visibility != PackageVisibility.Published)
            return Result<InquirySubmissionResponse>.Failure("Package not available.");

        var preferredDateStr = request.PreferredDate?.ToString("yyyy-MM-dd");
        var inquiry = Inquiry.Create(
            tenantId: request.TenantId,
            customerName: request.CustomerName,
            customerEmail: request.CustomerEmail,
            customerPhone: request.CustomerPhone,
            message: request.Message,
            tenantPackageId: null,
            numberOfPax: request.TravelersCount,
            preferredTravelDates: preferredDateStr
        );

        await uow.Inquiries.AddAsync(inquiry, ct);
        await uow.SaveChangesAsync(ct);

        var response = new InquirySubmissionResponse(
            InquiryId: inquiry.Id,
            Status: inquiry.Status.ToString(),
            SubmittedAt: inquiry.CreatedAt.UtcDateTime
        );

        return Result<InquirySubmissionResponse>.Success(response);
    }
}
