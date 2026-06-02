

namespace LabMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService, IMapper mapper) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist")]
    public async Task<IActionResult> GetAll()
        => Ok(await _paymentService.GetAllAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _paymentService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-invoice/{invoiceId:guid}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist")]
    public async Task<IActionResult> GetByInvoice(Guid invoiceId)
        => Ok(await _paymentService.GetByInvoiceIdAsync(invoiceId));

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Add([FromBody] PaymentCreateRequest request)
    {
        var created = await _paymentService.AddPaymentAsync(request);
        if (created is null) return BadRequest("Invalid InvoiceId.");
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PaymentUpdateRequest request)
    {
        var updated = await _paymentService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _paymentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
