namespace LabMS.Contracts.Doctor;

public record DoctorCreateRequest(
    string FirstName,
    string LastName,
    string? Specialty,
    string? PhoneNumber
);

