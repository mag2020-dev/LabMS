namespace LabMS.Services;
public interface IInvoiceService
{
    Task<InvoiceResponse?> AddInvoiceForVisitAsync(Guid visitId, decimal discount = 0);
    Task<InvoiceResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<InvoiceResponse>> GetAllAsync();
    Task<bool> UpdateAsync(Guid id, InvoiceUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}