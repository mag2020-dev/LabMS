namespace LabMS.Contracts.Billing;

public record TaxConfigurationCreateRequest(
    string Name,
    string? Code,
    decimal Rate,
    TaxType Type,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string? Description
);

public record TaxConfigurationUpdateRequest(
    string? Name,
    decimal? Rate,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    bool? IsActive,
    string? Description
);

public record TaxConfigurationResponse(
    Guid Id,
    string Name,
    string? Code,
    decimal Rate,
    TaxType Type,
    bool IsActive,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string? Description
);

public record TaxCalculationRequest(
    decimal Amount,
    List<Guid>? TaxConfigurationIds
);

public record TaxCalculationResponse(
    decimal SubTotal,
    decimal TotalTax,
    decimal GrandTotal,
    List<AppliedTaxInfo> AppliedTaxes
);

public record AppliedTaxInfo(
    Guid TaxConfigurationId,
    string TaxName,
    decimal Rate,
    decimal TaxAmount
);
