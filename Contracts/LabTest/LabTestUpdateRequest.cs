namespace LabMS.Contracts.LabTest;

public record LabTestUpdateRequest(
    string Name,
    string? Code,
    string? Description,
    decimal Price,
    int? EstimatedDuration
);
