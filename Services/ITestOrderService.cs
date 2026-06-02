

namespace LabMS.Services;

public interface ITestOrderService
{
    Task<IEnumerable<TestOrderResponse>> GetAllAsync();
    Task<TestOrderResponse?> GetByIdAsync(Guid id);
    Task<TestOrderResponse?> AddAsync(TestOrderCreateRequest request);
    Task<bool> UpdateAsync(Guid id, TestOrderUpdateRequest request);
    Task<bool> UpdateStatusAsync(Guid id, string newStatus);
    Task<bool> DeleteAsync(Guid id);
}
