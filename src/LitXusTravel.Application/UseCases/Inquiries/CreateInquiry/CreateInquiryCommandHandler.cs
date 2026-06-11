using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Inquiries.CreateInquiry;

public class CreateInquiryCommandHandler(
    IUnitOfWork uow,
    INotificationService notifications,
    IAuditService audit)
    : IRequestHandler<CreateInquiryCommand, Result<InquiryResponse>>
{
    public async Task<Result<InquiryResponse>> Handle(CreateInquiryCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<InquiryResponse>.Failure("Tenant not found.");

        if (request.TenantPackageId.HasValue)
        {
            var packageExists = await uow.TenantPackages.AnyAsync(
                tp => tp.Id == request.TenantPackageId && tp.TenantId == request.TenantId, ct);

            if (!packageExists)
                return Result<InquiryResponse>.Failure("Package not found for this tenant.");
        }

        var inquiry = Inquiry.Create(
            request.TenantId, request.CustomerName, request.CustomerEmail,
            request.CustomerPhone, request.Message, request.TenantPackageId,
            request.NumberOfPax, request.PreferredTravelDates);

        var activity = CrmActivity.Create(inquiry.Id, request.TenantId, ActivityType.Created);

        await uow.Inquiries.AddAsync(inquiry, ct);
        await uow.CrmActivities.AddAsync(activity, ct);
        await uow.SaveChangesAsync(ct);

        await notifications.SendInquiryReceivedAsync(
            request.TenantId, inquiry.Id, request.CustomerName, ct);

        await audit.LogAsync(AuditAction.Created, nameof(Inquiry), inquiry.Id,
            tenantId: request.TenantId,
            newValues: new { inquiry.CustomerName, inquiry.CustomerEmail }, ct: ct);

        return Result<InquiryResponse>.Success(MapToResponse(inquiry));
    }

    private static InquiryResponse MapToResponse(Inquiry i) => new(
        i.Id, i.TenantId, i.TenantPackageId, i.CustomerName, i.CustomerEmail,
        i.CustomerPhone, i.Message, i.NumberOfPax, i.PreferredTravelDates,
        i.Status.ToString(), i.FirstResponseAt, i.QuotedAt, i.ClosedAt,
        i.CreatedAt, i.UpdatedAt);
}
