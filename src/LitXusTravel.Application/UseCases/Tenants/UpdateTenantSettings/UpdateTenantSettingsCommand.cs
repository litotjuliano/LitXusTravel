using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Tenants.UpdateTenantSettings;

public record UpdateTenantSettingsCommand(Guid TenantId, string DefaultCurrency)
    : IRequest<Result>;
