using Microsoft.AspNetCore.Mvc;
using GateService.Services;

namespace GateService.Controllers;

[ApiController]
[Route("api/v1/gates")]
public class GatesController : ControllerBase
{
    private readonly IGateService _gateService;

    public GatesController(IGateService gateService)
    {
        _gateService = gateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _gateService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var gate = await _gateService.GetByIdAsync(id);
        if (gate == null) return NotFound();
        return Ok(gate);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GateCreateDto dto)
    {
        var gate = await _gateService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = gate.Id }, gate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, GateUpdateDto dto)
    {
        var updated = await _gateService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _gateService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/open")]
    public async Task<IActionResult> Open(int id)
    {
        var success = await _gateService.OpenGateAsync(id);
        if (!success) return NotFound();
        return Ok(new { message = "Gate opened" });
    }

    [HttpPost("{id}/close")]
    public async Task<IActionResult> Close(int id)
    {
        var success = await _gateService.CloseGateAsync(id);
        if (!success) return NotFound();
        return Ok(new { message = "Gate closed" });
    }
}