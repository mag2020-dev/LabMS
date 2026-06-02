namespace LabMS.Contracts.User;

public record UserRegisterRequest(
    string Username,
    string FullName,
    string Email,
    string Password
    );
