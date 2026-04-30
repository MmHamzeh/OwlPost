namespace OwlPost.RabbitMq.Contracts;

public interface IRabbitMqConnectionBuilder : IAsyncDisposable
{
    ValueTask<IConnection> GetConnectionAsync();
    ValueTask<IChannel> GetChannelAsync();


}
