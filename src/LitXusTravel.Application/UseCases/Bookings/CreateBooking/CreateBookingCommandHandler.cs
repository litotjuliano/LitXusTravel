using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.Bookings.CreateBooking;

/// <summary>
/// Creates a booking and auto-accrues commission if a valid referral code is provided.
/// Enforces Safeguard 2 (self-booking prevention) and Safeguard 6 (duplicate booking prevention).
/// </summary>
public class CreateBookingCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        // Validate tour exists and has capacity
        var tour = await unitOfWork.Tours.GetByIdAsync(request.TourId, ct);
        if (tour is null) return Result<Guid>.Failure("Tour not found.");
        if (tour.TenantId != request.TenantId) return Result<Guid>.Failure("Tour does not belong to this tenant.");
        if (!tour.HasCapacity()) return Result<Guid>.Failure("Tour has no available capacity.");

        // Safeguard 6: Duplicate booking prevention
        var isDuplicate = await unitOfWork.Bookings.IsDuplicateBookingAsync(
            request.CustomerEmail, request.TourId, request.TourDate, ct);
        if (isDuplicate)
            return Result<Guid>.Failure($"Customer {request.CustomerEmail} already has a booking for this tour on this date.");

        // Resolve referral code to agent
        StaffAgent? referringAgent = null;
        if (!string.IsNullOrWhiteSpace(request.ReferralCode))
        {
            referringAgent = await unitOfWork.StaffAgents.GetByCodeAsync(request.ReferralCode, request.TenantId, ct);

            if (referringAgent is not null)
            {
                // Safeguard 2: Self-booking prevention
                if (request.RequestingAgentId.HasValue && request.RequestingAgentId.Value == referringAgent.Id)
                    return Result<Guid>.Failure("Cannot use own referral code for booking.");

                if (!referringAgent.IsCodeValid(request.ReferralCode))
                    referringAgent = null; // Expired code — treat as no referral
            }
        }

        try
        {
            Booking booking = null!;

            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Create the booking
                booking = Booking.Create(
                    request.TenantId,
                    request.TourId,
                    request.CustomerName,
                    request.CustomerEmail,
                    request.TourDate,
                    tour.BasePrice,
                    request.ReferralCode,
                    referringAgent?.Id);

                booking.Confirm();
                await unitOfWork.Bookings.AddAsync(booking, ct);

                // Update tour capacity
                tour.AddBooking();
                unitOfWork.Tours.Update(tour);

                // Auto-accrue commission if referral code resolved to an active agent
                if (referringAgent is not null)
                {
                    var rule = await unitOfWork.CommissionRules.GetApplicableRuleAsync(
                        request.TenantId, referringAgent.Id, CommissionTrigger.TourBooked, ct);

                    if (rule is not null && rule.IsApplicable())
                    {
                        var commissionAmount = rule.IsPercentage
                            ? tour.BasePrice * rule.Amount / 100
                            : rule.Amount;

                        var accrual = CommissionAccrual.CreateFromBooking(
                            referringAgent.Id,
                            request.TenantId,
                            rule.Id,
                            booking.Id,
                            CommissionTriggerType.TourBooked,
                            commissionAmount,
                            rule.IsPercentage ? rule.Amount : null,
                            tour.BasePrice);

                        await unitOfWork.CommissionAccruals.AddAsync(accrual, ct);
                    }
                }

                await unitOfWork.SaveChangesAsync(ct);
            }, ct);

            await auditService.LogAsync(
                action: AuditAction.Created,
                entityType: nameof(Booking),
                entityId: booking.Id,
                tenantId: request.TenantId,
                ct: ct);

            return Result<Guid>.Success(booking.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
