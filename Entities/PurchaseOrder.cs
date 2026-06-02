namespace LabMS.Entities;

public class PurchaseOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string OrderNumber { get; set; } = default!;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = default!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = default!;

    public Guid? ApprovedByUserId { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
}

public enum PurchaseOrderStatus
{
    Draft,
    Submitted,
    Approved,
    Ordered,
    PartiallyReceived,
    Received,
    Cancelled
}
