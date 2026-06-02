


namespace LabMS.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(d => d.Specialty).IsRequired().HasMaxLength(100);
            builder.Property(d => d.PhoneNumber).HasMaxLength(20);
        }
    }




