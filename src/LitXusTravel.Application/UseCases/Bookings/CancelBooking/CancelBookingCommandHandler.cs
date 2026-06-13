using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.Bookings.CancelBooking;

/// <summary>
/// Cancels a booking and auto-reverses any accrued commissions.
/// Safeguard 1: Cancelled bookings reverse commissions. Safeguard 8: Post-payout refunds tracked.
/// </summary>
public class CancelBookingCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    : IRequestHandler<CancelBookingCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(request.BookingId, ct);
        if (booking is null) return Result<bool>.Failure("Booking not found.");
        if (booking.TenantId != request.TenantId) return Result<bool>.Failure("Booking does not belong to this tenant.");

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Cancel the booking
                booking.Cancel(request.Reason);
                unitOfWork.Bookings.Update(booking);

                // Update tour capacity
                var tour = await unitOfWork.Tours.GetByIdAsync(booking.TourId, ct);
                if (tour is not null)
                {
                    tour.RemoveBooking();
                    unitOfWork.Tours.Update(tour);
                }

                // Auto-reverse accrued commissions (Safeguard 1)
                var accruals = await unitOfWork.CommissionAccruals.GetBySourceAsync(booking.Id, ct);
                foreach (var accrual in accruals)
                {
                    if (accrual.Status == CommissionStatus.Paid)
                    {
                        // Safeguard 8: Create refund reversal for next payout deduction
                        var reversal = accrual.CreateRefundReversal();
                        await unitOfWork.CommissionAccruals.AddAsync(reversal, ct);
                    }
                    else if (accrual.Status != CommissionStatus.Reversed)
                    {
                        accrual.Reverse($"Booking cancelled: {request.Reason}");
                        unitOfWork.CommissionAccruals.Update(accrual);
                    }
                }

                await unitOfWork.SaveChangesAsync(ct);
            }, ct);

            await auditService.LogAsync(
                action: AuditAction.StatusChanged,
                entityType: nameof(Booking),
                entityId: booking.Id,
                tenantId: request.TenantId,
                ct: ct);

            return Result<bool>.Success(true);
        }
        catch (DomainException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
