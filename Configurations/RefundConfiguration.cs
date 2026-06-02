namespace LabMS.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RefundNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.RefundNumber)
            .IsUnique();

        builder.Property(r => r.Amount)
            .HasPrecision(18, 2);

        builder.Property(r => r.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Method)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.RequestedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(r => r.Payment)
            .WithMany()
            .HasForeignKey(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RequestedBy)
            .WithMany()
            .HasForeignKey(r => r.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ApprovedBy)
            .WithMany()
            .HasForeignKey(r => r.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
