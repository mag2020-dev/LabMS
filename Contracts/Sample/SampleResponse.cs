namespace LabMS.Contracts.Sample;

public record SampleResponse(
    Guid Id,
    string SampleCode,
    Guid TestOrderId,
    string TestName,
    string PatientName,
    SampleType Type,
    SampleStatus Status,
    DateTime CollectedAt,
    string? CollectedByName,
    DateTime? ReceivedAt,
    string? ReceivedByName,
    string? StorageLocation,
    string? Container,
    decimal? Volume,
    string? VolumeUnit,
    DateTime? ProcessedAt,
    DateTime? DisposedAt,
    string? RejectionReason,
    DateTime? RejectedAt,
    string? Notes
);

public record SampleTrackingResponse(
    Guid Id,
    Guid SampleId,
    SampleStatus Status,
    DateTime Timestamp,
    string? UserName,
    string? Location,
    string? Notes
);

public record SampleStatisticsResponse(
    int TotalSamples,
    int CollectedSamples,
    int InTransitSamples,
    int ReceivedSamples,
    int InTestingSamples,
    int TestedSamples,
    int RejectedSamples,
    Dictionary<SampleType, int> SamplesByType,
    Dictionary<SampleStatus, int> SamplesByStatus
);
