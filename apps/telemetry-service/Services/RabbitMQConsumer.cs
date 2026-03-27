using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TelemetryService.Models;
using TelemetryService.Repositories;

namespace TelemetryService.Services;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMQConsumer> _logger;

    public RabbitMQConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<RabbitMQConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"]
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "event.device", durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try
            {
                var evt = JsonSerializer.Deserialize<DeviceStateChangedEvent>(message);
                if (evt != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ITelemetryRepository>();
                    var telemetry = new Telemetry
                    {
                        DeviceId = evt.DeviceId,
                        DeviceType = evt.DeviceType,
                        HouseId = evt.HouseId,
                        Value = evt.Value ?? 0,
                        Unit = evt.Unit,
                        Status = evt.Status,
                        Timestamp = evt.Timestamp
                    };
                    await repo.SaveTelemetryAsync(telemetry);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Saved telemetry for device {DeviceId}", evt.DeviceId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        };

        _channel.BasicConsume(queue: "event.device", autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}