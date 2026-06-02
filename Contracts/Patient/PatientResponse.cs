namespace LabMS.Contracts.Patient;

public record PatientResponse(
    Guid PatientId,
    string FirstName,
    string LastName,
    string FullName,
    DateTime DateOfBirth,
    string Gender,
    string Phone,
    string? Email,
    string Address
);
