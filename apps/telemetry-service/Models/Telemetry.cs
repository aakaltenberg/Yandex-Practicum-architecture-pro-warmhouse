namespace TelemetryService.Models;

public class Telemetry
{
    public long Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // heating, lighting, camera, gate
    public string? HouseId { get; set; }
    public double Value { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}