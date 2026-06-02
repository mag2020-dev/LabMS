namespace LabMS.Configurations;

public class TestPackageItemConfiguration : IEntityTypeConfiguration<TestPackageItem>
{
    public void Configure(EntityTypeBuilder<TestPackageItem> builder)
    {
        builder.HasKey(tpi => tpi.Id);

        builder.Property(tpi => tpi.Quantity)
            .HasDefaultValue(1);

        builder.Property(tpi => tpi.IsOptional)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(tpi => tpi.TestPackage)
            .WithMany(tp => tp.TestPackageItems)
            .HasForeignKey(tpi => tpi.TestPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tpi => tpi.LabTest)
            .WithMany()
            .HasForeignKey(tpi => tpi.LabTestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
