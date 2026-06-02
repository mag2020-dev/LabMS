

namespace LabMS.Services;

public interface IVisitService
{
    Task<IEnumerable<VisitResponse>> GetAllAsync();
    Task<VisitResponse?> GetByIdAsync(Guid id);
    Task<VisitResponse> AddAsync(VisitCreateRequest request);
    Task<bool> UpdateAsync(Guid id, VisitUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}
