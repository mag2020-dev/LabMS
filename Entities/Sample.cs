namespace LabMS.Entities;

public class Sample
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SampleCode { get; set; } = default!; // Barcode/unique identifier
    public Guid TestOrderId { get; set; }
    public TestOrder TestOrder { get; set; } = default!;

    public SampleType Type { get; set; }
    public SampleStatus Status { get; set; } = SampleStatus.Collected;

    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
    public Guid? CollectedByUserId { get; set; }
    public User? CollectedBy { get; set; }

    public DateTime? ReceivedAt { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public User? ReceivedBy { get; set; }

    public string? StorageLocation { get; set; }
    public string? Container { get; set; }
    public decimal? Volume { get; set; }
    public string? VolumeUnit { get; set; }

    public DateTime? ProcessedAt { get; set; }
    public DateTime? DisposedAt { get; set; }

    public string? RejectionReason { get; set; }
    public DateTime? RejectedAt { get; set; }

    public string? Notes { get; set; }

    // Navigation
    public ICollection<SampleTracking> TrackingHistory { get; set; } = [];
}

public enum SampleType
{
    Blood,
    Urine,
    Stool,
    Saliva,
    Tissue,
    Swab,
    Other
}

public enum SampleStatus
{
    Collected,
    InTransit,
    Received,
    InStorage,
    InTesting,
    Tested,
    Disposed,
    Rejected
}
