namespace LabMS.Entities;

public class TestPackageItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TestPackageId { get; set; }
    public TestPackage TestPackage { get; set; } = default!;

    public Guid LabTestId { get; set; }
    public LabTest LabTest { get; set; } = default!;

    public int Quantity { get; set; } = 1;
    public bool IsOptional { get; set; } = false;
}
