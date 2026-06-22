using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class PackageOverrideConfiguration : IEntityTypeConfiguration<PackageOverride>
{
    public void Configure(EntityTypeBuilder<PackageOverride> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title).HasMaxLength(255);
        builder.Property(o => o.Currency).HasMaxLength(10);
        builder.Property(o => o.ContactPhone).HasMaxLength(20);
        builder.Property(o => o.ContactWhatsapp).HasMaxLength(20);
        builder.Property(o => o.Price).HasColumnType("decimal(10,2)");

        builder.HasIndex(o => o.TenantId);
        builder.HasIndex(o => o.TenantPackageId);
    }
}
