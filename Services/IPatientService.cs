
namespace LabMS.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientResponse>> GetAllAsync();
    Task<PatientResponse?> GetByIdAsync(Guid id);
    Task<PatientResponse> AddAsync(PatientCreateRequest request);
    Task<bool> UpdateAsync(Guid id, PatientUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}


