using System.Text.Json.Serialization;

namespace TelemetryService.Models;

public class DeviceStateChangedEvent
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("houseId")]
    public string HouseId { get; set; } = string.Empty;

    [JsonPropertyName("deviceType")]
    public string DeviceType { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}