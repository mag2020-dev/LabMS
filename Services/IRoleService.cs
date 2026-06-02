public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync();
    Task<RoleResponse?> GetByIdAsync(Guid id);
    Task<RoleResponse> AddAsync(RoleCreateRequest request);
    Task<bool> UpdateAsync(RoleUpdateRequest request);

    Task<bool> DeleteAsync(Guid id);
}