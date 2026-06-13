using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LitXusTravel.Infrastructure.Persistence.Repositories;

public class DisputeResolutionRepository : Repository<DisputeResolutionTicket>, IDisputeResolutionRepository
{
    public DisputeResolutionRepository(LitXusTravelDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DisputeResolutionTicket>> GetOpenAsync(CancellationToken ct = default)
    {
        return await _context.DisputeResolutionTickets
            .Where(d => d.Status == DisputeStatus.Open || d.Status == DisputeStatus.PendingReview)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DisputeResolutionTicket>> GetByStatusAsync(DisputeStatus status, CancellationToken ct = default)
    {
        return await _context.DisputeResolutionTickets
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DisputeResolutionTicket>> GetByCommissionAccrualAsync(Guid commissionAccrualId, CancellationToken ct = default)
    {
        return await _context.DisputeResolutionTickets
            .Where(d => d.CommissionAccrualId == commissionAccrualId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DisputeResolutionTicket>> GetByCreatorAsync(Guid superAdminId, CancellationToken ct = default)
    {
        return await _context.DisputeResolutionTickets
            .Where(d => d.SuperAdminId == superAdminId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DisputeResolutionTicket>> GetPendingReviewAsync(CancellationToken ct = default)
    {
        // Safeguard 10: Dispute resolution workflow - pending admin review
        return await _context.DisputeResolutionTickets
            .Where(d => d.Status == DisputeStatus.Open || d.Status == DisputeStatus.PendingReview)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DisputeResolutionTicket>> GetResolvedAsync(int daysBack = 30, CancellationToken ct = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysBack);
        return await _context.DisputeResolutionTickets
            .Where(d => d.Status == DisputeStatus.Resolved && d.ResolvedAt >= cutoffDate)
            .OrderByDescending(d => d.ResolvedAt)
            .ToListAsync(ct);
    }
}
