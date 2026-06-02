namespace LabMS.Contracts.Sample;

public record SampleCreateRequest(
    string SampleCode,
    Guid TestOrderId,
    SampleType Type,
    Guid CollectedByUserId,
    string? StorageLocation,
    string? Container,
    decimal? Volume,
    string? VolumeUnit,
    string? Notes
);

public record SampleUpdateRequest(
    SampleStatus? Status,
    string? StorageLocation,
    string? Container,
    decimal? Volume,
    string? VolumeUnit,
    string? Notes
);

public record SampleReceiveRequest(
    Guid ReceivedByUserId,
    string? StorageLocation
);

public record SampleRejectRequest(
    string RejectionReason,
    Guid RejectedByUserId
);

public record SampleTrackingCreateRequest(
    Guid SampleId,
    SampleStatus Status,
    Guid? UserId,
    string? Location,
    string? Notes
);
