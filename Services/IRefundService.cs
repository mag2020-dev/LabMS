namespace LabMS.Services;

public interface IRefundService
{
    Task<RefundResponse?> CreateAsync(RefundCreateRequest request, Guid requestedByUserId);
    Task<RefundResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<RefundResponse>> GetAllAsync();
    Task<IEnumerable<RefundResponse>> GetByStatusAsync(RefundStatus status);
    Task<bool> ApproveAsync(Guid id, Guid approvedByUserId, string? notes = null);
    Task<bool> RejectAsync(Guid id, Guid approvedByUserId, string? notes = null);
    Task<bool> ProcessAsync(Guid id);
}
