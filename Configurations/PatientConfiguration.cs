


namespace LabMS.Configurations;
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.DateOfBirth).IsRequired();
            builder.Property(p => p.Gender).IsRequired().HasMaxLength(10);
            builder.Property(p => p.Phone).HasMaxLength(20);
        }
    }

