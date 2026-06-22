using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.UpdatePackage;

public record UpdatePackageCommand(
    Guid Id,
    string Title,
    string Destination,
    decimal BasePrice,
    int DurationDays,
    string? Category,
    string? Description,
    string? ShortDescription,
    string? Region,
    string? FeaturedImageUrl,
    string? ContactPhone,
    string? ContactWhatsapp,
    string? Visibility
) : IRequest<Result<object>>;
