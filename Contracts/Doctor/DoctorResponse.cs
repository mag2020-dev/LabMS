namespace LabMS.Contracts.Doctor;
public record DoctorResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,   
    string? Specialty,
    string? PhoneNumber
);