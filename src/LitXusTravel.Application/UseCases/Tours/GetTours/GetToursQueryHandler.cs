using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tours.GetTours;

public class GetToursQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetToursQuery, Result<IEnumerable<TourDto>>>
{
    public async Task<Result<IEnumerable<TourDto>>> Handle(GetToursQuery request, CancellationToken ct)
    {
        var tours = await unitOfWork.Tours.GetByTenantAsync(request.TenantId, ct);

        if (request.Status.HasValue)
            tours = tours.Where(t => t.Status == request.Status.Value);

        var dtos = tours.Select(t => new TourDto(
            t.Id,
            t.TenantId,
            t.TenantPackageId,
            t.Title,
            t.Destination,
            t.StartDate,
            t.EndDate,
            t.MaxCapacity,
            t.CurrentBookings,
            t.MaxCapacity - t.CurrentBookings,
            t.BasePrice,
            t.Currency,
            t.Status.ToString()));

        return Result<IEnumerable<TourDto>>.Success(dtos);
    }
}
