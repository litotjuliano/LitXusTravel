using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.CreateTenantPackage;

public record CreateTenantPackageCommand(
    Guid TenantId,
    string Title,
    string Destination,
    int DurationDays,
    decimal Price,
    string Currency,
    string? Category,
    string? Region,
    string? Description,
    string? ShortDescription,
    string? FeaturedImageUrl,
    string? ContactPhone,
    string? ContactWhatsapp,
    bool ExtendToMaster
) : IRequest<Result<Guid>>;
