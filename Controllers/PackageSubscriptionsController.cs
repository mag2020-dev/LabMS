namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PackageSubscriptionsController(IPackageSubscriptionService subscriptionService) : ControllerBase
{
    private readonly IPackageSubscriptionService _subscriptionService = subscriptionService;

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist,LabManager")]
    public async Task<IActionResult> Create([FromBody] PackageSubscriptionCreateRequest request)
    {
        var subscription = await _subscriptionService.CreateAsync(request);
        return subscription is null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);
        return subscription is null ? NotFound() : Ok(subscription);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetAll()
    {
        var subscriptions = await _subscriptionService.GetAllAsync();
        return Ok(subscriptions);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var subscriptions = await _subscriptionService.GetByPatientIdAsync(patientId);
        return Ok(subscriptions);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,LabManager")]
    public async Task<IActionResult> GetActive()
    {
        var subscriptions = await _subscriptionService.GetActiveSubscriptionsAsync();
        return Ok(subscriptions);
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Receptionist,LabManager")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string reason)
    {
        var result = await _subscriptionService.CancelAsync(id, reason);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/pause")]
    [Authorize(Roles = "Admin,Receptionist,LabManager")]
    public async Task<IActionResult> Pause(Guid id)
    {
        var result = await _subscriptionService.PauseAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/resume")]
    [Authorize(Roles = "Admin,Receptionist,LabManager")]
    public async Task<IActionResult> Resume(Guid id)
    {
        var result = await _subscriptionService.ResumeAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/use-test")]
    [Authorize(Roles = "Admin,Receptionist,Technician")]
    public async Task<IActionResult> UseTest(Guid id)
    {
        var result = await _subscriptionService.UseTestAsync(id);
        return result ? NoContent() : BadRequest(new { message = "No tests remaining or subscription not found" });
    }
}
