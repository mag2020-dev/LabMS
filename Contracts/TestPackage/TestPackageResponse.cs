namespace LabMS.Contracts.TestPackage;

public record TestPackageResponse(
    Guid Id,
    string Name,
    string? Code,
    string? Description,
    decimal RegularPrice,
    decimal PackagePrice,
    decimal DiscountPercentage,
    decimal Savings,
    bool IsActive,
    PackageType Type,
    List<TestPackageItemResponse> Tests,
    DateTime CreatedAt
);

public record TestPackageItemResponse(
    Guid Id,
    Guid LabTestId,
    string TestName,
    decimal TestPrice,
    int Quantity,
    bool IsOptional
);
