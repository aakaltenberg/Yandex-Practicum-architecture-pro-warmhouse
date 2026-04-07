var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

static object GetTemperatureResponse(string sensorId, string location)
{
    if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(sensorId))
    {
        return Results.BadRequest(new { error = "Either location or sensorId must be provided" });
    }

    string finalLocation = location ?? "";
    if (string.IsNullOrEmpty(finalLocation))
    {
        finalLocation = sensorId switch
        {
            "1" => "Living Room",
            "2" => "Bedroom",
            "3" => "Kitchen",
            _ => "Unknown"
        };
    }

    string finalSensorId = sensorId ?? "";
    if (string.IsNullOrEmpty(finalSensorId))
    {
        finalSensorId = finalLocation switch
        {
            "Living Room" => "1",
            "Bedroom" => "2",
            "Kitchen" => "3",
            _ => "0"
        };
    }

    var random = new Random();
    double value = random.Next(-100, 351) / 10.0;

    var response = new TemperatureResponse
    {
        Value = value,
        Unit = "C",
        Timestamp = DateTime.UtcNow,
        Location = finalLocation,
        Status = "active",
        SensorId = finalSensorId,
        SensorType = "temperature",
        Description = "Random temperature sensor"
    };

    return Results.Ok(response);
}

app.MapGet("/temperature", (string? location, string? sensorId) =>
{
    return GetTemperatureResponse(sensorId ?? "", location ?? "");
});

app.MapGet("/temperature/{sensorId}", (string sensorId) =>
{
    return GetTemperatureResponse(sensorId, null);
});

app.Run();