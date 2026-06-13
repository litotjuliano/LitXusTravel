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

        // Navigation property is not loaded by GetByIdAsync — query directly to avoid duplicate insert
        var @override = await uow.PackageOverrides
            .FirstOrDefaultAsync(o => o.TenantPackageId == request.TenantPackageId, ct);
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

        // Load master explicitly for synced packages — avoids relying on nav property fixup
        LitXusTravel.Domain.Entities.Package? master = null;
        if (!tenantPackage.IsOwnedPackage && tenantPackage.MasterPackageId.HasValue)
            master = await uow.Packages.GetByIdAsync(tenantPackage.MasterPackageId.Value, ct);

        return Result<ResolvedPackageResponse>.Success(MergePackage(tenantPackage, @override, master));
    }

    private static ResolvedPackageResponse MergePackage(
        LitXusTravel.Domain.Entities.TenantPackage tp,
        LitXusTravel.Domain.Entities.PackageOverride? ov,
        LitXusTravel.Domain.Entities.Package? master = null)
    {
        if (tp.IsOwnedPackage)
        {
            return new ResolvedPackageResponse(
                Id: tp.Id,
                MasterPackageId: null,
                IsOwnedPackage: true,
                Visibility: "Published",
                Title: ov?.Title ?? string.Empty,
                Description: ov?.Description,
                ShortDescription: ov?.ShortDescription,
                Category: ov?.Category,
                Price: ov?.Price ?? 0,
                Currency: ov?.Currency ?? "MYR",
                DurationDays: ov?.DurationDays ?? 0,
                Destination: ov?.Destination ?? string.Empty,
                Region: ov?.Region,
                FeaturedImageUrl: ov?.FeaturedImageUrl,
                ImagesJson: ov?.ImagesJson,
                ItineraryJson: null,
                HighlightsJson: null,
                InclusionsJson: null,
                ExclusionsJson: null,
                ContactPhone: ov?.ContactPhone,
                ContactWhatsapp: ov?.ContactWhatsapp,
                IsCustomized: true,
                LastSyncedAt: tp.LastSyncedAt ?? DateTimeOffset.UtcNow,
                SyncSource: null
            );
        }

        if (master is null)
            throw new InvalidOperationException($"Master package not found for TenantPackage {tp.Id}.");

        var createdByCurrentTenant = master.CreatedByTenantId == tp.TenantId;

        return new ResolvedPackageResponse(
            Id: tp.Id,
            MasterPackageId: master.Id,
            IsOwnedPackage: createdByCurrentTenant,
            Visibility: master.Visibility.ToString(),
            Title: ov?.Title ?? master.Title,
            Description: ov?.Description ?? master.Description,
            ShortDescription: ov?.ShortDescription ?? master.ShortDescription,
            Category: master.Category,
            Price: ov?.Price ?? master.BasePrice,
            Currency: ov?.Currency ?? master.Currency,
            DurationDays: master.DurationDays,
            Destination: master.Destination,
            Region: master.Region,
            FeaturedImageUrl: ov?.FeaturedImageUrl ?? master.FeaturedImageUrl,
            ImagesJson: ov?.ImagesJson ?? master.ImagesJson,
            ItineraryJson: master.ItineraryJson,
            HighlightsJson: master.HighlightsJson,
            InclusionsJson: master.InclusionsJson,
            ExclusionsJson: master.ExclusionsJson,
            ContactPhone: ov?.ContactPhone,
            ContactWhatsapp: ov?.ContactWhatsapp,
            IsCustomized: tp.IsCustomized,
            LastSyncedAt: tp.LastSyncedAt ?? DateTimeOffset.UtcNow,
            SyncSource: null
        );
    }
}
