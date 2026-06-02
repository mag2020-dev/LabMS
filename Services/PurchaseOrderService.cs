namespace LabMS.Services;

public class PurchaseOrderService(LabMSDbContext context, IMapper mapper) : IPurchaseOrderService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PurchaseOrderResponse?> CreateAsync(PurchaseOrderCreateRequest request, Guid createdByUserId)
    {
        var orderNumber = await GenerateOrderNumberAsync();

        var purchaseOrder = new PurchaseOrder
        {
            OrderNumber = orderNumber,
            SupplierId = request.SupplierId,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Notes = request.Notes,
            Status = PurchaseOrderStatus.Draft,
            CreatedByUserId = createdByUserId
        };

        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var poItem = new PurchaseOrderItem
            {
                InventoryItemId = item.InventoryItemId,
                QuantityOrdered = item.QuantityOrdered,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.QuantityOrdered * item.UnitPrice
            };

            totalAmount += poItem.TotalPrice;
            purchaseOrder.Items.Add(poItem);
        }

        purchaseOrder.TotalAmount = totalAmount;

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(purchaseOrder.Id);
    }

    public async Task<PurchaseOrderResponse?> GetByIdAsync(Guid id)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.CreatedBy)
            .Include(p => p.ApprovedBy)
            .Include(p => p.Items)
                .ThenInclude(i => i.InventoryItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (po is null) return null;

        return _mapper.Map<PurchaseOrderResponse>(po);
    }

    public async Task<IEnumerable<PurchaseOrderResponse>> GetAllAsync()
    {
        var orders = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.CreatedBy)
            .Include(p => p.ApprovedBy)
            .Include(p => p.Items)
                .ThenInclude(i => i.InventoryItem)
            .OrderByDescending(p => p.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        return orders.Select(_mapper.Map<PurchaseOrderResponse>);
    }

    public async Task<IEnumerable<PurchaseOrderResponse>> GetBySupplierAsync(Guid supplierId)
    {
        var orders = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.CreatedBy)
            .Include(p => p.ApprovedBy)
            .Include(p => p.Items)
                .ThenInclude(i => i.InventoryItem)
            .Where(p => p.SupplierId == supplierId)
            .OrderByDescending(p => p.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        return orders.Select(_mapper.Map<PurchaseOrderResponse>);
    }

    public async Task<IEnumerable<PurchaseOrderResponse>> GetByStatusAsync(PurchaseOrderStatus status)
    {
        var orders = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.CreatedBy)
            .Include(p => p.ApprovedBy)
            .Include(p => p.Items)
                .ThenInclude(i => i.InventoryItem)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        return orders.Select(_mapper.Map<PurchaseOrderResponse>);
    }

    public async Task<bool> ApproveAsync(Guid id, Guid approvedByUserId)
    {
        var po = await _context.PurchaseOrders.FindAsync(id);
        if (po is null || po.Status != PurchaseOrderStatus.Submitted) return false;

        po.Status = PurchaseOrderStatus.Approved;
        po.ApprovedByUserId = approvedByUserId;
        po.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubmitAsync(Guid id)
    {
        var po = await _context.PurchaseOrders.FindAsync(id);
        if (po is null || po.Status != PurchaseOrderStatus.Draft) return false;

        po.Status = PurchaseOrderStatus.Submitted;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var po = await _context.PurchaseOrders.FindAsync(id);
        if (po is null) return false;

        po.Status = PurchaseOrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReceiveItemAsync(Guid orderId, PurchaseOrderReceiveRequest request)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == orderId);

        if (po is null) return false;

        var item = po.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item is null) return false;

        item.QuantityReceived += request.QuantityReceived;

        // Update inventory
        var inventoryItem = await _context.InventoryItems.FindAsync(item.InventoryItemId);
        if (inventoryItem != null)
        {
            inventoryItem.Quantity += request.QuantityReceived;

            // Create transaction
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                InventoryItemId = item.InventoryItemId,
                Type = TransactionType.Purchase,
                Quantity = request.QuantityReceived,
                QuantityAfter = inventoryItem.Quantity,
                Reference = po.OrderNumber,
                Notes = $"Received from PO {po.OrderNumber}",
                SupplierId = po.SupplierId
            });
        }

        // Update PO status
        var allItemsReceived = po.Items.All(i => i.QuantityReceived >= i.QuantityOrdered);
        var anyItemsReceived = po.Items.Any(i => i.QuantityReceived > 0);

        if (allItemsReceived)
        {
            po.Status = PurchaseOrderStatus.Received;
            po.ActualDeliveryDate = DateTime.UtcNow;
        }
        else if (anyItemsReceived)
        {
            po.Status = PurchaseOrderStatus.PartiallyReceived;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"PO-{date:yyyyMMdd}";
        
        var lastOrder = await _context.PurchaseOrders
            .Where(p => p.OrderNumber.StartsWith(prefix))
            .OrderByDescending(p => p.OrderNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastSequence = lastOrder.OrderNumber.Split('-').Last();
            if (int.TryParse(lastSequence, out int num))
            {
                sequence = num + 1;
            }
        }

        return $"{prefix}-{sequence:D4}";
    }

    
}
