
namespace LabMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController(IReportService reportService, IMapper mapper) : ControllerBase
{
    private readonly IReportService _reportService = reportService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _reportService.GetAllAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _reportService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Add([FromBody] ReportCreateRequest request)
    {
        var created = await _reportService.AddAsync(request);
        if (created is null) return BadRequest("Invalid VisitId");

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ReportUpdateRequest request)
    {
        var updated = await _reportService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _reportService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
