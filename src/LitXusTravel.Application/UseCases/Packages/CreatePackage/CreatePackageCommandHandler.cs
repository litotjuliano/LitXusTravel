using AutoMapper;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.CreatePackage;

public class CreatePackageCommandHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<CreatePackageCommand, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(CreatePackageCommand request, CancellationToken ct)
    {
        var package = Package.Create(
            title: request.Title,
            destination: request.Destination,
            basePrice: request.BasePrice,
            durationDays: request.DurationDays,
            category: request.Category,
            description: request.Description,
            currency: request.Currency);

        // Apply all optional fields in one pass
        package.UpdateDetails(
            title: request.Title,
            description: request.Description,
            shortDescription: request.ShortDescription,
            category: request.Category,
            basePrice: request.BasePrice,
            durationDays: request.DurationDays,
            destination: request.Destination,
            region: request.Region,
            itineraryJson: request.ItineraryJson,
            highlightsJson: request.HighlightsJson,
            inclusionsJson: request.InclusionsJson,
            exclusionsJson: request.ExclusionsJson,
            imagesJson: request.ImagesJson,
            featuredImageUrl: request.FeaturedImageUrl,
            maxGroupSize: request.MaxGroupSize);

        await uow.Packages.AddAsync(package, ct);
        await uow.SaveChangesAsync(ct);

        return Result<PackageResponse>.Success(mapper.Map<PackageResponse>(package));
    }
}
