using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class CrmActivityConfiguration : IEntityTypeConfiguration<CrmActivity>
{
    public void Configure(EntityTypeBuilder<CrmActivity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Notes).HasMaxLength(2000);

        builder.HasIndex(a => a.InquiryId);
        builder.HasIndex(a => a.TenantId);

        // Cascaded from Inquiry; TenantId is for query filtering only — no FK navigation needed
        builder.HasOne(a => a.Inquiry)
            .WithMany(i => i.Activities)
            .HasForeignKey(a => a.InquiryId)
            .OnDelete(DeleteBehavior.NoAction); // Cascade already set from Inquiry side
    }
}
