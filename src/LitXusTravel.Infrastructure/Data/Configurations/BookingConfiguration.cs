using LitXusTravel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.CustomerEmail).IsRequired().HasMaxLength(255);
        builder.Property(b => b.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(b => b.Status).HasConversion<string>().IsRequired();
        builder.Property(b => b.ReferralCode).HasMaxLength(100);
        builder.Property(b => b.CancellationReason).HasMaxLength(500);

        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => b.TourId);
        builder.HasIndex(b => b.AgentId);
        builder.HasIndex(b => b.Status);
        // Safeguard 6: composite index for duplicate check
        builder.HasIndex(b => new { b.CustomerEmail, b.TourId, b.TourDate });
    }
}
