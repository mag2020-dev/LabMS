namespace LabMS.Contracts.Authentication;

public record RegisterRequest(
    string Username,
    string FullName,
    string Email,
    string Password,
    List<string> Roles 
);
