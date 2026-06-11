using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class DisputeResolutionTicketConfiguration : IEntityTypeConfiguration<DisputeResolutionTicket>
{
    public void Configure(EntityTypeBuilder<DisputeResolutionTicket> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.SuperAdminId).IsRequired();
        builder.Property(d => d.CommissionAccrualId).IsRequired();
        builder.Property(d => d.Description).IsRequired().HasMaxLength(1000);
        builder.Property(d => d.ProposedFix).IsRequired().HasMaxLength(2000);
        builder.Property(d => d.ReasonCode).HasConversion<string>().IsRequired();
        builder.Property(d => d.Status).HasConversion<string>().IsRequired();
        builder.Property(d => d.ReviewedByTenantAdminId);
        builder.Property(d => d.ResolvedAt);
        builder.Property(d => d.OriginalAmount).HasPrecision(18, 2);
        builder.Property(d => d.AdjustedAmount).HasPrecision(18, 2);

        builder.HasIndex(d => d.Status);
    }
}
