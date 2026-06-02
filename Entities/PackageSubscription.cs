namespace LabMS.Entities;

public class PackageSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public Guid TestPackageId { get; set; }
    public TestPackage TestPackage { get; set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public SubscriptionFrequency Frequency { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    public decimal MonthlyPrice { get; set; }
    public int TestsRemaining { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
}

public enum SubscriptionFrequency
{
    Weekly,
    BiWeekly,
    Monthly,
    Quarterly,
    Annually
}

public enum SubscriptionStatus
{
    Active,
    Paused,
    Cancelled,
    Expired
}
