namespace GateService.Services;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}