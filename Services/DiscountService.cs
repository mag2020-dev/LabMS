namespace LabMS.Services;

public class DiscountService(LabMSDbContext context, IMapper mapper) : IDiscountService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<DiscountRuleResponse?> CreateRuleAsync(DiscountRuleCreateRequest request)
    {
        var rule = new DiscountRule
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Type = request.Type,
            Value = request.Value,
            Applicability = request.Applicability,
            SpecificTestId = request.SpecificTestId,
            SpecificPackageId = request.SpecificPackageId,
            MinimumTests = request.MinimumTests,
            MinimumAmount = request.MinimumAmount,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            Priority = request.Priority,
            IsActive = true
        };

        _context.DiscountRules.Add(rule);
        await _context.SaveChangesAsync();

        return await GetRuleByIdAsync(rule.Id);
    }

    public async Task<DiscountRuleResponse?> GetRuleByIdAsync(Guid id)
    {
        var rule = await _context.DiscountRules
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rule is null) return null;

        return _mapper.Map<DiscountRuleResponse>(rule);
    }

    public async Task<IEnumerable<DiscountRuleResponse>> GetAllRulesAsync()
    {
        var rules = await _context.DiscountRules
            .OrderByDescending(r => r.Priority)
            .AsNoTracking()
            .ToListAsync();

        return rules.Select(_mapper.Map<DiscountRuleResponse>);
    }

    public async Task<IEnumerable<DiscountRuleResponse>> GetActiveRulesAsync()
    {
        var now = DateTime.UtcNow;
        var rules = await _context.DiscountRules
            .Where(r => r.IsActive 
                && (!r.ValidFrom.HasValue || r.ValidFrom <= now)
                && (!r.ValidTo.HasValue || r.ValidTo >= now))
            .OrderByDescending(r => r.Priority)
            .AsNoTracking()
            .ToListAsync();

        return rules.Select(_mapper.Map<DiscountRuleResponse>);
    }

    public async Task<bool> UpdateRuleAsync(Guid id, DiscountRuleUpdateRequest request)
    {
        var rule = await _context.DiscountRules.FindAsync(id);
        if (rule is null) return false;

        if (request.Name != null) rule.Name = request.Name;
        if (request.Description != null) rule.Description = request.Description;
        if (request.Value.HasValue) rule.Value = request.Value.Value;
        if (request.ValidFrom.HasValue) rule.ValidFrom = request.ValidFrom;
        if (request.ValidTo.HasValue) rule.ValidTo = request.ValidTo;
        if (request.IsActive.HasValue) rule.IsActive = request.IsActive.Value;
        if (request.Priority.HasValue) rule.Priority = request.Priority.Value;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRuleAsync(Guid id)
    {
        var rule = await _context.DiscountRules.FindAsync(id);
        if (rule is null) return false;

        _context.DiscountRules.Remove(rule);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DiscountCalculationResponse> CalculateDiscountAsync(Guid invoiceId, List<Guid>? discountRuleIds = null)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Visit)
                .ThenInclude(v => v.TestOrders)
                    .ThenInclude(to => to.LabTest)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice is null)
            return new DiscountCalculationResponse(0, 0, 0, new List<AppliedDiscountInfo>());

        var originalAmount = invoice.TotalAmount;
        var applicableRules = await GetApplicableRulesAsync(invoice, discountRuleIds);

        var appliedDiscounts = new List<AppliedDiscountInfo>();
        decimal totalDiscount = 0;

        foreach (var rule in applicableRules.OrderByDescending(r => r.Priority))
        {
            var discountAmount = CalculateRuleDiscount(rule, originalAmount - totalDiscount);
            
            if (discountAmount > 0)
            {
                totalDiscount += discountAmount;
                appliedDiscounts.Add(new AppliedDiscountInfo(
                    rule.Id,
                    rule.Name,
                    discountAmount
                ));
            }
        }

        return new DiscountCalculationResponse(
            originalAmount,
            totalDiscount,
            originalAmount - totalDiscount,
            appliedDiscounts
        );
    }

    public async Task<bool> ApplyDiscountToInvoiceAsync(Guid invoiceId, List<Guid> discountRuleIds)
    {
        var calculation = await CalculateDiscountAsync(invoiceId, discountRuleIds);
        
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice is null) return false;

        invoice.Discount = calculation.DiscountAmount;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<List<DiscountRule>> GetApplicableRulesAsync(Invoice invoice, List<Guid>? ruleIds)
    {
        var query = _context.DiscountRules.Where(r => r.IsActive);

        if (ruleIds != null && ruleIds.Any())
        {
            query = query.Where(r => ruleIds.Contains(r.Id));
        }

        var now = DateTime.UtcNow;
        query = query.Where(r => 
            (!r.ValidFrom.HasValue || r.ValidFrom <= now) &&
            (!r.ValidTo.HasValue || r.ValidTo >= now));

        var rules = await query.ToListAsync();

        // Filter by applicability
        return rules.Where(r => IsRuleApplicable(r, invoice)).ToList();
    }

    private static bool IsRuleApplicable(DiscountRule rule, Invoice invoice)
    {
        if (rule.MinimumAmount.HasValue && invoice.TotalAmount < rule.MinimumAmount)
            return false;

        if (rule.MinimumTests.HasValue && invoice.Visit.TestOrders.Count < rule.MinimumTests)
            return false;

        return true;
    }

    private static decimal CalculateRuleDiscount(DiscountRule rule, decimal amount)
    {
        return rule.Type switch
        {
            DiscountType.Percentage => amount * (rule.Value / 100),
            DiscountType.FixedAmount => Math.Min(rule.Value, amount),
            _ => 0
        };
    }

    
}
