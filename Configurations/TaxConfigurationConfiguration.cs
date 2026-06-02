namespace LabMS.Configurations;

public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Code)
            .HasMaxLength(50);

        builder.HasIndex(t => t.Code)
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");

        builder.Property(t => t.Rate)
            .HasPrecision(18, 4);

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.Description)
            .HasMaxLength(500);
    }
}
