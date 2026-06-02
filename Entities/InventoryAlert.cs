namespace LabMS.Entities;

public class InventoryAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = default!;

    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }

    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsResolved { get; set; } = false;
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedByUserId { get; set; }
    public User? ResolvedBy { get; set; }
}

public enum AlertType
{
    LowStock,
    OutOfStock,
    ExpiryWarning,
    Expired,
    ReorderRequired
}

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}
