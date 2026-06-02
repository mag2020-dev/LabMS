

namespace LabMS.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.GeneratedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(r => r.Visit)
            .WithOne(v => v.Report)
            .HasForeignKey<Report>(r => r.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.GeneratedBy)
            .WithMany()
            .HasForeignKey(r => r.GeneratedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
