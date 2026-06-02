namespace LabMS.Contracts.Inventory;

public record InventoryItemCreateRequest(
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
    string? StorageConditions
);

public record InventoryItemUpdateRequest(
    string? Name,
    string? Description,
    int? Quantity,
    int? MinimumStockLevel,
    int? ReorderLevel,
    decimal? UnitPrice,
    DateTime? ExpiryDate,
    string? BatchNumber,
    string? StorageLocation,
    string? StorageConditions,
    bool? IsActive
);

public record InventoryTransactionCreateRequest(
    Guid InventoryItemId,
    TransactionType Type,
    int Quantity,
    Guid? UserId,
    string? Reference,
    string? Notes,
    Guid? SupplierId
);

public record SupplierCreateRequest(
    string Name,
    string? Code,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address
);

public record SupplierUpdateRequest(
    string? Name,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    bool? IsActive
);

public record PurchaseOrderCreateRequest(
    Guid SupplierId,
    DateTime? ExpectedDeliveryDate,
    string? Notes,
    List<PurchaseOrderItemRequest> Items
);

public record PurchaseOrderItemRequest(
    Guid InventoryItemId,
    int QuantityOrdered,
    decimal UnitPrice
);

public record PurchaseOrderReceiveRequest(
    Guid ItemId,
    int QuantityReceived
);
