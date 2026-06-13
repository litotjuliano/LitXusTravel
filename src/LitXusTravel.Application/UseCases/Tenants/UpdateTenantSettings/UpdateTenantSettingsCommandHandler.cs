using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Tenants.UpdateTenantSettings;

public class UpdateTenantSettingsCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateTenantSettingsCommand, Result>
{
    public async Task<Result> Handle(UpdateTenantSettingsCommand request, CancellationToken ct)
    {
        var tenant = await uow.Tenants.GetByIdAsync(request.TenantId, ct);
        if (tenant is null)
            return Result.Failure("Tenant not found.");

        tenant.UpdateSettings(request.DefaultCurrency);
        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
