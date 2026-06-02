namespace LabMS.Services;

public interface IReportService
{
    Task<IEnumerable<ReportResponse>> GetAllAsync();
    Task<ReportResponse?> GetByIdAsync(Guid id);
    Task<ReportResponse?> AddAsync(ReportCreateRequest request);
    Task<bool> UpdateAsync(Guid id, ReportUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}
