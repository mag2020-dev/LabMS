namespace LabMS.Contracts.Authentication;

public record LoginRequest(
    string UsernameOrEmail,
    string Password
);