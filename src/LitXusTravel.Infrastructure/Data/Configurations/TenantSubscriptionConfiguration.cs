using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.PlanName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.MonthlyPrice).HasColumnType("decimal(10,2)");

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.EndDate);
    }
}
