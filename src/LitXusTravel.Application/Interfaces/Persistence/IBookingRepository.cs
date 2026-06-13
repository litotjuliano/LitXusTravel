using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Interfaces.Persistence;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<Booking>> GetByTourAsync(Guid tourId, CancellationToken ct = default);
    Task<IEnumerable<Booking>> GetByAgentAsync(Guid agentId, CancellationToken ct = default);
    /// <summary>Safeguard 6: Check for duplicate booking (same customer + tour + date).</summary>
    Task<bool> IsDuplicateBookingAsync(string customerEmail, Guid tourId, DateTime tourDate, CancellationToken ct = default);
}
