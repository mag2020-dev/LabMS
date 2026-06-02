namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager")]
public class TaxesController(ITaxService taxService) : ControllerBase
{
    private readonly ITaxService _taxService = taxService;

    [HttpPost("configurations")]
    public async Task<IActionResult> CreateConfiguration([FromBody] TaxConfigurationCreateRequest request)
    {
        var config = await _taxService.CreateConfigurationAsync(request);
        return config is null ? BadRequest() : CreatedAtAction(nameof(GetConfigurationById), new { id = config.Id }, config);
    }

    [HttpGet("configurations/{id}")]
    public async Task<IActionResult> GetConfigurationById(Guid id)
    {
        var config = await _taxService.GetConfigurationByIdAsync(id);
        return config is null ? NotFound() : Ok(config);
    }

    [HttpGet("configurations")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllConfigurations()
    {
        var configs = await _taxService.GetAllConfigurationsAsync();
        return Ok(configs);
    }

    [HttpGet("configurations/active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveConfigurations()
    {
        var configs = await _taxService.GetActiveConfigurationsAsync();
        return Ok(configs);
    }

    [HttpPut("configurations/{id}")]
    public async Task<IActionResult> UpdateConfiguration(Guid id, [FromBody] TaxConfigurationUpdateRequest request)
    {
        var result = await _taxService.UpdateConfigurationAsync(id, request);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("configurations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfiguration(Guid id)
    {
        var result = await _taxService.DeleteConfigurationAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("calculate")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateTax([FromBody] TaxCalculationRequest request)
    {
        var result = await _taxService.CalculateTaxAsync(request.Amount, request.TaxConfigurationIds);
        return Ok(result);
    }
}
