using Microsoft.EntityFrameworkCore;
using TelemetryService.Data;
using TelemetryService.Models;

namespace TelemetryService.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly AppDbContext _context;

    public TelemetryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveTelemetryAsync(Telemetry telemetry)
    {
        _context.Telemetry.Add(telemetry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Telemetry>> GetTelemetryAsync(string deviceId, DateTime? from, DateTime? to, int limit)
    {
        var query = _context.Telemetry
            .Where(t => t.DeviceId == deviceId);

        if (from.HasValue)
            query = query.Where(t => t.Timestamp >= from.Value);
        if (to.HasValue)
            query = query.Where(t => t.Timestamp <= to.Value);

        return await query
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync();
    }
}