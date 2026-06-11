namespace LitXusTravel.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendInquiryReceivedAsync(Guid tenantId, Guid inquiryId, string customerName, CancellationToken ct = default);
    Task SendPackagePublishedAsync(Guid packageId, string packageTitle, CancellationToken ct = default);
    Task SendPackageSyncedAsync(Guid tenantId, Guid packageId, CancellationToken ct = default);
}
