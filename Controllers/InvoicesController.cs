

namespace LabMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController(IInvoiceService invoiceService, IMapper mapper) : ControllerBase
{
    private readonly IInvoiceService _invoiceService = invoiceService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist")]
    public async Task<IActionResult> GetAll()
        => Ok(await _invoiceService.GetAllAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _invoiceService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("generate/{visitId:guid}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Add(Guid visitId, [FromQuery] decimal discount = 0)
    {
        var created = await _invoiceService.AddInvoiceForVisitAsync(visitId, discount);
        if (created is null) return BadRequest("Invalid VisitId or no test orders found.");
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InvoiceUpdateRequest request)
    {
        var updated = await _invoiceService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _invoiceService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
