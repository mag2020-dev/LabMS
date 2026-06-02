namespace LabMS.Services;

public class InventoryService(LabMSDbContext context, IMapper mapper) : IInventoryService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<InventoryItemResponse?> CreateItemAsync(InventoryItemCreateRequest request)
    {
        var item = new InventoryItem
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Category = request.Category,
            Manufacturer = request.Manufacturer,
            Quantity = request.Quantity,
            MinimumStockLevel = request.MinimumStockLevel,
            ReorderLevel = request.ReorderLevel,
            Unit = request.Unit,
            UnitPrice = request.UnitPrice,
            ExpiryDate = request.ExpiryDate,
            BatchNumber = request.BatchNumber,
            StorageLocation = request.StorageLocation,
            StorageConditions = request.StorageConditions,
            IsActive = true
        };

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        // Check and create alerts
        await CheckAndCreateAlertsAsync(item);

        return await GetItemByIdAsync(item.Id);
    }

    public async Task<InventoryItemResponse?> GetItemByIdAsync(Guid id)
    {
        var item = await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item is null) return null;

        return _mapper.Map<InventoryItemResponse>(item);
    }

    public async Task<IEnumerable<InventoryItemResponse>> GetAllItemsAsync()
    {
        var items = await _context.InventoryItems
            .OrderBy(i => i.Name)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(_mapper.Map<InventoryItemResponse>);
    }

    public async Task<IEnumerable<InventoryItemResponse>> GetLowStockItemsAsync()
    {
        var items = await _context.InventoryItems
            .Where(i => i.IsActive && i.Quantity <= i.ReorderLevel)
            .OrderBy(i => i.Quantity)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(_mapper.Map<InventoryItemResponse>);
    }

    public async Task<IEnumerable<InventoryItemResponse>> GetExpiringItemsAsync(int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        var items = await _context.InventoryItems
            .Where(i => i.IsActive && i.ExpiryDate.HasValue && i.ExpiryDate <= thresholdDate)
            .OrderBy(i => i.ExpiryDate)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(_mapper.Map<InventoryItemResponse>);
    }

    public async Task<IEnumerable<InventoryItemResponse>> GetByCategoryAsync(InventoryCategory category)
    {
        var items = await _context.InventoryItems
            .Where(i => i.Category == category && i.IsActive)
            .OrderBy(i => i.Name)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(_mapper.Map<InventoryItemResponse>);
    }

    public async Task<bool> UpdateItemAsync(Guid id, InventoryItemUpdateRequest request)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null) return false;

        if (request.Name != null) item.Name = request.Name;
        if (request.Description != null) item.Description = request.Description;
        if (request.Quantity.HasValue) item.Quantity = request.Quantity.Value;
        if (request.MinimumStockLevel.HasValue) item.MinimumStockLevel = request.MinimumStockLevel.Value;
        if (request.ReorderLevel.HasValue) item.ReorderLevel = request.ReorderLevel.Value;
        if (request.UnitPrice.HasValue) item.UnitPrice = request.UnitPrice.Value;
        if (request.ExpiryDate.HasValue) item.ExpiryDate = request.ExpiryDate;
        if (request.BatchNumber != null) item.BatchNumber = request.BatchNumber;
        if (request.StorageLocation != null) item.StorageLocation = request.StorageLocation;
        if (request.StorageConditions != null) item.StorageConditions = request.StorageConditions;
        if (request.IsActive.HasValue) item.IsActive = request.IsActive.Value;

        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await CheckAndCreateAlertsAsync(item);

        return true;
    }

    public async Task<bool> DeleteItemAsync(Guid id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null) return false;

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdjustStockAsync(Guid itemId, int quantity, Guid userId, string? notes = null)
    {
        var item = await _context.InventoryItems.FindAsync(itemId);
        if (item is null) return false;

        var oldQuantity = item.Quantity;
        item.Quantity += quantity;

        var transaction = new InventoryTransaction
        {
            InventoryItemId = itemId,
            Type = quantity > 0 ? TransactionType.Purchase : TransactionType.Usage,
            Quantity = Math.Abs(quantity),
            QuantityAfter = item.Quantity,
            UserId = userId,
            Notes = notes
        };

        _context.InventoryTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        await CheckAndCreateAlertsAsync(item);

        return true;
    }

    public async Task<IEnumerable<InventoryTransactionResponse>> GetTransactionHistoryAsync(Guid itemId)
    {
        var transactions = await _context.InventoryTransactions
            .Include(t => t.User)
            .Include(t => t.Supplier)
            .Include(t => t.InventoryItem)
            .Where(t => t.InventoryItemId == itemId)
            .OrderByDescending(t => t.TransactionDate)
            .AsNoTracking()
            .ToListAsync();

        return transactions.Select(_mapper.Map<InventoryTransactionResponse>);
    }

    public async Task<IEnumerable<InventoryAlertResponse>> GetActiveAlertsAsync()
    {
        var alerts = await _context.InventoryAlerts
            .Include(a => a.InventoryItem)
            .Include(a => a.ResolvedBy)
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.Severity)
            .ThenBy(a => a.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return alerts.Select(_mapper.Map<InventoryAlertResponse>);
    }

    public async Task<bool> ResolveAlertAsync(Guid alertId, Guid userId)
    {
        var alert = await _context.InventoryAlerts.FindAsync(alertId);
        if (alert is null) return false;

        alert.IsResolved = true;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedByUserId = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InventoryStatisticsResponse> GetStatisticsAsync()
    {
        var items = await _context.InventoryItems.ToListAsync();

        var itemsByCategory = items
            .GroupBy(i => i.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        var totalValue = items.Sum(i => i.Quantity * i.UnitPrice);

        return new InventoryStatisticsResponse(
            items.Count,
            items.Count(i => i.IsActive),
            items.Count(i => i.Quantity <= i.ReorderLevel),
            items.Count(i => i.Quantity == 0),
            items.Count(i => i.ExpiryDate.HasValue && i.ExpiryDate <= DateTime.UtcNow.AddDays(30)),
            totalValue,
            itemsByCategory
        );
    }

    private async Task CheckAndCreateAlertsAsync(InventoryItem item)
    {
        // Check low stock
        if (item.Quantity <= item.MinimumStockLevel)
        {
            var existingAlert = await _context.InventoryAlerts
                .Where(a => a.InventoryItemId == item.Id && a.Type == AlertType.LowStock && !a.IsResolved)
                .FirstOrDefaultAsync();

            if (existingAlert is null)
            {
                _context.InventoryAlerts.Add(new InventoryAlert
                {
                    InventoryItemId = item.Id,
                    Type = item.Quantity == 0 ? AlertType.OutOfStock : AlertType.LowStock,
                    Severity = item.Quantity == 0 ? AlertSeverity.Critical : AlertSeverity.High,
                    Message = item.Quantity == 0 
                        ? $"{item.Name} is out of stock" 
                        : $"{item.Name} is below minimum stock level ({item.Quantity} {item.Unit})"
                });
            }
        }

        // Check expiry
        if (item.ExpiryDate.HasValue)
        {
            var daysToExpiry = (item.ExpiryDate.Value - DateTime.UtcNow).Days;

            if (daysToExpiry <= 0)
            {
                var existingAlert = await _context.InventoryAlerts
                    .Where(a => a.InventoryItemId == item.Id && a.Type == AlertType.Expired && !a.IsResolved)
                    .FirstOrDefaultAsync();

                if (existingAlert is null)
                {
                    _context.InventoryAlerts.Add(new InventoryAlert
                    {
                        InventoryItemId = item.Id,
                        Type = AlertType.Expired,
                        Severity = AlertSeverity.Critical,
                        Message = $"{item.Name} has expired (Batch: {item.BatchNumber})"
                    });
                }
            }
            else if (daysToExpiry <= 30)
            {
                var existingAlert = await _context.InventoryAlerts
                    .Where(a => a.InventoryItemId == item.Id && a.Type == AlertType.ExpiryWarning && !a.IsResolved)
                    .FirstOrDefaultAsync();

                if (existingAlert is null)
                {
                    _context.InventoryAlerts.Add(new InventoryAlert
                    {
                        InventoryItemId = item.Id,
                        Type = AlertType.ExpiryWarning,
                        Severity = daysToExpiry <= 7 ? AlertSeverity.High : AlertSeverity.Medium,
                        Message = $"{item.Name} expires in {daysToExpiry} days (Batch: {item.BatchNumber})"
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    
}
