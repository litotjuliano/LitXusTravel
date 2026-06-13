using LitXusTravel.Application.Common.Models;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.Bookings.GetBookings;

public record GetBookingsQuery(Guid TenantId, Guid? TourId = null, Guid? AgentId = null)
    : IRequest<Result<IEnumerable<BookingDto>>>;

public record BookingDto(
    Guid Id,
    Guid TenantId,
    Guid TourId,
    string CustomerName,
    string CustomerEmail,
    DateTime BookingDate,
    DateTime TourDate,
    decimal TotalAmount,
    string Status,
    string? ReferralCode,
    Guid? AgentId);
