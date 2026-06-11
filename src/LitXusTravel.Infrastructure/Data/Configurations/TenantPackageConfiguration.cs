using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class TenantPackageConfiguration : IEntityTypeConfiguration<TenantPackage>
{
    public void Configure(EntityTypeBuilder<TenantPackage> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.SyncStatus).HasConversion<string>().HasMaxLength(50);

        builder.HasIndex(tp => tp.TenantId);
        builder.HasIndex(tp => tp.MasterPackageId);
        builder.HasIndex(tp => new { tp.TenantId, tp.MasterPackageId }).IsUnique();

        builder.HasOne(tp => tp.MasterPackage)
            .WithMany(p => p.TenantPackages)
            .HasForeignKey(tp => tp.MasterPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tp => tp.Tenant)
            .WithMany(t => t.TenantPackages)
            .HasForeignKey(tp => tp.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tp => tp.Override)
            .WithOne(o => o.TenantPackage)
            .HasForeignKey<PackageOverride>(o => o.TenantPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
