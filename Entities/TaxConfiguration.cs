namespace LabMS.Entities;

public class TaxConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!; // VAT, GST, Sales Tax
    public string? Code { get; set; }
    public decimal Rate { get; set; } // Percentage

    public TaxType Type { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public string? Description { get; set; }
}

public enum TaxType
{
    VAT,
    GST,
    SalesTax,
    ServiceTax
}
