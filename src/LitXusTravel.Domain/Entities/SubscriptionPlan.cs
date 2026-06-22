using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int MaxPackages { get; private set; }
    public int MaxTeamMembers { get; private set; }
    public bool IsActive { get; private set; } = true;

    private SubscriptionPlan() { }

    public static SubscriptionPlan Create(string name, decimal price, int maxPackages, int maxTeamMembers)
    {
        var plan = new SubscriptionPlan();
        plan.Apply(name, price, maxPackages, maxTeamMembers);
        return plan;
    }

    public void Update(string name, decimal price, int maxPackages, int maxTeamMembers)
    {
        Apply(name, price, maxPackages, maxTeamMembers);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void Apply(string name, decimal price, int maxPackages, int maxTeamMembers)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Plan name is required.");
        if (price < 0)
            throw new DomainException("Price cannot be negative.");
        if (maxPackages <= 0)
            throw new DomainException("Max packages must be greater than 0.");
        if (maxTeamMembers <= 0)
            throw new DomainException("Max team members must be greater than 0.");

        Name = name;
        Price = price;
        MaxPackages = maxPackages;
        MaxTeamMembers = maxTeamMembers;
    }
}
