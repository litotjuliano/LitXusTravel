namespace LitXusTravel.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendInquiryReceivedAsync(Guid tenantId, Guid inquiryId, string customerName, CancellationToken ct = default);
    Task SendPackagePublishedAsync(Guid packageId, string packageTitle, CancellationToken ct = default);
    Task SendPackageSyncedAsync(Guid tenantId, Guid packageId, CancellationToken ct = default);
    Task SendSubscriptionRenewedAsync(Guid tenantId, string tenantName, string planName, CancellationToken ct = default);

    // Subscription lifecycle notifications
    Task SendExpiringWarningAsync(Guid tenantId, string tenantName, string planName, int daysRemaining, CancellationToken ct = default);
    Task SendGracePeriodStartedAsync(Guid tenantId, string tenantName, string planName, CancellationToken ct = default);
    Task SendFullyExpiredAsync(Guid tenantId, string tenantName, string planName, CancellationToken ct = default);
}
