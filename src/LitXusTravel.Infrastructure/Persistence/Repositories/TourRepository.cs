using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class TourRepository(LitXusTravelDbContext context) : Repository<Tour>(context), ITourRepository
{
    public async Task<IEnumerable<Tour>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Tours.Where(t => t.TenantId == tenantId).ToListAsync(ct);

    public async Task<IEnumerable<Tour>> GetScheduledByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Tours.Where(t => t.TenantId == tenantId && t.Status == TourStatus.Scheduled).ToListAsync(ct);

    public async Task<IEnumerable<Tour>> GetCompletedByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Tours.Where(t => t.TenantId == tenantId && t.Status == TourStatus.Completed).ToListAsync(ct);
}
