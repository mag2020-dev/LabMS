namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SamplesController(ISampleService sampleService) : ControllerBase
{
    private readonly ISampleService _sampleService = sampleService;

    [HttpPost]
    [Authorize(Roles = "Admin,Technician,Receptionist")]
    public async Task<IActionResult> Create([FromBody] SampleCreateRequest request)
    {
        var sample = await _sampleService.CreateAsync(request);
        return sample is null 
            ? Conflict(new { message = "Sample code already exists" }) 
            : CreatedAtAction(nameof(GetById), new { id = sample.Id }, sample);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sample = await _sampleService.GetByIdAsync(id);
        return sample is null ? NotFound() : Ok(sample);
    }

    [HttpGet("code/{sampleCode}")]
    public async Task<IActionResult> GetBySampleCode(string sampleCode)
    {
        var sample = await _sampleService.GetBySampleCodeAsync(sampleCode);
        return sample is null ? NotFound() : Ok(sample);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> GetAll()
    {
        var samples = await _sampleService.GetAllAsync();
        return Ok(samples);
    }

    [HttpGet("test-order/{testOrderId}")]
    public async Task<IActionResult> GetByTestOrder(Guid testOrderId)
    {
        var samples = await _sampleService.GetByTestOrderIdAsync(testOrderId);
        return Ok(samples);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> GetByStatus(SampleStatus status)
    {
        var samples = await _sampleService.GetByStatusAsync(status);
        return Ok(samples);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var samples = await _sampleService.GetByPatientIdAsync(patientId);
        return Ok(samples);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SampleUpdateRequest request)
    {
        var result = await _sampleService.UpdateAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/receive")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] SampleReceiveRequest request)
    {
        var result = await _sampleService.ReceiveAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,Technician,LabManager")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] SampleRejectRequest request)
    {
        var result = await _sampleService.RejectAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/processed")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> MarkAsProcessed(Guid id)
    {
        var result = await _sampleService.MarkAsProcessedAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/disposed")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> MarkAsDisposed(Guid id)
    {
        var result = await _sampleService.MarkAsDisposedAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id}/tracking")]
    public async Task<IActionResult> GetTrackingHistory(Guid id)
    {
        var tracking = await _sampleService.GetTrackingHistoryAsync(id);
        return Ok(tracking);
    }

    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var stats = await _sampleService.GetStatisticsAsync(startDate, endDate);
        return Ok(stats);
    }
}
