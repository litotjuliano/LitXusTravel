using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tours.CreateTour;

public record CreateTourCommand(
    Guid TenantId,
    string Title,
    string Destination,
    DateTime StartDate,
    DateTime EndDate,
    int MaxCapacity,
    decimal BasePrice,
    string Currency = "MYR",
    Guid? TenantPackageId = null)
    : IRequest<Result<Guid>>;
