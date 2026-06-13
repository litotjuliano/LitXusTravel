using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(255);
        builder.Property(a => a.Role).HasConversion<string>().IsRequired();
        builder.Property(a => a.Scope).HasConversion<string>().IsRequired();
        builder.Property(a => a.AssignedTenantId);
        builder.Property(a => a.IsActive).IsRequired();

        builder.OwnsOne(a => a.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.HasIndex(a => a.IsActive);
    }
}
