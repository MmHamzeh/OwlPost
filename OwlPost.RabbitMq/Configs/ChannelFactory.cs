namespace OwlPost.RabbitMq.Configs;

internal class ChannelFactory
{
    #region Fields and Ctor

    private IChannel? _channel;
    private IConnection? _connection;
    private readonly ConnectionFactory _connectionFactory;


    public ChannelFactory(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #endregion

    internal async Task<IChannel> GetChannelAsync()
    {
        if (_channel is not null)
            return _channel;

        _connection ??= await _connectionFactory.GetRabbitConnection();

        _channel = await _connection.CreateChannelAsync();
        return _channel;
    }
}
