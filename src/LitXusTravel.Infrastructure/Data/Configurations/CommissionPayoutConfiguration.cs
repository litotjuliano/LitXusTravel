using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class CommissionPayoutConfiguration : IEntityTypeConfiguration<CommissionPayout>
{
    public void Configure(EntityTypeBuilder<CommissionPayout> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.AgentId);
        builder.Property(c => c.TenantId).IsRequired();
        builder.Property(c => c.PayoutPeriodStart).IsRequired();
        builder.Property(c => c.PayoutPeriodEnd).IsRequired();
        builder.Property(c => c.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().IsRequired();
        builder.Property(c => c.ProcessedAt);
        builder.Property(c => c.TransactionId).HasMaxLength(255);

        builder.Property(c => c.CommissionAccrualIds)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Guid>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))))
            .HasMaxLength(5000);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.Status);
    }
}
