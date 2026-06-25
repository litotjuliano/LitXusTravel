using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Identity;

namespace LitXusTravel.Infrastructure.Services;

public class NotificationService(
    ILogger<NotificationService> logger,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork uow) : INotificationService
{
    public Task SendInquiryReceivedAsync(Guid tenantId, Guid inquiryId, string customerName, CancellationToken ct = default)
    {
        logger.LogInformation("Notification: New inquiry {InquiryId} from {CustomerName} for tenant {TenantId}",
            inquiryId, customerName, tenantId);
        return Task.CompletedTask;
    }

    public Task SendPackagePublishedAsync(Guid packageId, string packageTitle, CancellationToken ct = default)
    {
        logger.LogInformation("Notification: Package {PackageId} '{Title}' published", packageId, packageTitle);
        return Task.CompletedTask;
    }

    public Task SendPackageSyncedAsync(Guid tenantId, Guid packageId, CancellationToken ct = default)
    {
        logger.LogInformation("Notification: Package {PackageId} synced to tenant {TenantId}", packageId, tenantId);
        return Task.CompletedTask;
    }

    public async Task SendSubscriptionRenewedAsync(Guid tenantId, string tenantName, string planName, CancellationToken ct = default)
    {
        var superAdmins = await userManager.GetUsersInRoleAsync("SuperAdmin");
        var admins      = await userManager.GetUsersInRoleAsync("Admin");
        var recipients  = superAdmins
            .Concat(admins.Where(u => u.TenantId == null || u.TenantId == tenantId))
            .DistinctBy(u => u.Id).ToList();

        foreach (var user in recipients)
        {
            var notification = Notification.Create(
                type: "subscription_renewed",
                title: "Subscription Renewed",
                message: $"{tenantName} has successfully renewed their {planName} subscription.",
                tenantId: null,
                userId: user.Id,
                relatedEntityId: tenantId,
                relatedEntityType: "Tenant");

            await uow.Notifications.AddAsync(notification, ct);
        }

        await uow.SaveChangesAsync(ct);

        logger.LogInformation(
            "[Notification] subscription_renewed sent to {Count} admin(s) for TenantId {TenantId}",
            recipients.Count, tenantId);
    }

    public async Task SendExpiringWarningAsync(Guid tenantId, string tenantName, string planName, int daysRemaining, CancellationToken ct = default)
    {
        await SendToAdminsAsync(
            type: "subscription_expiring_soon",
            title: "Subscription Expiring Soon",
            message: $"{tenantName}'s {planName} subscription expires in {daysRemaining} day{(daysRemaining == 1 ? "" : "s")}.",
            tenantId: tenantId,
            ct: ct);
    }

    public async Task SendGracePeriodStartedAsync(Guid tenantId, string tenantName, string planName, CancellationToken ct = default)
    {
        await SendToAdminsAsync(
            type: "subscription_grace_period",
            title: "Subscription Grace Period",
            message: $"{tenantName}'s {planName} subscription has expired. They have 7 days to renew before access is revoked.",
            tenantId: tenantId,
            ct: ct);
    }

    public async Task SendFullyExpiredAsync(Guid tenantId, string tenantName, string planName, CancellationToken ct = default)
    {
        await SendToAdminsAsync(
            type: "subscription_fully_expired",
            title: "Subscription Fully Expired",
            message: $"{tenantName}'s {planName} subscription has fully expired. Access has been revoked.",
            tenantId: tenantId,
            ct: ct);
    }

    private async Task SendToAdminsAsync(string type, string title, string message, Guid tenantId, CancellationToken ct)
    {
        var superAdmins = await userManager.GetUsersInRoleAsync("SuperAdmin");
        var admins      = await userManager.GetUsersInRoleAsync("Admin");
        // Tenant-scoped admins (TenantId != null) only receive notifications about their own tenant.
        // Platform admins (TenantId == null) and SuperAdmins receive all.
        var recipients  = superAdmins
            .Concat(admins.Where(u => u.TenantId == null || u.TenantId == tenantId))
            .DistinctBy(u => u.Id).ToList();

        foreach (var user in recipients)
        {
            var notification = Notification.Create(
                type: type,
                title: title,
                message: message,
                tenantId: null,
                userId: user.Id,
                relatedEntityId: tenantId,
                relatedEntityType: "Tenant");

            await uow.Notifications.AddAsync(notification, ct);
        }

        await uow.SaveChangesAsync(ct);

        logger.LogInformation(
            "[Notification] {Type} sent to {Count} admin(s) for TenantId {TenantId}",
            type, recipients.Count, tenantId);
    }
}
