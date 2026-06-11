using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Tenants.GetTenantById;

public class GetTenantByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTenantByIdQuery, Result<TenantResponse>>
{
    public async Task<Result<TenantResponse>> Handle(GetTenantByIdQuery request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<TenantResponse>.Failure("Tenant not found.");

        return Result<TenantResponse>.Success(new TenantResponse(
            tenant.Id, tenant.Name, tenant.Slug, tenant.Subdomain,
            tenant.ContactEmail.Value, tenant.ContactPhone, tenant.LogoUrl,
            tenant.IsActive, tenant.ProvisioningStatus.ToString(),
            tenant.WebsiteUrl, tenant.CreatedAt));
    }
}
