namespace LabMS.Services;

public class AdminBootstrapService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<AdminBootstrapService> logger) : IHostedService
{
    private static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var username = configuration["AdminBootstrap:Username"];
        var fullName = configuration["AdminBootstrap:FullName"] ?? "System Administrator";
        var email = configuration["AdminBootstrap:Email"];
        var password = configuration["AdminBootstrap:Password"];

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Admin bootstrap is enabled but Username, Email, or Password is missing.");
            return;
        }

        if (password.Length < 12)
        {
            logger.LogWarning("Admin bootstrap password must be at least 12 characters long.");
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LabMSDbContext>();

        var userExists = await dbContext.Users
            .AnyAsync(user => user.Username == username || user.Email == email, cancellationToken);

        if (userExists)
        {
            logger.LogInformation("Admin bootstrap skipped because the configured user already exists.");
            return;
        }

        var adminRole = await dbContext.Roles
            .FirstOrDefaultAsync(role => role.Name == "Admin", cancellationToken);

        if (adminRole is null)
        {
            adminRole = new Role { Id = AdminRoleId, Name = "Admin" };
            dbContext.Roles.Add(adminRole);
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            FullName = fullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            UserRoles =
            [
                new UserRole
                {
                    Role = adminRole
                }
            ]
        };

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Admin bootstrap created the configured admin user.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
