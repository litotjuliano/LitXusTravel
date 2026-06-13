using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Bookings.CreateBooking;

public record CreateBookingCommand(
    Guid TenantId,
    Guid TourId,
    string CustomerName,
    string CustomerEmail,
    DateTime TourDate,
    string? ReferralCode = null,
    Guid? RequestingAgentId = null)
    : IRequest<Result<Guid>>;
