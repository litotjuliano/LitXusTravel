using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Packages.UpdatePackageOverride;

public record UpdatePackageOverrideCommand(
    Guid TenantId,
    Guid TenantPackageId,
    string? Title = null,
    decimal? Price = null,
    string? Currency = null,
    string? FeaturedImageUrl = null,
    string? ImagesJson = null,
    string? Description = null,
    string? ShortDescription = null,
    string? ContactPhone = null,
    string? ContactWhatsapp = null,
    // Owned-package fields (destination/duration/category live in override, not master)
    string? Destination = null,
    int? DurationDays = null,
    string? Category = null,
    string? Region = null
) : IRequest<Result<ResolvedPackageResponse>>;
