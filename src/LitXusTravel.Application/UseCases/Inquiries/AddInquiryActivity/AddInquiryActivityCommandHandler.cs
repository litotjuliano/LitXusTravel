using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Inquiries.AddInquiryActivity;

public class AddInquiryActivityCommandHandler(IUnitOfWork uow)
    : IRequestHandler<AddInquiryActivityCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddInquiryActivityCommand request, CancellationToken ct)
    {
        var inquiry = await uow.Inquiries.GetByIdAsync(request.InquiryId, ct);
        if (inquiry is null)
            return Result<string>.Failure("Inquiry not found.");

        if (inquiry.TenantId != request.TenantId)
            return Result<string>.Failure("Unauthorized.");

        if (!Enum.TryParse<ActivityType>(request.ActivityType, ignoreCase: true, out var activityType))
            return Result<string>.Failure("Invalid activity type.");

        var activity = CrmActivity.Create(
            inquiryId: request.InquiryId,
            tenantId: request.TenantId,
            type: activityType,
            notes: request.Notes
        );

        await uow.CrmActivities.AddAsync(activity, ct);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success("Activity added successfully.");
    }
}
