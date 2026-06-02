namespace LabMS.Services;

public interface IPaymentPlanService
{
    Task<PaymentPlanResponse?> CreateAsync(PaymentPlanCreateRequest request, Guid createdByUserId);
    Task<PaymentPlanResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentPlanResponse>> GetAllAsync();
    Task<IEnumerable<PaymentPlanResponse>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<PaymentPlanResponse>> GetActiveAsync();
    Task<bool> ProcessPaymentAsync(Guid planId, PaymentPlanPaymentRequest request);
    Task<bool> CancelAsync(Guid id);
}
