using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Bookings.GetBookings;

public class GetBookingsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetBookingsQuery, Result<IEnumerable<BookingDto>>>
{
    public async Task<Result<IEnumerable<BookingDto>>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        IEnumerable<Domain.Entities.Booking> bookings;

        if (request.TourId.HasValue)
            bookings = await unitOfWork.Bookings.GetByTourAsync(request.TourId.Value, ct);
        else if (request.AgentId.HasValue)
            bookings = await unitOfWork.Bookings.GetByAgentAsync(request.AgentId.Value, ct);
        else
            bookings = await unitOfWork.Bookings.GetByTenantAsync(request.TenantId, ct);

        var dtos = bookings.Select(b => new BookingDto(
            b.Id,
            b.TenantId,
            b.TourId,
            b.CustomerName,
            b.CustomerEmail,
            b.BookingDate,
            b.TourDate,
            b.TotalAmount,
            b.Status.ToString(),
            b.ReferralCode,
            b.AgentId));

        return Result<IEnumerable<BookingDto>>.Success(dtos);
    }
}
