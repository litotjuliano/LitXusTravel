using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Inquiries.UpdateInquiryStatus;

public class UpdateInquiryStatusCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateInquiryStatusCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateInquiryStatusCommand request, CancellationToken ct)
    {
        var inquiry = await uow.Inquiries.GetByIdAsync(request.InquiryId, ct);
        if (inquiry is null)
            return Result<string>.Failure("Inquiry not found.");

        if (inquiry.TenantId != request.TenantId)
            return Result<string>.Failure("Unauthorized.");

        if (!Enum.TryParse<InquiryStatus>(request.Status, ignoreCase: true, out var status))
            return Result<string>.Failure("Invalid inquiry status.");

        inquiry.UpdateStatus(status);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success("Inquiry status updated successfully.");
    }
}
