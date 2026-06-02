namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Receptionist")]
public class PaymentPlansController(IPaymentPlanService paymentPlanService) : ControllerBase
{
    private readonly IPaymentPlanService _paymentPlanService = paymentPlanService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentPlanCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var plan = await _paymentPlanService.CreateAsync(request, userId);
        return plan is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await _paymentPlanService.GetByIdAsync(id);
        return plan is null ? NotFound() : Ok(plan);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _paymentPlanService.GetAllAsync();
        return Ok(plans);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var plans = await _paymentPlanService.GetByPatientIdAsync(patientId);
        return Ok(plans);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetActive()
    {
        var plans = await _paymentPlanService.GetActiveAsync();
        return Ok(plans);
    }

    [HttpPost("{id}/process-payment")]
    public async Task<IActionResult> ProcessPayment(Guid id, [FromBody] PaymentPlanPaymentRequest request)
    {
        var result = await _paymentPlanService.ProcessPaymentAsync(id, request);
        return result ? NoContent() : BadRequest(new { message = "Payment failed or installment not found" });
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _paymentPlanService.CancelAsync(id);
        return result ? NoContent() : NotFound();
    }
}
