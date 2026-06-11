using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Infrastructure.Data.Configurations;

public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.PackageTitle).IsRequired().HasMaxLength(255);
        builder.Property(q => q.Currency).IsRequired().HasMaxLength(10).HasDefaultValue("MYR");
        builder.Property(q => q.Status).HasConversion<string>().HasMaxLength(50);

        builder.Property(q => q.BasePrice).HasColumnType("decimal(10,2)");
        builder.Property(q => q.Markup).HasColumnType("decimal(10,2)");
        builder.Property(q => q.FinalPrice).HasColumnType("decimal(10,2)");
        builder.Property(q => q.TotalPrice).HasColumnType("decimal(10,2)");

        builder.HasIndex(q => q.InquiryId);
        builder.HasIndex(q => q.TenantId);
        builder.HasIndex(q => q.Status);

        // Cascaded from Inquiry; no second cascade from Tenant
        builder.HasOne(q => q.Inquiry)
            .WithMany(i => i.Quotations)
            .HasForeignKey(q => q.InquiryId)
            .OnDelete(DeleteBehavior.NoAction); // Cascade already set from Inquiry side
    }
}
