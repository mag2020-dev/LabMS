namespace LabMS.Contracts.Authentication;

public record AuthResponse(
    Guid UserId,
    string Username,
    string FullName,
    string Email,
    List<string> Roles,
    string Token
);