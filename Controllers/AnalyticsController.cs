namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,LabManager")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    private readonly IAnalyticsService _analyticsService = analyticsService;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var metrics = await _analyticsService.GetDashboardMetricsAsync();
        return Ok(metrics);
    }

    [HttpGet("reports/financial")]
    public async Task<IActionResult> GetFinancialReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var report = await _analyticsService.GetFinancialReportAsync(startDate, endDate);
        return Ok(report);
    }

    [HttpGet("reports/operational")]
    public async Task<IActionResult> GetOperationalReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var report = await _analyticsService.GetOperationalReportAsync(startDate, endDate);
        return Ok(report);
    }

    [HttpGet("reports/clinical")]
    public async Task<IActionResult> GetClinicalReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var report = await _analyticsService.GetClinicalReportAsync(startDate, endDate);
        return Ok(report);
    }

    [HttpGet("reports/inventory")]
    public async Task<IActionResult> GetInventoryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var report = await _analyticsService.GetInventoryReportAsync(startDate, endDate);
        return Ok(report);
    }
}
