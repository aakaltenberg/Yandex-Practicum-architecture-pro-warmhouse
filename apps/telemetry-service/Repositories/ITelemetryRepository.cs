using TelemetryService.Models;

namespace TelemetryService.Repositories;

public interface ITelemetryRepository
{
    Task SaveTelemetryAsync(Telemetry telemetry);
    Task<List<Telemetry>> GetTelemetryAsync(string deviceId, DateTime? from, DateTime? to, int limit);
}