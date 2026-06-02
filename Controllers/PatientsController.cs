
namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    private readonly IPatientService _patientService = patientService;
   

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _patientService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _patientService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Add(PatientCreateRequest request)
    {

        var created = await _patientService.AddAsync(request);
        if (created is null)
        {
            return BadRequest("Could not create patient.");
        }
        return CreatedAtAction(nameof(Get), new { id = created.PatientId }, created);
    }

    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Update(Guid id, PatientUpdateRequest request)
    {
        var updated = await _patientService.UpdateAsync(id, request);

        return updated ? NoContent() : NotFound();
    }

   
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _patientService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
