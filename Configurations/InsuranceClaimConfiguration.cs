namespace LabMS.Configurations;

public class InsuranceClaimConfiguration : IEntityTypeConfiguration<InsuranceClaim>
{
    public void Configure(EntityTypeBuilder<InsuranceClaim> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ClaimNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => c.ClaimNumber)
            .IsUnique();

        builder.Property(c => c.InsuranceProvider)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PolicyNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.GroupNumber)
            .HasMaxLength(100);

        builder.Property(c => c.ClaimAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.ApprovedAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.PatientResponsibility)
            .HasPrecision(18, 2);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(500);

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(c => c.Invoice)
            .WithMany()
            .HasForeignKey(c => c.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Patient)
            .WithMany()
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.SubmittedBy)
            .WithMany()
            .HasForeignKey(c => c.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
