using Microsoft.AspNetCore.Mvc;
using TelemetryService.Services;

namespace TelemetryService.Controllers;

[ApiController]
[Route("api/v1/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetryBlService _telemetryService;

    public TelemetryController(ITelemetryBlService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [HttpGet("{deviceId}")]
    public async Task<IActionResult> GetTelemetry(
        string deviceId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 100)
    {
        var data = await _telemetryService.GetTelemetryAsync(deviceId, from, to, limit);
        return Ok(data);
    }
}