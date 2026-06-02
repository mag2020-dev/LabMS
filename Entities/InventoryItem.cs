namespace LabMS.Entities;

public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? Description { get; set; }

    public InventoryCategory Category { get; set; }
    public string? Manufacturer { get; set; }

    public int Quantity { get; set; }
    public int MinimumStockLevel { get; set; }
    public int ReorderLevel { get; set; }
    public string Unit { get; set; } = default!; // ml, units, pieces, etc.

    public decimal UnitPrice { get; set; }

    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }

    public string? StorageLocation { get; set; }
    public string? StorageConditions { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<InventoryTransaction> Transactions { get; set; } = [];
    public ICollection<InventoryAlert> Alerts { get; set; } = [];
}

public enum InventoryCategory
{
    Reagent,
    Consumable,
    Equipment,
    Chemical,
    Calibrator,
    Control,
    Other
}
