namespace OwlPost.RabbitMq.Contracts;

public interface IRabbitMqConnectionManagement : IAsyncDisposable
{
    ValueTask<IConnection> GetConnectionAsync();
    ValueTask<IChannel> GetChannelAsync();


}
