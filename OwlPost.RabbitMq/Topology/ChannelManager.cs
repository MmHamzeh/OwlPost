namespace OwlPost.RabbitMq.Topology;

internal interface IChannelManager : IAsyncDisposable
{
    ValueTask<IChannel> GetChannelAsync(IConnection? connection = null);
}

internal class ChannelManager : IChannelManager
{
    private readonly IAppLogger<ChannelManager> _logger;
    private readonly IConnectionManager _connectionManager;
    private IChannel? _channel;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private readonly RabbitMqOptions _rabbitMqOptions;

    internal ChannelManager(IAppLogger<ChannelManager> logger, IOptions<RabbitMqOptions> options, IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
        _rabbitMqOptions = options.Value;
    }


    public async ValueTask<IChannel> GetChannelAsync(IConnection? connection = null)
    {
        if (_channel is { IsOpen: true })
            return _channel!;

        connection ??= await _connectionManager.GetConnectionAsync();

        await _sync.WaitAsync();
        try
        {
            if (_channel is { IsOpen: true })
                return _channel!;

            _channel?.Dispose();

            var channelOption = _rabbitMqOptions.Channel;

            var opt = new CreateChannelOptions
            (
                publisherConfirmationsEnabled: channelOption.PublisherConfirmationsEnabled,
                publisherConfirmationTrackingEnabled: channelOption.PublisherConfirmationTrackingEnabled
            );

            _channel = await connection.CreateChannelAsync(opt);

            var connectionTimeOut = channelOption.ContinuationTimeout ??
                                    channelOption.DefaultContinuationTimeout;

            if (connectionTimeOut <= TimeSpan.Zero)
            {
                _logger.LogWarning("Invalid continuation timeout value: {Timeout}. " +
                                   "Using default value: {DefaultTimeout}.",
                    connectionTimeOut,
                    channelOption.DefaultContinuationTimeout);
                connectionTimeOut = channelOption.DefaultContinuationTimeout;
            }

            _channel.ContinuationTimeout = connectionTimeOut;

            _logger.LogInformation("RabbitMQ channel created.");
            return _channel!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ channel.");
            throw;
        }
        finally
        {
            _sync.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_channel is not null)
                await _channel.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error closing RabbitMQ resources.");
        }
        finally
        {
            _channel?.Dispose();
            _sync.Dispose();
        }
    }

}