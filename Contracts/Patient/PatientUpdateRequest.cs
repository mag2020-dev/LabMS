namespace LabMS.Contracts.Patient;

public record PatientUpdateRequest(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string Phone,
    string? Email,
    string Address
);
