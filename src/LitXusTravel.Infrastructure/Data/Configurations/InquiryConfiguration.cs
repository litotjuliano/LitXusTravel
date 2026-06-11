using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
{
    public void Configure(EntityTypeBuilder<Inquiry> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.CustomerName).IsRequired().HasMaxLength(255);
        builder.Property(i => i.CustomerEmail).IsRequired().HasMaxLength(255);
        builder.Property(i => i.CustomerPhone).IsRequired().HasMaxLength(20);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.CreatedAt);

        // Restrict to avoid multiple cascade paths:
        //   Tenant → TenantPackages (CASCADE) → Inquiries (SET NULL)
        //   Tenant → Inquiries (would be a second cascade path)
        builder.HasOne(i => i.TenantPackage)
            .WithMany(tp => tp.Inquiries)
            .HasForeignKey(i => i.TenantPackageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.Activities)
            .WithOne(a => a.Inquiry)
            .HasForeignKey(a => a.InquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Quotations)
            .WithOne(q => q.Inquiry)
            .HasForeignKey(q => q.InquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        // TenantId FK — no cascade; tenant deletion handled in application layer
        builder.HasOne<Domain.Entities.Tenant>()
            .WithMany(t => t.Inquiries)
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
