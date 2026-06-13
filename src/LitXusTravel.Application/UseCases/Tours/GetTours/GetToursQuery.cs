using LitXusTravel.Application.Common.Models;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tours.GetTours;

public record GetToursQuery(Guid TenantId, TourStatus? Status = null) : IRequest<Result<IEnumerable<TourDto>>>;

public record TourDto(
    Guid Id,
    Guid TenantId,
    Guid? TenantPackageId,
    string Title,
    string Destination,
    DateTime StartDate,
    DateTime EndDate,
    int MaxCapacity,
    int CurrentBookings,
    int AvailableSpots,
    decimal BasePrice,
    string Currency,
    string Status);
