using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LitXusTravel.Application.UseCases.Tenants.ProcessMockPayment;

public class ProcessMockPaymentCommandHandler(
    IUnitOfWork uow,
    INotificationService notificationService,
    ILogger<ProcessMockPaymentCommandHandler> logger)
    : IRequestHandler<ProcessMockPaymentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ProcessMockPaymentCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Result<string>.Failure("Tenant not found.");

        var plan = await uow.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == request.PlanName && p.IsActive, ct);
        if (plan is null)
            return Result<string>.Failure($"Unknown or inactive plan '{request.PlanName}'.");

        var existing = (await uow.TenantSubscriptions.FindAsync(s => s.TenantId == request.TenantId, ct)).ToList();
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

        sub.SetEndDate(DateTime.UtcNow.AddYears(1));

        await uow.TenantSubscriptions.AddAsync(sub, ct);
        await uow.SaveChangesAsync(ct);

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        var invoice = Invoice.Create(
            request.TenantId,
            tenant.Name,
            invoiceNumber,
            plan.Name,
            plan.Price,
            DateTime.UtcNow.ToString("MMM yyyy"),
            "Paid",
            DateTime.UtcNow);
        await uow.Invoices.AddAsync(invoice, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation(
            "[MockPayment] Subscription renewed for TenantId: {TenantId}, Plan: {PlanName}, ExpiresAt: {ExpiresAt}",
            request.TenantId, plan.Name, sub.EndDate);

        await notificationService.SendSubscriptionRenewedAsync(
            request.TenantId, tenant.Name, plan.Name, ct);

        return Result<string>.Success("Mock payment processed. Subscription renewed for 1 year.");
    }
}
