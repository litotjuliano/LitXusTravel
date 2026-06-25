using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.SendSubscriptionNotification;

public class SendSubscriptionNotificationCommandHandler(
    IUnitOfWork uow,
    INotificationService notificationService)
    : IRequestHandler<SendSubscriptionNotificationCommand, Result>
{
    public async Task<Result> Handle(SendSubscriptionNotificationCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Result.Failure("Tenant not found.");

        var subs = (await uow.TenantSubscriptions.FindAsync(
            s => s.TenantId == request.TenantId, ct)).ToList();

        var activeSub = subs.OrderByDescending(s => s.StartDate).FirstOrDefault();
        var planName = activeSub?.PlanName ?? "Unknown";
        var daysRemaining = activeSub?.DaysRemaining ?? 0;

        switch (request.Type)
        {
            case "expiring_soon":
                await notificationService.SendExpiringWarningAsync(
                    tenant.Id, tenant.Name, planName, daysRemaining, ct);
                break;

            case "grace_period":
                await notificationService.SendGracePeriodStartedAsync(
                    tenant.Id, tenant.Name, planName, ct);
                break;

            case "fully_expired":
                await notificationService.SendFullyExpiredAsync(
                    tenant.Id, tenant.Name, planName, ct);
                break;

            default:
                return Result.Failure($"Unknown notification type '{request.Type}'. Valid: expiring_soon, grace_period, fully_expired.");
        }

        return Result.Success();
    }
}
