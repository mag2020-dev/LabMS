namespace LabMS.Services;

public class AuthService(LabMSDbContext context, IConfiguration config) : IAuthService
{
    private readonly LabMSDbContext _context = context;
    private readonly IConfiguration _config = config;

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // check existing user
        if (await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        // attach roles
        foreach (var roleName in request.Roles)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                role = new Role { Id = Guid.NewGuid(), Name = roleName };
                _context.Roles.Add(role);
            }
            user.UserRoles.Add(new UserRole { User = user, Role = role });
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail);

        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return await GenerateAuthResponse(user);
    }

    private Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var jwtKey = _config["Jwt:Key"] ?? _config["JWT_SECRET_KEY"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("A JWT signing key is required. Set Jwt:Key or JWT_SECRET_KEY.");
        }

        if (jwtKey.Length < 32)
        {
            throw new InvalidOperationException("The JWT signing key must be at least 32 characters long.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        var response = new AuthResponse(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            roles,
            new JwtSecurityTokenHandler().WriteToken(token)
        );

        return Task.FromResult(response);
    }
}
