using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.DeleteSubscriptionPlan;

public class DeleteSubscriptionPlanCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteSubscriptionPlanCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteSubscriptionPlanCommand request, CancellationToken ct)
    {
        var plan = await uow.SubscriptionPlans.GetByIdAsync(request.Id, ct);
        if (plan is null)
            return Result<string>.Failure("Subscription plan not found.");

        plan.SoftDelete();
        uow.SubscriptionPlans.Update(plan);
        await uow.SaveChangesAsync(ct);

        return Result<string>.Success("Plan deleted.");
    }
}
