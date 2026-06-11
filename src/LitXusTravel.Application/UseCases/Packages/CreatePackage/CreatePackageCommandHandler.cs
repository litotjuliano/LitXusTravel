using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Packages.CreatePackage;

public class CreatePackageCommandHandler(IUnitOfWork uow, IAuditService audit)
    : IRequestHandler<CreatePackageCommand, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(CreatePackageCommand request, CancellationToken ct)
    {
        var package = Package.Create(
            request.Title, request.Destination, request.BasePrice,
            request.DurationDays, request.Category, request.Description, request.Currency);

        package.UpdateDetails(
            request.Title, request.Description, request.ShortDescription,
            request.Category, request.BasePrice, request.DurationDays,
            request.Destination, request.Region, request.ItineraryJson,
            request.HighlightsJson, request.InclusionsJson, request.ExclusionsJson,
            request.ImagesJson, request.FeaturedImageUrl, request.MaxGroupSize);

        await uow.Packages.AddAsync(package, ct);
        await uow.SaveChangesAsync(ct);

        await audit.LogAsync(AuditAction.Created, nameof(Package), package.Id,
            newValues: new { package.Title, package.Destination, package.BasePrice }, ct: ct);

        return Result<PackageResponse>.Success(MapToResponse(package));
    }

    private static PackageResponse MapToResponse(Domain.Entities.Package p) => new(
        p.Id, p.Title, p.Description, p.ShortDescription, p.Category,
        p.BasePrice, p.Currency, p.DurationDays, p.Destination, p.Region,
        p.FeaturedImageUrl, p.ImagesJson, p.ItineraryJson, p.HighlightsJson,
        p.InclusionsJson, p.ExclusionsJson, p.Rating, p.ReviewCount,
        p.Visibility.ToString(), p.IsPopular, p.IsFeatured, p.CreatedAt, p.UpdatedAt);
}
