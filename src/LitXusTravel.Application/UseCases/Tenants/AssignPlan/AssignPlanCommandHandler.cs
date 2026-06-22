using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.AssignPlan;

public class AssignPlanCommandHandler(IUnitOfWork uow)
    : IRequestHandler<AssignPlanCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AssignPlanCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Result<string>.Failure("Tenant not found.");

        var plan = await uow.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == request.PlanName && p.IsActive, ct);
        if (plan is null)
            return Result<string>.Failure($"Unknown or inactive plan '{request.PlanName}'.");

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
            plan.Name,
            plan.Price,
            plan.MaxPackages,
            plan.MaxTeamMembers);

        await uow.TenantSubscriptions.AddAsync(sub, ct);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success($"Plan '{plan.Name}' assigned to tenant.");
    }
}
