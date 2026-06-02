namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager,Receptionist")]
public class InsuranceClaimsController(IInsuranceClaimService claimService) : ControllerBase
{
    private readonly IInsuranceClaimService _claimService = claimService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InsuranceClaimCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var claim = await _claimService.CreateAsync(request, userId);
        return claim is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = claim.Id }, claim);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var claim = await _claimService.GetByIdAsync(id);
        return claim is null ? NotFound() : Ok(claim);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var claims = await _claimService.GetAllAsync();
        return Ok(claims);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var claims = await _claimService.GetByPatientIdAsync(patientId);
        return Ok(claims);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(ClaimStatus status)
    {
        var claims = await _claimService.GetByStatusAsync(status);
        return Ok(claims);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InsuranceClaimUpdateRequest request)
    {
        var result = await _claimService.UpdateAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(Guid id)
    {
        var result = await _claimService.SubmitAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] decimal approvedAmount)
    {
        var result = await _claimService.ApproveAsync(id, approvedAmount);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
    {
        var result = await _claimService.RejectAsync(id, reason);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/paid")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> MarkAsPaid(Guid id)
    {
        var result = await _claimService.MarkAsPaidAsync(id);
        return result ? NoContent() : NotFound();
    }
}
