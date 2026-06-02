

namespace LabMS.Services;

public interface ITestResultService
{
    Task<IEnumerable<TestResultResponse>> GetAllAsync();
    Task<TestResultResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<TestResultResponse>> GetByTestOrderIdAsync(Guid testOrderId);
    Task<TestResultResponse?> AddAsync(TestResultCreateRequest request);
    Task<bool> VerifyAsync(Guid id, TestResultVerifyRequest request);
    Task<bool> UpdateAsync(Guid id, TestResultUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}
