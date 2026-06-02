namespace LabMS.Services;

public interface ITaxService
{
    Task<TaxConfigurationResponse?> CreateConfigurationAsync(TaxConfigurationCreateRequest request);
    Task<TaxConfigurationResponse?> GetConfigurationByIdAsync(Guid id);
    Task<IEnumerable<TaxConfigurationResponse>> GetAllConfigurationsAsync();
    Task<IEnumerable<TaxConfigurationResponse>> GetActiveConfigurationsAsync();
    Task<bool> UpdateConfigurationAsync(Guid id, TaxConfigurationUpdateRequest request);
    Task<bool> DeleteConfigurationAsync(Guid id);
    Task<TaxCalculationResponse> CalculateTaxAsync(decimal amount, List<Guid>? taxConfigurationIds = null);
}
