namespace LabMS.Services;

public interface IInventoryService
{
    Task<InventoryItemResponse?> CreateItemAsync(InventoryItemCreateRequest request);
    Task<InventoryItemResponse?> GetItemByIdAsync(Guid id);
    Task<IEnumerable<InventoryItemResponse>> GetAllItemsAsync();
    Task<IEnumerable<InventoryItemResponse>> GetLowStockItemsAsync();
    Task<IEnumerable<InventoryItemResponse>> GetExpiringItemsAsync(int daysThreshold = 30);
    Task<IEnumerable<InventoryItemResponse>> GetByCategoryAsync(InventoryCategory category);
    Task<bool> UpdateItemAsync(Guid id, InventoryItemUpdateRequest request);
    Task<bool> DeleteItemAsync(Guid id);
    Task<bool> AdjustStockAsync(Guid itemId, int quantity, Guid userId, string? notes = null);
    Task<IEnumerable<InventoryTransactionResponse>> GetTransactionHistoryAsync(Guid itemId);
    Task<IEnumerable<InventoryAlertResponse>> GetActiveAlertsAsync();
    Task<bool> ResolveAlertAsync(Guid alertId, Guid userId);
    Task<InventoryStatisticsResponse> GetStatisticsAsync();
}
