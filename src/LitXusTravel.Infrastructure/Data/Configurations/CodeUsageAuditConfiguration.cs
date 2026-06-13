using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class CodeUsageAuditConfiguration : IEntityTypeConfiguration<CodeUsageAudit>
{
    public void Configure(EntityTypeBuilder<CodeUsageAudit> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.TenantId).IsRequired();
        builder.Property(c => c.Code).IsRequired().HasMaxLength(100);
        builder.Property(c => c.StaffAgentId);
        builder.Property(c => c.UsedAt).IsRequired();
        builder.Property(c => c.CustomerIp).HasMaxLength(50);
        builder.Property(c => c.CustomerLocation).HasMaxLength(255);
        builder.Property(c => c.BookingId).IsRequired();

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.Code);
        builder.HasIndex(c => c.StaffAgentId);
    }
}
