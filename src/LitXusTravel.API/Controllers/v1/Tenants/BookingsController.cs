using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Bookings.CreateBooking;
using LitXusTravel.Application.UseCases.Bookings.CancelBooking;
using LitXusTravel.Application.UseCases.Bookings.GetBookings;

using LitXusTravel.API.Filters;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/bookings")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
[SubscriptionWriteGuard]
[Tags("Bookings")]
public class BookingsController(IMediator mediator) : ControllerBase
{
    /// <summary>List bookings for a tenant. Filter by tourId or agentId.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookings(
        Guid tenantId,
        [FromQuery] Guid? tourId = null,
        [FromQuery] Guid? agentId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetBookingsQuery(tenantId, tourId, agentId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Create a booking. Automatically accrues commission if a valid referral code is provided.
    /// Enforces Safeguard 2 (no self-booking) and Safeguard 6 (no duplicate booking).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBooking(Guid tenantId, CreateBookingRequest request, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CreateBookingCommand(
            tenantId,
            request.TourId,
            request.CustomerName,
            request.CustomerEmail,
            request.TourDate,
            request.ReferralCode,
            request.RequestingAgentId), ct);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetBookings), new { tenantId }, new { id = result.Value });
    }

    /// <summary>
    /// Cancel a booking. Automatically reverses accrued commissions.
    /// Safeguard 1: Cancelled bookings reverse commissions.
    /// Safeguard 8: Post-payout refunds create negative reversal entries for next payout.
    /// </summary>
    [HttpPost("{bookingId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBooking(Guid tenantId, Guid bookingId, CancelBookingRequest request, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CancelBookingCommand(tenantId, bookingId, request.Reason ?? ""), ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
                return NotFound(new { errors = result.Errors });
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Booking cancelled. Commissions reversed." });
    }
}

public record CreateBookingRequest(
    Guid TourId,
    string CustomerName,
    string CustomerEmail,
    DateTime TourDate,
    string? ReferralCode = null,
    Guid? RequestingAgentId = null);

public record CancelBookingRequest(string? Reason = null);
