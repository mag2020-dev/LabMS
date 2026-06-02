namespace LabMS.Services;

public interface IDiscountService
{
    Task<DiscountRuleResponse?> CreateRuleAsync(DiscountRuleCreateRequest request);
    Task<DiscountRuleResponse?> GetRuleByIdAsync(Guid id);
    Task<IEnumerable<DiscountRuleResponse>> GetAllRulesAsync();
    Task<IEnumerable<DiscountRuleResponse>> GetActiveRulesAsync();
    Task<bool> UpdateRuleAsync(Guid id, DiscountRuleUpdateRequest request);
    Task<bool> DeleteRuleAsync(Guid id);
    Task<DiscountCalculationResponse> CalculateDiscountAsync(Guid invoiceId, List<Guid>? discountRuleIds = null);
    Task<bool> ApplyDiscountToInvoiceAsync(Guid invoiceId, List<Guid> discountRuleIds);
}
