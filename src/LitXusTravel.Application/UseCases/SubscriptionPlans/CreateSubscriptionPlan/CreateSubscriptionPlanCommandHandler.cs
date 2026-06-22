using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.SubscriptionPlans.CreateSubscriptionPlan;

public class CreateSubscriptionPlanCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateSubscriptionPlanCommand, Result<SubscriptionPlanResponse>>
{
    public async Task<Result<SubscriptionPlanResponse>> Handle(CreateSubscriptionPlanCommand request, CancellationToken ct)
    {
        var nameTaken = await uow.SubscriptionPlans.AnyAsync(p => p.Name == request.Name, ct);
        if (nameTaken)
            return Result<SubscriptionPlanResponse>.Failure($"A plan named '{request.Name}' already exists.");

        SubscriptionPlan plan;
        try
        {
            plan = SubscriptionPlan.Create(request.Name, request.Price, request.MaxPackages, request.MaxTeamMembers);
        }
        catch (DomainException ex)
        {
            return Result<SubscriptionPlanResponse>.Failure(ex.Message);
        }

        await uow.SubscriptionPlans.AddAsync(plan, ct);
        await uow.SaveChangesAsync(ct);

        return Result<SubscriptionPlanResponse>.Success(new SubscriptionPlanResponse(
            plan.Id, plan.Name, plan.Price, plan.MaxPackages, plan.MaxTeamMembers, plan.IsActive, plan.CreatedAt));
    }
}
