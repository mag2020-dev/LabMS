namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager")]
public class DiscountsController(IDiscountService discountService) : ControllerBase
{
    private readonly IDiscountService _discountService = discountService;

    [HttpPost("rules")]
    public async Task<IActionResult> CreateRule([FromBody] DiscountRuleCreateRequest request)
    {
        var rule = await _discountService.CreateRuleAsync(request);
        return rule is null ? BadRequest() : CreatedAtAction(nameof(GetRuleById), new { id = rule.Id }, rule);
    }

    [HttpGet("rules/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRuleById(Guid id)
    {
        var rule = await _discountService.GetRuleByIdAsync(id);
        return rule is null ? NotFound() : Ok(rule);
    }

    [HttpGet("rules")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllRules()
    {
        var rules = await _discountService.GetAllRulesAsync();
        return Ok(rules);
    }

    [HttpGet("rules/active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveRules()
    {
        var rules = await _discountService.GetActiveRulesAsync();
        return Ok(rules);
    }

    [HttpPut("rules/{id}")]
    public async Task<IActionResult> UpdateRule(Guid id, [FromBody] DiscountRuleUpdateRequest request)
    {
        var result = await _discountService.UpdateRuleAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("rules/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRule(Guid id)
    {
        var result = await _discountService.DeleteRuleAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("calculate")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> CalculateDiscount([FromBody] ApplyDiscountRequest request)
    {
        var result = await _discountService.CalculateDiscountAsync(request.InvoiceId, request.DiscountRuleIds);
        return Ok(result);
    }

    [HttpPost("apply")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> ApplyDiscount([FromBody] ApplyDiscountRequest request)
    {
        var result = await _discountService.ApplyDiscountToInvoiceAsync(request.InvoiceId, request.DiscountRuleIds);
        return result ? NoContent() : BadRequest(new { message = "Failed to apply discount" });
    }
}
