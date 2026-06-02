namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager")]
public class PurchaseOrdersController(IPurchaseOrderService purchaseOrderService) : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService = purchaseOrderService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseOrderCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var po = await _purchaseOrderService.CreateAsync(request, userId);
        return po is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = po.Id }, po);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var po = await _purchaseOrderService.GetByIdAsync(id);
        return po is null ? NotFound() : Ok(po);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _purchaseOrderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetBySupplier(Guid supplierId)
    {
        var orders = await _purchaseOrderService.GetBySupplierAsync(supplierId);
        return Ok(orders);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(PurchaseOrderStatus status)
    {
        var orders = await _purchaseOrderService.GetByStatusAsync(status);
        return Ok(orders);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(Guid id)
    {
        var result = await _purchaseOrderService.SubmitAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _purchaseOrderService.ApproveAsync(id, userId);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _purchaseOrderService.CancelAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/receive")]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> ReceiveItem(Guid id, [FromBody] PurchaseOrderReceiveRequest request)
    {
        var result = await _purchaseOrderService.ReceiveItemAsync(id, request);
        return result ? NoContent() : NotFound();
    }
}
