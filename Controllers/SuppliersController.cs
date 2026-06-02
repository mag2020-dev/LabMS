namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager")]
public class SuppliersController(ISupplierService supplierService) : ControllerBase
{
    private readonly ISupplierService _supplierService = supplierService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SupplierCreateRequest request)
    {
        var supplier = await _supplierService.CreateAsync(request);
        return supplier is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var suppliers = await _supplierService.GetActiveAsync();
        return Ok(suppliers);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SupplierUpdateRequest request)
    {
        var result = await _supplierService.UpdateAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _supplierService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
