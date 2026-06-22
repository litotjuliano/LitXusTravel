using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Tours.CreateTour;
using LitXusTravel.Application.UseCases.Tours.CompleteTour;
using LitXusTravel.Application.UseCases.Tours.GetTours;
using LitXusTravel.Domain.Entities;

using LitXusTravel.API.Filters;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/tours")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
[Tags("Tours")]
public class ToursController(IMediator mediator) : ControllerBase
{
    /// <summary>List tours for a tenant. Optionally filter by status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TourDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTours(Guid tenantId, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        TourStatus? parsedStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TourStatus>(status, true, out var s))
            parsedStatus = s;

        var result = await mediator.Send(new GetToursQuery(tenantId, parsedStatus), ct);
        return Ok(result.Value);
    }

    /// <summary>Create a new tour for a tenant.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTour(Guid tenantId, CreateTourRequest request, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CreateTourCommand(
            tenantId,
            request.Title,
            request.Destination,
            request.StartDate,
            request.EndDate,
            request.MaxCapacity,
            request.BasePrice,
            request.Currency ?? "MYR",
            request.TenantPackageId), ct);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetTours), new { tenantId }, new { id = result.Value });
    }

    /// <summary>
    /// Mark a tour as Completed. Automatically finalizes all accrued commissions for its bookings.
    /// Safeguard 1: Commission only finalizes on tour completion.
    /// </summary>
    [HttpPost("{tourId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTour(Guid tenantId, Guid tourId, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CompleteTourCommand(tenantId, tourId), ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
                return NotFound(new { errors = result.Errors });
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Tour completed. Commissions finalized." });
    }
}

public record CreateTourRequest(
    string Title,
    string Destination,
    DateTime StartDate,
    DateTime EndDate,
    int MaxCapacity,
    decimal BasePrice,
    string? Currency = null,
    Guid? TenantPackageId = null);
