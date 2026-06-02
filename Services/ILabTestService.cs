

namespace LabMS.Services;



public interface ILabTestService
{
    Task<IEnumerable<LabTestResponse>> GetAllAsync();
    Task<LabTestResponse?> GetByIdAsync(Guid id);
    Task<LabTestResponse> AddAsync(LabTestCreateRequest request);
    Task<bool> UpdateAsync(Guid id, LabTestUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}