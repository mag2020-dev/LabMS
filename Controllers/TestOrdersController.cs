using LabMS.Contracts.TestOrder;
using LabMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestOrdersController(ITestOrderService testOrderService) : ControllerBase
{
    private readonly ITestOrderService _testOrderService = testOrderService;

    [HttpGet]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> GetAll()
        => Ok(await _testOrderService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LabManager,Receptionist,Technician")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _testOrderService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Add([FromBody] TestOrderCreateRequest request)
    {
        var created = await _testOrderService.AddAsync(request);
        if (created is null) return BadRequest("Lab test not found");

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TestOrderUpdateRequest request)
    {
        var updated = await _testOrderService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string newStatus)
    {
        try
        {
            var updated = await _testOrderService.UpdateStatusAsync(id, newStatus);
            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _testOrderService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
