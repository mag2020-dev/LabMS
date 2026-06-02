namespace LabMS.Contracts.Visit;



public record VisitCreateRequest(
    string PatientName,
    string? DoctorName,
    DateTime VisitDate,
    string? Reason
);