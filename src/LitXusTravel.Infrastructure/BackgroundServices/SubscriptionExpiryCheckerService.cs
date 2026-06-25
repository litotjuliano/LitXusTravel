using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Data.Contexts;

namespace LitXusTravel.Infrastructure.BackgroundServices;

public class SubscriptionExpiryCheckerService(
    IServiceScopeFactory scopeFactory,
    ILogger<SubscriptionExpiryCheckerService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        do
        {
            try { await CheckAndNotifyAsync(stoppingToken); }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "[SubscriptionExpiryChecker] Error during check cycle");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CheckAndNotifyAsync(CancellationToken ct)
    {
        logger.LogInformation("[SubscriptionExpiryChecker] Running subscription health check");

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<LitXusTravelDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var subs = await db.TenantSubscriptions.AsNoTracking().ToListAsync(ct);
        var tenantIds = subs.Select(s => s.TenantId).Distinct().ToList();
        var tenants = await db.Tenants
            .Where(t => tenantIds.Contains(t.Id))
            .AsNoTracking()
            .ToDictionaryAsync(t => t.Id, ct);

        foreach (var sub in subs)
        {
            if (!tenants.TryGetValue(sub.TenantId, out var tenant)) continue;

            if (sub.Status == SubscriptionStatus.Active && sub.DaysRemaining is >= 6 and <= 7)
            {
                if (!await AlreadySentAsync(db, "subscription_expiring_soon", tenant.Id, ct))
                {
                    await notificationService.SendExpiringWarningAsync(
                        tenant.Id, tenant.Name, sub.PlanName, sub.DaysRemaining!.Value, ct);
                    logger.LogInformation("[SubscriptionExpiryChecker] Expiry warning → {Tenant}", tenant.Name);
                }
            }
            else if (sub.IsInGracePeriod)
            {
                if (!await AlreadySentAsync(db, "subscription_grace_period", tenant.Id, ct))
                {
                    await notificationService.SendGracePeriodStartedAsync(
                        tenant.Id, tenant.Name, sub.PlanName, ct);
                    logger.LogInformation("[SubscriptionExpiryChecker] Grace period → {Tenant}", tenant.Name);
                }
            }
            else if (sub.IsReadOnly)
            {
                if (!await AlreadySentAsync(db, "subscription_fully_expired", tenant.Id, ct))
                {
                    await notificationService.SendFullyExpiredAsync(
                        tenant.Id, tenant.Name, sub.PlanName, ct);
                    logger.LogInformation("[SubscriptionExpiryChecker] Fully expired → {Tenant}", tenant.Name);
                }
            }
        }
    }

    private static async Task<bool> AlreadySentAsync(
        LitXusTravelDbContext db, string type, Guid tenantId, CancellationToken ct)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-8);
        return await db.Notifications
            .AnyAsync(n => n.Type == type
                        && n.RelatedEntityId == tenantId
                        && n.CreatedAt >= cutoff, ct);
    }
}
