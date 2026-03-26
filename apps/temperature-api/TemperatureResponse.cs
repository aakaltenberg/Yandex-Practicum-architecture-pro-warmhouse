using System.Text.Json.Serialization;
public class TemperatureResponse
{
    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "C";

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "active";

    [JsonPropertyName("sensor_id")]
    public string SensorId { get; set; } = "";

    [JsonPropertyName("sensor_type")]
    public string SensorType { get; set; } = "temperature";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "Random temperature sensor";
}