using LabMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabMS.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Discount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        builder.Property(i => i.IssuedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(i => i.Visit)
            .WithOne(v => v.Invoice)
            .HasForeignKey<Invoice>(i => i.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Payments)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId);
    }
}
