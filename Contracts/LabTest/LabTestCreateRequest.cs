namespace LabMS.Contracts.LabTest;

public record LabTestCreateRequest(
    string Name,
    string? Code,
    string? Description,
    decimal Price,
    int? EstimatedDuration
);