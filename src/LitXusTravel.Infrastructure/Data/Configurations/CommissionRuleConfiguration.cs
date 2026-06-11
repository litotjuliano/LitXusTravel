using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class CommissionRuleConfiguration : IEntityTypeConfiguration<CommissionRule>
{
    public void Configure(EntityTypeBuilder<CommissionRule> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.TenantId).IsRequired();
        builder.Property(c => c.AgentId);
        builder.Property(c => c.Trigger).HasConversion<string>().IsRequired();
        builder.Property(c => c.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.IsPercentage).IsRequired();
        builder.Property(c => c.PayoutFrequency).HasMaxLength(50);
        builder.Property(c => c.AutoApprove).IsRequired();
        builder.Property(c => c.MinimumThreshold).HasPrecision(18, 2);
        builder.Property(c => c.EffectiveFrom).IsRequired();
        builder.Property(c => c.EffectiveTo);
        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.AgentId);
    }
}
