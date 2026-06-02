namespace LabMS.Contracts.Report;

public record ReportCreateRequest(
    Guid VisitId,
    string FilePath,
    Guid? GeneratedById
);