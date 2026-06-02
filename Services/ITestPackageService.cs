namespace LabMS.Services;

public interface ITestPackageService
{
    Task<TestPackageResponse?> CreateAsync(TestPackageCreateRequest request);
    Task<TestPackageResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<TestPackageResponse>> GetAllAsync();
    Task<IEnumerable<TestPackageResponse>> GetActiveAsync();
    Task<IEnumerable<TestPackageResponse>> GetByTypeAsync(PackageType type);
    Task<bool> UpdateAsync(Guid id, TestPackageUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
    Task<bool> DeactivateAsync(Guid id);
}
