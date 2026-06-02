
namespace LabMS.Contracts.Visit;

public record VisitUpdateRequest(
    string? DoctorName,
    DateTime VisitDate,
    string? Reason,
    VisitStatus Status,
    string? Notes
);
