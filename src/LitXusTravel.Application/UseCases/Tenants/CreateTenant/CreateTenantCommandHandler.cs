using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Tenants.CreateTenant;

public class CreateTenantCommandHandler(
    IUnitOfWork uow,
    IWebsiteProvisioner provisioner,
    IAuditService audit)
    : IRequestHandler<CreateTenantCommand, Result<TenantResponse>>
{
    public async Task<Result<TenantResponse>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var existing = await uow.Tenants
            .FirstOrDefaultAsync(t => t.ContactEmail.Value == request.Email.ToLowerInvariant(), ct);

        if (existing is not null)
            return Result<TenantResponse>.Failure("A tenant with this email already exists.");

        var tenant = Tenant.Create(request.Name, request.Email, request.Phone, request.Country);

        var trial = TenantSubscription.CreateTrial(tenant.Id);
        tenant.Subscriptions.Add(trial);

        await uow.Tenants.AddAsync(tenant, ct);
        await uow.SaveChangesAsync(ct);

        // Provision website (generates subdomain, non-blocking failure acceptable)
        try
        {
            var subdomain = await provisioner.ProvisionForTenantAsync(tenant.Id, tenant.Name, ct);
            tenant.AssignSubdomain(subdomain);
            tenant.MarkProvisioningComplete();
        }
        catch
        {
            tenant.MarkProvisioningFailed();
        }

        await uow.SaveChangesAsync(ct);

        await audit.LogAsync(AuditAction.Created, nameof(Tenant), tenant.Id,
            newValues: new { tenant.Name, tenant.ContactEmail.Value }, ct: ct);

        return Result<TenantResponse>.Success(MapToResponse(tenant));
    }

    private static TenantResponse MapToResponse(Tenant t) => new(
        t.Id, t.Name, t.Slug, t.Subdomain, t.ContactEmail.Value,
        t.ContactPhone, t.LogoUrl, t.IsActive,
        t.ProvisioningStatus.ToString(), t.WebsiteUrl, t.CreatedAt);
}
