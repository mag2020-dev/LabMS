namespace LabMS.Entities;

public class DiscountRule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? Description { get; set; }

    public DiscountType Type { get; set; }
    public decimal Value { get; set; } // Percentage or fixed amount

    public DiscountApplicability Applicability { get; set; }
    public Guid? SpecificTestId { get; set; }
    public Guid? SpecificPackageId { get; set; }

    public int? MinimumTests { get; set; }
    public decimal? MinimumAmount { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0; // Higher priority applied first

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum DiscountType
{
    Percentage,
    FixedAmount
}

public enum DiscountApplicability
{
    AllTests,
    SpecificTest,
    SpecificPackage,
    BulkDiscount,
    SeniorCitizen,
    Corporate,
    Seasonal
}
