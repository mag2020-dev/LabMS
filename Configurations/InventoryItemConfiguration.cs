namespace LabMS.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Code)
            .HasMaxLength(50);

        builder.HasIndex(i => i.Code)
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Manufacturer)
            .HasMaxLength(200);

        builder.Property(i => i.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.BatchNumber)
            .HasMaxLength(100);

        builder.Property(i => i.StorageLocation)
            .HasMaxLength(200);

        builder.Property(i => i.StorageConditions)
            .HasMaxLength(500);

        builder.Property(i => i.IsActive)
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasMany(i => i.Transactions)
            .WithOne(t => t.InventoryItem)
            .HasForeignKey(t => t.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Alerts)
            .WithOne(a => a.InventoryItem)
            .HasForeignKey(a => a.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
