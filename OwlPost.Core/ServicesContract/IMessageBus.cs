namespace OwlPost.Core.ServicesContract;

public interface IMessageBus 
{
    Task PublishAsync<T>(T message, string routingKey);

    Task PublishAsync<T>(T message, string exchange, string routingKey, bool isPersistent);
}
