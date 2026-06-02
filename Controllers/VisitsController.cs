
namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController(IVisitService visitService) : ControllerBase
{
    private readonly IVisitService _visitService = visitService;

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _visitService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _visitService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Add(VisitCreateRequest request)
    {
        var created = await _visitService.AddAsync(request);
        if (created is null)
        {
            return BadRequest("Could not create visit.");
        }
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Update(Guid id, VisitUpdateRequest request)
    {
        var updated = await _visitService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _visitService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
