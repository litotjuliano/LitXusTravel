using Microsoft.Extensions.Logging;
using LitXusTravel.Application.Interfaces.Services;

namespace LitXusTravel.Infrastructure.Services;

public class NotificationService(ILogger<NotificationService> logger) : INotificationService
{
    public Task SendInquiryReceivedAsync(Guid tenantId, Guid inquiryId, string customerName, CancellationToken ct = default)
    {
        logger.LogInformation("Notification: New inquiry {InquiryId} from {CustomerName} for tenant {TenantId}",
            inquiryId, customerName, tenantId);
        // Email / push notification integration goes here
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
}
