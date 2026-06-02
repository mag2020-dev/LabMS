using LabMS.Contracts.User;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<UserResponse> RegisterAsync(UserRegisterRequest request);
    Task<bool> AssignRoleAsync(AssignRoleRequest request);
    Task<bool> UpdateAsync(UserUpdateRequest request);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);

}