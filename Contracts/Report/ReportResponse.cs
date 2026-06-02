namespace LabMS.Contracts;

public record ReportResponse(
    Guid Id,
    Guid VisitId,
    string FilePath,
    DateTime GeneratedAt,
    string? GeneratedByName
);