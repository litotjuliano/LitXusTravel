using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Tenants.GetTenantSettings;

public record GetTenantSettingsQuery(Guid TenantId) : IRequest<Result<TenantSettingsResponse>>;

public record TenantSettingsResponse(
    Guid TenantId,
    string Name,
    string? ContactEmail,
    string? ContactPhone,
    bool IsActive,
    string ProvisioningStatus,
    string? Country,
    DateTimeOffset CreatedAt,
    string DefaultCurrency
);
