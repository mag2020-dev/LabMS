namespace LabMS.Entities;

public class InventoryTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = default!;

    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public int QuantityAfter { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public string? Reference { get; set; } // PO number, test order, etc.
    public string? Notes { get; set; }

    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}

public enum TransactionType
{
    Purchase,
    Usage,
    Adjustment,
    Return,
    Disposal,
    Transfer
}
