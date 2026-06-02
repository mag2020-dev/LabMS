
namespace LabMS.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LabTestsController(ILabTestService labTestService, IMapper mapper) : ControllerBase
{
    private readonly ILabTestService _labTestService = labTestService;
    private readonly IMapper _mapper = mapper;
    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _labTestService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _labTestService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Add([FromBody]LabTestCreateRequest request)
    {
        var created = await _labTestService.AddAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Update(Guid id, LabTestUpdateRequest request)
    {
        var updated = await _labTestService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _labTestService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
