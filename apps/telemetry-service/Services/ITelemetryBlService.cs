using TelemetryService.Models;

namespace TelemetryService.Services;

public interface ITelemetryBlService
{
    Task<List<Telemetry>> GetTelemetryAsync(string deviceId, DateTime? from, DateTime? to, int limit = 100);
}