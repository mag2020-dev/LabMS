namespace LabMS.Contracts.Billing;

public record DiscountRuleCreateRequest(
    string Name,
    string? Code,
    string? Description,
    DiscountType Type,
    decimal Value,
    DiscountApplicability Applicability,
    Guid? SpecificTestId,
    Guid? SpecificPackageId,
    int? MinimumTests,
    decimal? MinimumAmount,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    int Priority
);

public record DiscountRuleUpdateRequest(
    string? Name,
    string? Description,
    decimal? Value,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool? IsActive,
    int? Priority
);

public record DiscountRuleResponse(
    Guid Id,
    string Name,
    string? Code,
    string? Description,
    DiscountType Type,
    decimal Value,
    DiscountApplicability Applicability,
    Guid? SpecificTestId,
    Guid? SpecificPackageId,
    int? MinimumTests,
    decimal? MinimumAmount,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool IsActive,
    int Priority,
    DateTime CreatedAt
);

public record ApplyDiscountRequest(
    Guid InvoiceId,
    List<Guid> DiscountRuleIds
);

public record DiscountCalculationResponse(
    decimal OriginalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    List<AppliedDiscountInfo> AppliedDiscounts
);

public record AppliedDiscountInfo(
    Guid DiscountRuleId,
    string DiscountName,
    decimal DiscountAmount
);
