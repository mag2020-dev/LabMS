namespace LabMS.Services;

public interface IPurchaseOrderService
{
    Task<PurchaseOrderResponse?> CreateAsync(PurchaseOrderCreateRequest request, Guid createdByUserId);
    Task<PurchaseOrderResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<PurchaseOrderResponse>> GetAllAsync();
    Task<IEnumerable<PurchaseOrderResponse>> GetBySupplierAsync(Guid supplierId);
    Task<IEnumerable<PurchaseOrderResponse>> GetByStatusAsync(PurchaseOrderStatus status);
    Task<bool> ApproveAsync(Guid id, Guid approvedByUserId);
    Task<bool> SubmitAsync(Guid id);
    Task<bool> CancelAsync(Guid id);
    Task<bool> ReceiveItemAsync(Guid orderId, PurchaseOrderReceiveRequest request);
}
