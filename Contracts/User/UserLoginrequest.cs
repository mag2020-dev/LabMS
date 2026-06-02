using Microsoft.AspNetCore.Server.HttpSys;

namespace LabMS.Contracts.User;

public record UserLoginRequest (
    String Username,
    string Password
    );