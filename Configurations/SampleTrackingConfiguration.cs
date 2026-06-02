namespace LabMS.Configurations;

public class SampleTrackingConfiguration : IEntityTypeConfiguration<SampleTracking>
{
    public void Configure(EntityTypeBuilder<SampleTracking> builder)
    {
        builder.HasKey(st => st.Id);

        builder.Property(st => st.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(st => st.Timestamp)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(st => st.Location)
            .HasMaxLength(200);

        builder.Property(st => st.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(st => st.Sample)
            .WithMany(s => s.TrackingHistory)
            .HasForeignKey(st => st.SampleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.User)
            .WithMany()
            .HasForeignKey(st => st.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
