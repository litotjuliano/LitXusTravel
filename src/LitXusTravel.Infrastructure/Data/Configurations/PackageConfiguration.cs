using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Destination).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Region).HasMaxLength(100);
        builder.Property(p => p.Category).HasMaxLength(100);
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(10).HasDefaultValue("MYR");
        builder.Property(p => p.ShortDescription).HasMaxLength(500);
        builder.Property(p => p.FeaturedImageUrl).HasMaxLength(500);
        builder.Property(p => p.VideoUrl).HasMaxLength(500);
        builder.Property(p => p.BasePrice).HasColumnType("decimal(10,2)");
        builder.Property(p => p.Rating).HasColumnType("decimal(3,2)");
        builder.Property(p => p.Visibility).HasConversion<string>().HasMaxLength(50);

        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Destination);
        builder.HasIndex(p => p.Visibility);
        builder.HasIndex(p => p.IsFeatured);
    }
}
