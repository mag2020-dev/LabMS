namespace LabMS.Contracts.TestPackage;

public record PackageSubscriptionResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid TestPackageId,
    string PackageName,
    DateTime StartDate,
    DateTime EndDate,
    SubscriptionFrequency Frequency,
    SubscriptionStatus Status,
    decimal MonthlyPrice,
    int TestsRemaining,
    DateTime CreatedAt
);
