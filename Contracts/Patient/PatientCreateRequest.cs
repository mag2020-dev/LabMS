namespace LabMS.Contracts.Patient;
public record PatientCreateRequest(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string Phone,
    string? Email,
    string Address
);


