namespace LabMS.Configurations;

public class DiscountRuleConfiguration : IEntityTypeConfiguration<DiscountRule>
{
    public void Configure(EntityTypeBuilder<DiscountRule> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Code)
            .HasMaxLength(50);

        builder.HasIndex(r => r.Code)
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.Value)
            .HasPrecision(18, 2);

        builder.Property(r => r.Applicability)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.MinimumAmount)
            .HasPrecision(18, 2);

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
