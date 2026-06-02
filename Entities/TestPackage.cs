namespace LabMS.Entities;

public class TestPackage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? Description { get; set; }

    public decimal RegularPrice { get; set; }
    public decimal PackagePrice { get; set; }
    public decimal DiscountPercentage { get; set; }

    public bool IsActive { get; set; } = true;
    public PackageType Type { get; set; } = PackageType.Standard;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<TestPackageItem> TestPackageItems { get; set; } = [];
}

public enum PackageType
{
    Standard,
    Premium,
    Corporate,
    Wellness,
    Seasonal,
    Custom
}
