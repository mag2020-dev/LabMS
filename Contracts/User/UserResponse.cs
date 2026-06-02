namespace LabMS.Contracts.User;
public record UserResponse(
    Guid Id, 
    string Username,
    string FullName,
    string Email,
    IEnumerable<string> Roles
    );