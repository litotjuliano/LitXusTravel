using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class TenantPackageRepository(LitXusTravelDbContext context)
    : Repository<TenantPackage>(context), ITenantPackageRepository
{
    public async Task<IEnumerable<TenantPackage>> GetByTenantWithDetailsAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.TenantPackages
            .Include(tp => tp.MasterPackage)
            .Include(tp => tp.Override)
            .Where(tp => tp.TenantId == tenantId && tp.IsActive)
            .ToListAsync(ct);

    public async Task<IEnumerable<TenantPackage>> GetAllWithTenantsAsync(CancellationToken ct = default)
        => await _context.TenantPackages
            .Include(tp => tp.Tenant)
            .Where(tp => tp.IsActive)
            .ToListAsync(ct);
}
