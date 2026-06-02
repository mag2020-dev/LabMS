

namespace LabMS.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits");

        builder.HasKey(v => v.Id);

        // Foreign keys
        builder.HasOne(v => v.Patient)
            .WithMany(p => p.Visits)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Doctor)
            .WithMany(d => d.Visits)
            .HasForeignKey(v => v.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Shadow props for UX
        builder.Property(v => v.PatientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.DoctorName)
            .HasMaxLength(200);

        builder.Property(v => v.Reason)
            .HasMaxLength(500);

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
