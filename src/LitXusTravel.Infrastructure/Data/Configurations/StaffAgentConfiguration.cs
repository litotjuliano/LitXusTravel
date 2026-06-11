using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class StaffAgentConfiguration : IEntityTypeConfiguration<StaffAgent>
{
    public void Configure(EntityTypeBuilder<StaffAgent> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId).IsRequired();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(255);
        builder.Property(s => s.UniqueCode).IsRequired().HasMaxLength(100);
        builder.Property(s => s.CodeIssuedAt).IsRequired();
        builder.Property(s => s.CodeExpiresAt);
        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.JoinedAt).IsRequired();
        builder.Property(s => s.DepartedAt);

        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.UniqueCode).IsUnique();
        builder.HasIndex(s => s.IsActive);
    }
}
