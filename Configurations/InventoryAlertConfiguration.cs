namespace LabMS.Configurations;

public class InventoryAlertConfiguration : IEntityTypeConfiguration<InventoryAlert>
{
    public void Configure(EntityTypeBuilder<InventoryAlert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Severity)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.IsResolved)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(a => a.InventoryItem)
            .WithMany(i => i.Alerts)
            .HasForeignKey(a => a.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.ResolvedBy)
            .WithMany()
            .HasForeignKey(a => a.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
