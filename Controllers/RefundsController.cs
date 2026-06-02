namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager")]
public class RefundsController(IRefundService refundService) : ControllerBase
{
    private readonly IRefundService _refundService = refundService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RefundCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var refund = await _refundService.CreateAsync(request, userId);
        return refund is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = refund.Id }, refund);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var refund = await _refundService.GetByIdAsync(id);
        return refund is null ? NotFound() : Ok(refund);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var refunds = await _refundService.GetAllAsync();
        return Ok(refunds);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(RefundStatus status)
    {
        var refunds = await _refundService.GetByStatusAsync(status);
        return Ok(refunds);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] string? notes = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _refundService.ApproveAsync(id, userId, notes);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string? notes = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _refundService.RejectAsync(id, userId, notes);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/process")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Process(Guid id)
    {
        var result = await _refundService.ProcessAsync(id);
        return result ? NoContent() : NotFound();
    }
}
