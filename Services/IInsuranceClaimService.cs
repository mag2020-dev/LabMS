namespace LabMS.Services;

public interface IInsuranceClaimService
{
    Task<InsuranceClaimResponse?> CreateAsync(InsuranceClaimCreateRequest request, Guid submittedByUserId);
    Task<InsuranceClaimResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<InsuranceClaimResponse>> GetAllAsync();
    Task<IEnumerable<InsuranceClaimResponse>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<InsuranceClaimResponse>> GetByStatusAsync(ClaimStatus status);
    Task<bool> UpdateAsync(Guid id, InsuranceClaimUpdateRequest request);
    Task<bool> SubmitAsync(Guid id);
    Task<bool> ApproveAsync(Guid id, decimal approvedAmount);
    Task<bool> RejectAsync(Guid id, string reason);
    Task<bool> MarkAsPaidAsync(Guid id);
}
