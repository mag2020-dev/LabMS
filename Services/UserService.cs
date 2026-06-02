
namespace LabMS.Services;

public class UserService(LabMSDbContext context, IMapper mapper) : IUserService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null ? null : _mapper.Map<UserResponse>(user);
    }

    public async Task<bool> UpdateAsync(UserUpdateRequest request)
    {
        var user = await _context.Users.FindAsync(request.Id);
        if (user is null) return false;

        user.FullName = request.FullName;
        user.Email = request.Email;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
    {
        var user = _mapper.Map<User>(request);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<bool> AssignRoleAsync(AssignRoleRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        var role = await _context.Roles.FindAsync(request.RoleId);

        if (user is null || role is null) return false;

        if (user.UserRoles.Any(ur => ur.RoleId == role.Id)) return true; // already assigned

        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await _context.SaveChangesAsync();

        return true;
    }

    


    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user is null) return false;

        // ✅ Verify current password
        if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return false;

        // ✅ Hash new password
        user.PasswordHash = HashPassword(request.NewPassword);

        await _context.SaveChangesAsync();
        return true;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }

}
