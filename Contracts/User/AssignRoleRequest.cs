namespace LabMS.Contracts.User;

public record AssignRoleRequest(
    Guid UserId,
    Guid RoleId
    );
