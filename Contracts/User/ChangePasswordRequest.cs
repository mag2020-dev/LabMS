namespace LabMS.Contracts.User;
public record ChangePasswordRequest(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
);
