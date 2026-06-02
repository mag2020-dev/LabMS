namespace LabMS.Contracts.TestPackage;

public record TestPackageCreateRequest(
    string Name,
    string? Code,
    string? Description,
    decimal PackagePrice,
    PackageType Type,
    List<TestPackageItemRequest> Tests
);

public record TestPackageUpdateRequest(
    string? Name,
    string? Code,
    string? Description,
    decimal? PackagePrice,
    PackageType? Type,
    bool? IsActive,
    List<TestPackageItemRequest>? Tests
);

public record TestPackageItemRequest(
    Guid LabTestId,
    int Quantity,
    bool IsOptional
);
