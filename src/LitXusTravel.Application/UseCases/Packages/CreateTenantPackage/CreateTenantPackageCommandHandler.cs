using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.CreateTenantPackage;

public class CreateTenantPackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateTenantPackageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTenantPackageCommand request, CancellationToken ct)
    {
        if (request.ExtendToMaster)
        {
            // Create a master package stamped with the originating tenant
            var masterPackage = Package.Create(
                request.Title, request.Destination, request.Price,
                request.DurationDays, request.Category ?? string.Empty,
                request.Description ?? string.Empty, request.Currency);

            masterPackage.MarkCreatedByTenant(request.TenantId);
            await uow.Packages.AddAsync(masterPackage, ct);

            var tenantPackage = TenantPackage.Create(request.TenantId, masterPackage.Id);
            await uow.TenantPackages.AddAsync(tenantPackage, ct);

            await uow.SaveChangesAsync(ct);
            return Result<Guid>.Success(tenantPackage.Id);
        }
        else
        {
            // Standalone tenant-owned package — no master entry
            var tenantPackage = TenantPackage.CreateOwned(request.TenantId);
            await uow.TenantPackages.AddAsync(tenantPackage, ct);
            await uow.SaveChangesAsync(ct);

            var ownData = PackageOverride.CreateForOwned(
                request.TenantId, tenantPackage.Id,
                request.Title, request.Destination, request.DurationDays,
                request.Price, request.Currency, request.Category, request.Region,
                request.Description, request.ShortDescription,
                request.FeaturedImageUrl, request.ContactPhone, request.ContactWhatsapp);

            await uow.PackageOverrides.AddAsync(ownData, ct);
            await uow.SaveChangesAsync(ct);

            return Result<Guid>.Success(tenantPackage.Id);
        }
    }
}
