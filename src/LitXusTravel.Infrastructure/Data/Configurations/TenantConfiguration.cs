using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
        builder.Property(t => t.Slug).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Subdomain).HasMaxLength(100);
        builder.Property(t => t.LogoUrl).HasMaxLength(500);
        builder.Property(t => t.WebsiteUrl).HasMaxLength(500);
        builder.Property(t => t.ContactPhone).HasMaxLength(20);
        builder.Property(t => t.Country).HasMaxLength(100);
        builder.Property(t => t.ProvisioningStatus)
            .HasConversion<string>().HasMaxLength(50);

        builder.OwnsOne(t => t.ContactEmail, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("ContactEmail")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.Subdomain).IsUnique().HasFilter("[Subdomain] IS NOT NULL");
        builder.HasIndex(t => t.IsActive);

        builder.HasMany(t => t.Subscriptions)
            .WithOne()
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
