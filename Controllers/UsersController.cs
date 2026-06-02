
namespace LabMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _userService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRegisterRequest request)
    {
        var created = await _userService.RegisterAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(AssignRoleRequest request)
    {
        var assigned = await _userService.AssignRoleAsync(request);
        return assigned ? NoContent() : NotFound();
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UserUpdateRequest request)
    {
        if (id != request.Id) return BadRequest("Id mismatch");

        var updated = await _userService.UpdateAsync(request);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{id}/change-password")]
    [Authorize] 
    public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordRequest request)
    {
        if (id != request.UserId) return BadRequest("Id mismatch");

        var result = await _userService.ChangePasswordAsync(request);
        return result ? NoContent() : BadRequest("Invalid current password or user not found");
    }


}
