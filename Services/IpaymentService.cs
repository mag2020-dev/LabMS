namespace LabMS.Services;

public interface IPaymentService
{
    Task<PaymentResponse?> AddPaymentAsync(PaymentCreateRequest request);
    Task<IEnumerable<PaymentResponse>> GetByInvoiceIdAsync(Guid invoiceId);
    Task<PaymentResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentResponse>> GetAllAsync();
    Task<bool> UpdateAsync(Guid id, PaymentUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}
