

namespace LabMS.Services;

public interface IDoctorService
{
    Task<IEnumerable<DoctorResponse>> GetAllAsync();
    Task<DoctorResponse?> GetByIdAsync(Guid id);
    Task<DoctorResponse> AddAsync(DoctorCreateRequest request);
    Task<bool> UpdateAsync(Guid id, DoctorUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}
