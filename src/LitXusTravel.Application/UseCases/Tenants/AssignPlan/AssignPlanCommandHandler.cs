using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.AssignPlan;

public class AssignPlanCommandHandler(IUnitOfWork uow)
    : IRequestHandler<AssignPlanCommand, Result<string>>
{
    private static readonly Dictionary<string, (decimal price, int maxPackages, int maxTeamMembers)> Plans = new()
    {
        ["Starter"]    = (99,   10,  2),
        ["Pro"]        = (299,  50,  10),
        ["Enterprise"] = (999,  999, 50),
    };

    public async Task<Result<string>> Handle(AssignPlanCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Result<string>.Failure("Tenant not found.");

        if (!Plans.TryGetValue(request.PlanName, out var planConfig))
            return Result<string>.Failure($"Unknown plan '{request.PlanName}'. Valid plans: Starter, Pro, Enterprise.");

        // Expire any existing active subscriptions
        var existing = (await uow.TenantSubscriptions.FindAsync(
            s => s.TenantId == request.TenantId, ct)).ToList();

        foreach (var s in existing.Where(s => s.IsActive))
        {
            s.Expire();
            uow.TenantSubscriptions.Update(s);
        }

        var sub = TenantSubscription.Create(
            request.TenantId,
            request.PlanName,
            planConfig.price,
            planConfig.maxPackages,
            planConfig.maxTeamMembers);

        await uow.TenantSubscriptions.AddAsync(sub, ct);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success($"Plan '{request.PlanName}' assigned to tenant.");
    }
}
