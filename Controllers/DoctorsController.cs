

namespace LabMS.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DoctorsController(IDoctorService doctorService, IMapper mapper) : ControllerBase
{
    private readonly IDoctorService _doctorService = doctorService;
    private readonly IMapper _mapper = mapper;
    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _doctorService.GetAllAsync());
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _doctorService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Add(DoctorCreateRequest request)
    {
        var created = await _doctorService.AddAsync(request);
        if (created is null)
        {
            return BadRequest("Could not create doctor.");
        }
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Update(Guid id, DoctorUpdateRequest request)
    {
        var updated = await _doctorService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _doctorService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
