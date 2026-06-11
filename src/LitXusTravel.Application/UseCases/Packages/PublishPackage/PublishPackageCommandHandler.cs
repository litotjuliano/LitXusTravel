using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.UseCases.Packages.PublishPackage;

public class PublishPackageCommandHandler(IUnitOfWork uow, IAuditService audit, INotificationService notifications)
    : IRequestHandler<PublishPackageCommand, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(PublishPackageCommand request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<PackageResponse>.Failure("Package not found.");

        try
        {
            package.Publish();
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<PackageResponse>.Failure(ex.Message);
        }

        await uow.SaveChangesAsync(ct);

        await audit.LogAsync(AuditAction.Published, nameof(Package), package.Id,
            newValues: new { package.Visibility }, ct: ct);

        await notifications.SendPackagePublishedAsync(package.Id, package.Title, ct);

        return Result<PackageResponse>.Success(new PackageResponse(
            package.Id, package.Title, package.Description, package.ShortDescription,
            package.Category, package.BasePrice, package.Currency, package.DurationDays,
            package.Destination, package.Region, package.FeaturedImageUrl, package.ImagesJson,
            package.ItineraryJson, package.HighlightsJson, package.InclusionsJson,
            package.ExclusionsJson, package.Rating, package.ReviewCount,
            package.Visibility.ToString(), package.IsPopular, package.IsFeatured,
            package.CreatedAt, package.UpdatedAt));
    }
}
