using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type).IsRequired().HasMaxLength(100);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(255);
        builder.Property(n => n.RelatedEntityType).HasMaxLength(100);

        builder.HasIndex(n => n.TenantId);
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);

        // TenantId — no FK constraint to avoid cascade conflicts; TenantId used for filtering only
        builder.HasOne<Domain.Entities.Tenant>()
            .WithMany()
            .HasForeignKey(n => n.TenantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
