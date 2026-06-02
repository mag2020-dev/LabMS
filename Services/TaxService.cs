namespace LabMS.Services;

public class TaxService(LabMSDbContext context, IMapper mapper) : ITaxService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<TaxConfigurationResponse?> CreateConfigurationAsync(TaxConfigurationCreateRequest request)
    {
        var config = new TaxConfiguration
        {
            Name = request.Name,
            Code = request.Code,
            Rate = request.Rate,
            Type = request.Type,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            Description = request.Description,
            IsActive = true
        };

        _context.TaxConfigurations.Add(config);
        await _context.SaveChangesAsync();

        return await GetConfigurationByIdAsync(config.Id);
    }

    public async Task<TaxConfigurationResponse?> GetConfigurationByIdAsync(Guid id)
    {
        var config = await _context.TaxConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (config is null) return null;

        return _mapper.Map<TaxConfigurationResponse>(config);
    }

    public async Task<IEnumerable<TaxConfigurationResponse>> GetAllConfigurationsAsync()
    {
        var configs = await _context.TaxConfigurations
            .OrderBy(t => t.Name)
            .AsNoTracking()
            .ToListAsync();

        return configs.Select(_mapper.Map<TaxConfigurationResponse>);
    }

    public async Task<IEnumerable<TaxConfigurationResponse>> GetActiveConfigurationsAsync()
    {
        var now = DateTime.UtcNow;
        var configs = await _context.TaxConfigurations
            .Where(t => t.IsActive 
                && t.EffectiveFrom <= now
                && (!t.EffectiveTo.HasValue || t.EffectiveTo >= now))
            .OrderBy(t => t.Name)
            .AsNoTracking()
            .ToListAsync();

        return configs.Select(_mapper.Map<TaxConfigurationResponse>);
    }

    public async Task<bool> UpdateConfigurationAsync(Guid id, TaxConfigurationUpdateRequest request)
    {
        var config = await _context.TaxConfigurations.FindAsync(id);
        if (config is null) return false;

        if (request.Name != null) config.Name = request.Name;
        if (request.Rate.HasValue) config.Rate = request.Rate.Value;
        if (request.EffectiveFrom.HasValue) config.EffectiveFrom = request.EffectiveFrom.Value;
        if (request.EffectiveTo.HasValue) config.EffectiveTo = request.EffectiveTo;
        if (request.IsActive.HasValue) config.IsActive = request.IsActive.Value;
        if (request.Description != null) config.Description = request.Description;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteConfigurationAsync(Guid id)
    {
        var config = await _context.TaxConfigurations.FindAsync(id);
        if (config is null) return false;

        _context.TaxConfigurations.Remove(config);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TaxCalculationResponse> CalculateTaxAsync(decimal amount, List<Guid>? taxConfigurationIds = null)
    {
        IEnumerable<TaxConfiguration> taxes;

        if (taxConfigurationIds != null && taxConfigurationIds.Any())
        {
            taxes = await _context.TaxConfigurations
                .Where(t => taxConfigurationIds.Contains(t.Id) && t.IsActive)
                .ToListAsync();
        }
        else
        {
            var now = DateTime.UtcNow;
            taxes = await _context.TaxConfigurations
                .Where(t => t.IsActive 
                    && t.EffectiveFrom <= now
                    && (!t.EffectiveTo.HasValue || t.EffectiveTo >= now))
                .ToListAsync();
        }

        var appliedTaxes = new List<AppliedTaxInfo>();
        decimal totalTax = 0;

        foreach (var tax in taxes)
        {
            var taxAmount = amount * (tax.Rate / 100);
            totalTax += taxAmount;

            appliedTaxes.Add(new AppliedTaxInfo(
                tax.Id,
                tax.Name,
                tax.Rate,
                taxAmount
            ));
        }

        return new TaxCalculationResponse(
            amount,
            totalTax,
            amount + totalTax,
            appliedTaxes
        );
    }

    
}
