using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class CommissionAccrualConfiguration : IEntityTypeConfiguration<CommissionAccrual>
{
    public void Configure(EntityTypeBuilder<CommissionAccrual> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.AgentId).IsRequired();
        builder.Property(c => c.TenantId).IsRequired();
        builder.Property(c => c.CommissionRuleId).IsRequired();
        builder.Property(c => c.TriggerType).HasConversion<string>().IsRequired();
        builder.Property(c => c.SourceId).IsRequired();
        builder.Property(c => c.CommissionAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.CommissionPercentage).HasPrecision(5, 2);
        builder.Property(c => c.BaseAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().IsRequired();
        builder.Property(c => c.AccruedAt).IsRequired();
        builder.Property(c => c.PaidAt);
        builder.Property(c => c.PayoutId);
        builder.Property(c => c.DisputeTicketId);

        builder.HasIndex(c => c.AgentId);
        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.Status);
    }
}
