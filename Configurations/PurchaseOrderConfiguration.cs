namespace LabMS.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(po => po.Id);

        builder.Property(po => po.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(po => po.OrderNumber)
            .IsUnique();

        builder.Property(po => po.OrderDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(po => po.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(PurchaseOrderStatus.Draft);

        builder.Property(po => po.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(po => po.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(po => po.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.CreatedBy)
            .WithMany()
            .HasForeignKey(po => po.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.ApprovedBy)
            .WithMany()
            .HasForeignKey(po => po.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
