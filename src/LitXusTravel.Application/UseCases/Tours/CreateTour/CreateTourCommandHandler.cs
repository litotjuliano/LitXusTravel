using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tours.CreateTour;

public class CreateTourCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    : IRequestHandler<CreateTourCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTourCommand request, CancellationToken ct)
    {
        try
        {
            var tour = Tour.Create(
                request.TenantId,
                request.Title,
                request.Destination,
                request.StartDate,
                request.EndDate,
                request.MaxCapacity,
                request.BasePrice,
                request.Currency,
                request.TenantPackageId);

            await unitOfWork.Tours.AddAsync(tour, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await auditService.LogAsync(
                action: AuditAction.Created,
                entityType: nameof(Tour),
                entityId: tour.Id,
                tenantId: request.TenantId,
                ct: ct);

            return Result<Guid>.Success(tour.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
