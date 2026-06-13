using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tours.CompleteTour;

/// <summary>
/// Marks a tour as completed and finalizes all accrued commissions for confirmed bookings.
/// Safeguard 1: Commission only finalizes when tour is marked Complete.
/// </summary>
public class CompleteTourCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    : IRequestHandler<CompleteTourCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CompleteTourCommand request, CancellationToken ct)
    {
        var tour = await unitOfWork.Tours.GetByIdAsync(request.TourId, ct);
        if (tour is null) return Result<bool>.Failure("Tour not found.");
        if (tour.TenantId != request.TenantId) return Result<bool>.Failure("Tour does not belong to this tenant.");

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // 1. Complete all confirmed bookings for this tour
                var bookings = await unitOfWork.Bookings.GetByTourAsync(tour.Id, ct);
                foreach (var booking in bookings.Where(b => b.Status == BookingStatus.Confirmed))
                    booking.Complete();

                // 2. Complete the tour
                tour.Complete();
                unitOfWork.Tours.Update(tour);

                // 3. Finalize all accrued commissions for this tour's bookings
                var confirmedBookingIds = bookings
                    .Where(b => b.Status == BookingStatus.Completed)
                    .Select(b => b.Id)
                    .ToHashSet();

                var allAccruals = await unitOfWork.CommissionAccruals.GetByTenantAsync(request.TenantId, ct);
                var toFinalize = allAccruals
                    .Where(a => confirmedBookingIds.Contains(a.SourceId) && a.Status == CommissionStatus.Accrued)
                    .ToList();

                foreach (var accrual in toFinalize)
                {
                    accrual.MarkAsFinalized();
                    unitOfWork.CommissionAccruals.Update(accrual);
                }

                await unitOfWork.SaveChangesAsync(ct);
            }, ct);

            await auditService.LogAsync(
                action: AuditAction.StatusChanged,
                entityType: nameof(Tour),
                entityId: tour.Id,
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
