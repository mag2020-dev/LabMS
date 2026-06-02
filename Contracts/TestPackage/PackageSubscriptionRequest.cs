namespace LabMS.Contracts.TestPackage;

public record PackageSubscriptionCreateRequest(
    Guid PatientId,
    Guid TestPackageId,
    DateTime StartDate,
    DateTime EndDate,
    SubscriptionFrequency Frequency,
    int TestsIncluded
);

public record PackageSubscriptionUpdateRequest(
    DateTime? EndDate,
    SubscriptionStatus? Status,
    int? TestsRemaining
);
