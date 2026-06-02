namespace LabMS.Contracts.User;

public record UserUpdateRequest(
    Guid Id, 
    string FullName,
    string Email
    );