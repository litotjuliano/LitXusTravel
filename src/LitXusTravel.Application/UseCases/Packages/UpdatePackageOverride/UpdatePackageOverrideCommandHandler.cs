using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;

namespace LitXusTravel.Application.UseCases.Packages.UpdatePackageOverride;

public class UpdatePackageOverrideCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdatePackageOverrideCommand, Result<ResolvedPackageResponse>>
{
    public async Task<Result<ResolvedPackageResponse>> Handle(UpdatePackageOverrideCommand request, CancellationToken ct)
    {
        var tenantPackage = await uow.TenantPackages.GetByIdAsync(request.TenantPackageId, ct);
        if (tenantPackage is null)
            return Result<ResolvedPackageResponse>.Failure("Tenant package not found.");

        if (tenantPackage.TenantId != request.TenantId)
            return Result<ResolvedPackageResponse>.Failure("Unauthorized.");

        if (tenantPackage.IsLocked)
            return Result<ResolvedPackageResponse>.Failure("Package is locked and cannot be customized.");

        var @override = tenantPackage.Override;
        if (@override is null)
        {
            @override = LitXusTravel.Domain.Entities.PackageOverride.CreateEmpty(request.TenantId, request.TenantPackageId);
            await uow.PackageOverrides.AddAsync(@override, ct);
        }

        @override.Update(
            title: request.Title,
            price: request.Price,
            currency: request.Currency,
            featuredImageUrl: request.FeaturedImageUrl,
            imagesJson: request.ImagesJson,
            description: request.Description,
            shortDescription: request.ShortDescription,
            contactPhone: request.ContactPhone,
            contactWhatsapp: request.ContactWhatsapp,
            destination: request.Destination,
            durationDays: request.DurationDays,
            category: request.Category,
            region: request.Region
        );

        if (@override.HasAnyOverride())
            tenantPackage.MarkCustomized();

        await uow.SaveChangesAsync(ct);
        tenantPackage = await uow.TenantPackages.GetByIdAsync(request.TenantPackageId, ct);
        
        return Result<ResolvedPackageResponse>.Success(MergePackage(tenantPackage!));
    }

    private ResolvedPackageResponse MergePackage(LitXusTravel.Domain.Entities.TenantPackage tp)
    {
        var @override = tp.Override!;

        if (tp.IsOwnedPackage)
        {
            return new ResolvedPackageResponse(
                Id: tp.Id,
                MasterPackageId: null,
                IsOwnedPackage: true,
                Title: @override.Title ?? string.Empty,
                Description: @override.Description,
                ShortDescription: @override.ShortDescription,
                Category: @override.Category,
                Price: @override.Price ?? 0,
                Currency: @override.Currency ?? "MYR",
                DurationDays: @override.DurationDays ?? 0,
                Destination: @override.Destination ?? string.Empty,
                Region: @override.Region,
                FeaturedImageUrl: @override.FeaturedImageUrl,
                ImagesJson: @override.ImagesJson,
                ItineraryJson: null,
                HighlightsJson: null,
                InclusionsJson: null,
                ExclusionsJson: null,
                ContactPhone: @override.ContactPhone,
                ContactWhatsapp: @override.ContactWhatsapp,
                IsCustomized: true,
                LastSyncedAt: tp.LastSyncedAt ?? DateTimeOffset.UtcNow,
                SyncSource: null
            );
        }

        var master = tp.MasterPackage!;
        var createdByCurrentTenant = master.CreatedByTenantId == tp.TenantId;

        return new ResolvedPackageResponse(
            Id: tp.Id,
            MasterPackageId: master.Id,
            IsOwnedPackage: createdByCurrentTenant,
            Title: @override?.Title ?? master.Title,
            Description: @override?.Description ?? master.Description,
            ShortDescription: @override?.ShortDescription ?? master.ShortDescription,
            Category: master.Category,
            Price: @override?.Price ?? master.BasePrice,
            Currency: @override?.Currency ?? master.Currency,
            DurationDays: master.DurationDays,
            Destination: master.Destination,
            Region: master.Region,
            FeaturedImageUrl: @override?.FeaturedImageUrl ?? master.FeaturedImageUrl,
            ImagesJson: @override?.ImagesJson ?? master.ImagesJson,
            ItineraryJson: master.ItineraryJson,
            HighlightsJson: master.HighlightsJson,
            InclusionsJson: master.InclusionsJson,
            ExclusionsJson: master.ExclusionsJson,
            ContactPhone: @override?.ContactPhone,
            ContactWhatsapp: @override?.ContactWhatsapp,
            IsCustomized: tp.IsCustomized,
            LastSyncedAt: tp.LastSyncedAt ?? DateTimeOffset.UtcNow,
            SyncSource: null
        );
    }
}
