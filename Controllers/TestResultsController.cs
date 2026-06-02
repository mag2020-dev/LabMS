namespace LabMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestResultsController(ITestResultService testResultService, IMapper mapper) : ControllerBase
{
    private readonly ITestResultService _testResultService = testResultService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _testResultService.GetAllAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _testResultService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-order/{orderId:guid}")]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> GetByTestOrder(Guid orderId)
    {
        var results = await _testResultService.GetByTestOrderIdAsync(orderId);
        return !results.Any() ? NotFound() : Ok(results);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Add([FromBody] TestResultCreateRequest request)
    {
        var created = await _testResultService.AddAsync(request);
        if (created is null) return BadRequest("Invalid TestOrderId or related LabTest not found.");

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TestResultUpdateRequest request)
    {
        var updated = await _testResultService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/verify")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] TestResultVerifyRequest request)
    {
        var verified = await _testResultService.VerifyAsync(id, request);
        return verified ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _testResultService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
