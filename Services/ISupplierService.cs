namespace LabMS.Services;

public interface ISupplierService
{
    Task<SupplierResponse?> CreateAsync(SupplierCreateRequest request);
    Task<SupplierResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<SupplierResponse>> GetAllAsync();
    Task<IEnumerable<SupplierResponse>> GetActiveAsync();
    Task<bool> UpdateAsync(Guid id, SupplierUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}
