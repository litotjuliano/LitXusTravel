using LitXusTravel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tours");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Destination).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Currency).IsRequired().HasMaxLength(10);
        builder.Property(t => t.BasePrice).HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.Status).HasConversion<string>().IsRequired();

        builder.HasIndex(t => t.TenantId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => new { t.TenantId, t.Status });
    }
}
