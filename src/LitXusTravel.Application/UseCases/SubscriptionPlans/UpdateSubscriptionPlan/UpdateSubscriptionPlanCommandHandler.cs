using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.UpdateSubscriptionPlan;

public class UpdateSubscriptionPlanCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateSubscriptionPlanCommand, Result<SubscriptionPlanResponse>>
{
    public async Task<Result<SubscriptionPlanResponse>> Handle(UpdateSubscriptionPlanCommand request, CancellationToken ct)
    {
        var plan = await uow.SubscriptionPlans.GetByIdAsync(request.Id, ct);
        if (plan is null)
            return Result<SubscriptionPlanResponse>.Failure("Subscription plan not found.");

        var nameTaken = await uow.SubscriptionPlans.AnyAsync(p => p.Name == request.Name && p.Id != request.Id, ct);
        if (nameTaken)
            return Result<SubscriptionPlanResponse>.Failure($"A plan named '{request.Name}' already exists.");

        try
        {
            plan.Update(request.Name, request.Price, request.MaxPackages, request.MaxTeamMembers);
        }
        catch (DomainException ex)
        {
            return Result<SubscriptionPlanResponse>.Failure(ex.Message);
        }

        uow.SubscriptionPlans.Update(plan);
        await uow.SaveChangesAsync(ct);

        return Result<SubscriptionPlanResponse>.Success(new SubscriptionPlanResponse(
            plan.Id, plan.Name, plan.Price, plan.MaxPackages, plan.MaxTeamMembers, plan.IsActive, plan.CreatedAt));
    }
}
