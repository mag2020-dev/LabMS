namespace LabMS.Contracts.Doctor;


public record DoctorUpdateRequest(
    Guid Id,
   
    string FirstName,
    string LastName,
    string? Specialty,
    string? PhoneNumber
);