namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestPackagesController(ITestPackageService testPackageService) : ControllerBase
{
    private readonly ITestPackageService _testPackageService = testPackageService;

    [HttpPost]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Create([FromBody] TestPackageCreateRequest request)
    {
        var package = await _testPackageService.CreateAsync(request);
        return package is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = package.Id }, package);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var package = await _testPackageService.GetByIdAsync(id);
        return package is null ? NotFound() : Ok(package);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var packages = await _testPackageService.GetAllAsync();
        return Ok(packages);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive()
    {
        var packages = await _testPackageService.GetActiveAsync();
        return Ok(packages);
    }

    [HttpGet("type/{type}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByType(PackageType type)
    {
        var packages = await _testPackageService.GetByTypeAsync(type);
        return Ok(packages);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TestPackageUpdateRequest request)
    {
        var result = await _testPackageService.UpdateAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _testPackageService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _testPackageService.ActivateAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _testPackageService.DeactivateAsync(id);
        return result ? NoContent() : NotFound();
    }
}
