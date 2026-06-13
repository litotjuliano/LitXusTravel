using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class BookingRepository(LitXusTravelDbContext context) : Repository<Booking>(context), IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Bookings.Where(b => b.TenantId == tenantId).ToListAsync(ct);

    public async Task<IEnumerable<Booking>> GetByTourAsync(Guid tourId, CancellationToken ct = default)
        => await _context.Bookings.Where(b => b.TourId == tourId).ToListAsync(ct);

    public async Task<IEnumerable<Booking>> GetByAgentAsync(Guid agentId, CancellationToken ct = default)
        => await _context.Bookings.Where(b => b.AgentId == agentId).ToListAsync(ct);

    public async Task<bool> IsDuplicateBookingAsync(string customerEmail, Guid tourId, DateTime tourDate, CancellationToken ct = default)
        => await _context.Bookings.AnyAsync(b =>
            b.CustomerEmail == customerEmail &&
            b.TourId == tourId &&
            b.TourDate.Date == tourDate.Date &&
            b.Status != BookingStatus.Cancelled, ct);
}
