namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    private readonly IInventoryService _inventoryService = inventoryService;

    [HttpPost]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Create([FromBody] InventoryItemCreateRequest request)
    {
        var item = await _inventoryService.CreateItemAsync(request);
        return item is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _inventoryService.GetItemByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _inventoryService.GetAllItemsAsync();
        return Ok(items);
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetLowStock()
    {
        var items = await _inventoryService.GetLowStockItemsAsync();
        return Ok(items);
    }

    [HttpGet("expiring")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetExpiring([FromQuery] int daysThreshold = 30)
    {
        var items = await _inventoryService.GetExpiringItemsAsync(daysThreshold);
        return Ok(items);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(InventoryCategory category)
    {
        var items = await _inventoryService.GetByCategoryAsync(category);
        return Ok(items);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InventoryItemUpdateRequest request)
    {
        var result = await _inventoryService.UpdateItemAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _inventoryService.DeleteItemAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/adjust-stock")]
    [Authorize(Roles = "Admin,LabManager,Technician")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] int quantity, [FromQuery] string? notes = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _inventoryService.AdjustStockAsync(id, quantity, userId, notes);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id}/transactions")]
    public async Task<IActionResult> GetTransactionHistory(Guid id)
    {
        var transactions = await _inventoryService.GetTransactionHistoryAsync(id);
        return Ok(transactions);
    }

    [HttpGet("alerts")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetActiveAlerts()
    {
        var alerts = await _inventoryService.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    [HttpPost("alerts/{alertId}/resolve")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> ResolveAlert(Guid alertId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _inventoryService.ResolveAlertAsync(alertId, userId);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _inventoryService.GetStatisticsAsync();
        return Ok(stats);
    }
}
