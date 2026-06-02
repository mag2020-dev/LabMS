namespace LabMS.Configurations;

public class TestPackageConfiguration : IEntityTypeConfiguration<TestPackage>
{
    public void Configure(EntityTypeBuilder<TestPackage> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tp => tp.Code)
            .HasMaxLength(50);

        builder.Property(tp => tp.Description)
            .HasMaxLength(1000);

        builder.Property(tp => tp.RegularPrice)
            .HasPrecision(18, 2);

        builder.Property(tp => tp.PackagePrice)
            .HasPrecision(18, 2);

        builder.Property(tp => tp.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(tp => tp.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(tp => tp.IsActive)
            .HasDefaultValue(true);

        builder.Property(tp => tp.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasMany(tp => tp.TestPackageItems)
            .WithOne(tpi => tpi.TestPackage)
            .HasForeignKey(tpi => tpi.TestPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
