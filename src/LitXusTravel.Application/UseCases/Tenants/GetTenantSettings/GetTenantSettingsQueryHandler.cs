using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Tenants.GetTenantSettings;

public class GetTenantSettingsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTenantSettingsQuery, Result<TenantSettingsResponse>>
{
    public async Task<Result<TenantSettingsResponse>> Handle(
        GetTenantSettingsQuery request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result<TenantSettingsResponse>.Failure("Tenant not found.");

        var response = new TenantSettingsResponse(
            TenantId: tenant.Id,
            Name: tenant.Name,
            ContactEmail: tenant.ContactEmail?.Value,
            ContactPhone: tenant.ContactPhone,
            IsActive: tenant.IsActive,
            ProvisioningStatus: tenant.ProvisioningStatus.ToString(),
            Country: tenant.Country,
            CreatedAt: tenant.CreatedAt,
            DefaultCurrency: tenant.DefaultCurrency
        );

        return Result<TenantSettingsResponse>.Success(response);
    }
}
