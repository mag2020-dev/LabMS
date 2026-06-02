namespace LabMS.Entities;

public class PurchaseOrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = default!;

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = default!;

    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; } = 0;

    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
