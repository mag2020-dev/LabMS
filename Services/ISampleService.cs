namespace LabMS.Services;

public interface ISampleService
{
    Task<SampleResponse?> CreateAsync(SampleCreateRequest request);
    Task<SampleResponse?> GetByIdAsync(Guid id);
    Task<SampleResponse?> GetBySampleCodeAsync(string sampleCode);
    Task<IEnumerable<SampleResponse>> GetAllAsync();
    Task<IEnumerable<SampleResponse>> GetByTestOrderIdAsync(Guid testOrderId);
    Task<IEnumerable<SampleResponse>> GetByStatusAsync(SampleStatus status);
    Task<IEnumerable<SampleResponse>> GetByPatientIdAsync(Guid patientId);
    Task<bool> UpdateAsync(Guid id, SampleUpdateRequest request);
    Task<bool> ReceiveAsync(Guid id, SampleReceiveRequest request);
    Task<bool> RejectAsync(Guid id, SampleRejectRequest request);
    Task<bool> MarkAsProcessedAsync(Guid id);
    Task<bool> MarkAsDisposedAsync(Guid id);
    Task<IEnumerable<SampleTrackingResponse>> GetTrackingHistoryAsync(Guid sampleId);
    Task<SampleStatisticsResponse> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}
