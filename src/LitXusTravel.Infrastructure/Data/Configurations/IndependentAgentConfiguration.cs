using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class IndependentAgentConfiguration : IEntityTypeConfiguration<IndependentAgent>
{
    public void Configure(EntityTypeBuilder<IndependentAgent> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).IsRequired().HasMaxLength(255);
        builder.Property(i => i.SubscriptionTier).IsRequired().HasMaxLength(100);
        builder.Property(i => i.WhiteLabelDomain).IsRequired().HasMaxLength(255);
        builder.Property(i => i.IsActive).IsRequired();

        builder.OwnsOne(i => i.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.Property(i => i.AuthorizedTenantIds)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Guid>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))))
            .HasMaxLength(1000);

        builder.HasIndex(i => i.IsActive);
    }
}
