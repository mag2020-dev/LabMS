

namespace LabMS.Services;

public class RoleService(LabMSDbContext context, IMapper mapper) : IRoleService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync()
    {
        var roles = await _context.Roles.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<RoleResponse>>(roles);
    }

    public async Task<RoleResponse?> GetByIdAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        return role is null ? null : _mapper.Map<RoleResponse>(role);
    }

    public async Task<RoleResponse> AddAsync(RoleCreateRequest request)
    {
        var role = _mapper.Map<Role>(request);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return _mapper.Map<RoleResponse>(role);
    }
    public async Task<bool> UpdateAsync(RoleUpdateRequest request)
    {
        var role = await _context.Roles.FindAsync(request.Id);
        if (role is null) return false;

        role.Name = request.Name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role is null) return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }
}
