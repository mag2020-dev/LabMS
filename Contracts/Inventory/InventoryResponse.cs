namespace LabMS.Contracts.Inventory;

public record InventoryItemResponse(
    Guid Id,
    string Name,
    string? Code,
    string? Description,
    InventoryCategory Category,
    string? Manufacturer,
    int Quantity,
    int MinimumStockLevel,
    int ReorderLevel,
    string Unit,
    decimal UnitPrice,
    DateTime? ExpiryDate,
    string? BatchNumber,
    string? StorageLocation,
    string? StorageConditions,
    bool IsActive,
    bool IsLowStock,
    bool IsExpiringSoon,
    DateTime CreatedAt
);

public record InventoryTransactionResponse(
    Guid Id,
    Guid InventoryItemId,
    string ItemName,
    TransactionType Type,
    int Quantity,
    int QuantityAfter,
    DateTime TransactionDate,
    string? UserName,
    string? Reference,
    string? Notes,
    string? SupplierName
);

public record InventoryAlertResponse(
    Guid Id,
    Guid InventoryItemId,
    string ItemName,
    AlertType Type,
    AlertSeverity Severity,
    string Message,
    DateTime CreatedAt,
    bool IsResolved,
    DateTime? ResolvedAt,
    string? ResolvedByName
);

public record SupplierResponse(
    Guid Id,
    string Name,
    string? Code,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime CreatedAt
);

public record PurchaseOrderResponse(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    string SupplierName,
    DateTime OrderDate,
    DateTime? ExpectedDeliveryDate,
    DateTime? ActualDeliveryDate,
    PurchaseOrderStatus Status,
    decimal TotalAmount,
    string? Notes,
    string CreatedByName,
    string? ApprovedByName,
    DateTime? ApprovedAt,
    List<PurchaseOrderItemResponse> Items
);

public record PurchaseOrderItemResponse(
    Guid Id,
    Guid InventoryItemId,
    string ItemName,
    int QuantityOrdered,
    int QuantityReceived,
    decimal UnitPrice,
    decimal TotalPrice
);

public record InventoryStatisticsResponse(
    int TotalItems,
    int ActiveItems,
    int LowStockItems,
    int OutOfStockItems,
    int ExpiringItems,
    decimal TotalInventoryValue,
    Dictionary<InventoryCategory, int> ItemsByCategory
);
