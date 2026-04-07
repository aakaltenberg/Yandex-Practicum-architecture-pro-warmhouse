using TelemetryService.Models;
using TelemetryService.Repositories;

namespace TelemetryService.Services;

public class TelemetryBlService : ITelemetryBlService
{
    private readonly ITelemetryRepository _repository;

    public TelemetryBlService(ITelemetryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Telemetry>> GetTelemetryAsync(string deviceId, DateTime? from, DateTime? to, int limit = 100)
    {
        return await _repository.GetTelemetryAsync(deviceId, from, to, limit);
    }
}