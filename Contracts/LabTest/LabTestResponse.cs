namespace LabMS.Contracts.LabTest;

public record LabTestResponse(
    Guid Id,
    string Name,
    string? Code,
    string? Description,
    decimal Price,
    int? EstimatedDuration
);
