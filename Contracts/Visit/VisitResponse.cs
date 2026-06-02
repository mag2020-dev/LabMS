namespace LabMS.Contracts.Visit;


public record VisitResponse(
    Guid Id,
    string PatientName,
    string? DoctorName,
    DateTime VisitDate,
    string? Reason,
    VisitStatus Status,
    string? Notes
);