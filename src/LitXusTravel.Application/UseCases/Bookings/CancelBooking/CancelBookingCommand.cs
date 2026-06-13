using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Bookings.CancelBooking;

public record CancelBookingCommand(Guid TenantId, Guid BookingId, string Reason = "") : IRequest<Result<bool>>;
