using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public enum SubscriptionStatus { Trial, Active, Expired, Suspended }

public class TenantSubscription : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string PlanName { get; private set; } = string.Empty;
    public decimal MonthlyPrice { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public int MaxPackages { get; private set; }
    public int MaxTeamMembers { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public bool AutoRenew { get; private set; }
    public bool IsTrial { get; private set; }
    public DateTime? TrialEndsAt { get; private set; }

    public bool IsActive => Status is SubscriptionStatus.Active or SubscriptionStatus.Trial
                            && (EndDate == null || EndDate > DateTime.UtcNow);

    private TenantSubscription() { }

    public static TenantSubscription CreateTrial(Guid tenantId)
        => new()
        {
            TenantId = tenantId,
            PlanName = "Trial",
            MonthlyPrice = 0,
            StartDate = DateTime.UtcNow,
            Status = SubscriptionStatus.Trial,
            IsTrial = true,
            TrialEndsAt = DateTime.UtcNow.AddDays(30),
            MaxPackages = 10,
            MaxTeamMembers = 2,
            AutoRenew = false
        };

    public static TenantSubscription Create(Guid tenantId, string planName, decimal monthlyPrice,
        int maxPackages, int maxTeamMembers)
        => new()
        {
            TenantId = tenantId,
            PlanName = planName,
            MonthlyPrice = monthlyPrice,
            StartDate = DateTime.UtcNow,
            Status = SubscriptionStatus.Active,
            IsTrial = false,
            MaxPackages = maxPackages,
            MaxTeamMembers = maxTeamMembers,
            AutoRenew = true
        };

    public void Suspend()
    {
        if (Status == SubscriptionStatus.Expired)
            throw new DomainException("Cannot suspend an expired subscription.");
        Status = SubscriptionStatus.Suspended;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Expire()
    {
        Status = SubscriptionStatus.Expired;
        EndDate = DateTime.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
