using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tours.CompleteTour;

public record CompleteTourCommand(Guid TenantId, Guid TourId) : IRequest<Result<bool>>;
