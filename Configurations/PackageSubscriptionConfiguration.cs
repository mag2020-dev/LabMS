namespace LabMS.Configurations;

public class PackageSubscriptionConfiguration : IEntityTypeConfiguration<PackageSubscription>
{
    public void Configure(EntityTypeBuilder<PackageSubscription> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.StartDate)
            .IsRequired();

        builder.Property(ps => ps.EndDate)
            .IsRequired();

        builder.Property(ps => ps.Frequency)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ps => ps.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(SubscriptionStatus.Active);

        builder.Property(ps => ps.MonthlyPrice)
            .HasPrecision(18, 2);

        builder.Property(ps => ps.TestsRemaining)
            .HasDefaultValue(0);

        builder.Property(ps => ps.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ps => ps.CancellationReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(ps => ps.Patient)
            .WithMany()
            .HasForeignKey(ps => ps.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.TestPackage)
            .WithMany()
            .HasForeignKey(ps => ps.TestPackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
