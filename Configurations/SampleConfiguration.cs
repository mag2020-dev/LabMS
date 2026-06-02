namespace LabMS.Configurations;

public class SampleConfiguration : IEntityTypeConfiguration<Sample>
{
    public void Configure(EntityTypeBuilder<Sample> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SampleCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.SampleCode)
            .IsUnique();

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.CollectedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.StorageLocation)
            .HasMaxLength(200);

        builder.Property(s => s.Container)
            .HasMaxLength(100);

        builder.Property(s => s.Volume)
            .HasPrecision(10, 2);

        builder.Property(s => s.VolumeUnit)
            .HasMaxLength(20);

        builder.Property(s => s.RejectionReason)
            .HasMaxLength(500);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(s => s.TestOrder)
            .WithMany()
            .HasForeignKey(s => s.TestOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.CollectedBy)
            .WithMany()
            .HasForeignKey(s => s.CollectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ReceivedBy)
            .WithMany()
            .HasForeignKey(s => s.ReceivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.TrackingHistory)
            .WithOne(st => st.Sample)
            .HasForeignKey(st => st.SampleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
