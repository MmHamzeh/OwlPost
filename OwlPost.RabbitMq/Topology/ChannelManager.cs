namespace OwlPost.RabbitMq.Topology;

internal interface IChannelManager : IAsyncDisposable
{
    ValueTask<IChannel> GetChannelAsyncForPublish();
    ValueTask<IChannel> GetChannelAsyncForConsume();
}

internal class ChannelManager : IChannelManager
{
    private readonly ILogger<ChannelManager> _logger;
    private readonly IConnectionManager _connectionManager;
    private IChannel? _publishChannel;
    private IChannel? _consumeChannel;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private readonly RabbitMqOptions _rabbitMqOptions;

    internal ChannelManager(ILogger<ChannelManager> logger, IOptions<RabbitMqOptions> options, IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
        _rabbitMqOptions = options.Value;
    }


    public async ValueTask<IChannel> GetChannelAsyncForPublish()
    {
        if (_publishChannel is { IsOpen: true })
            return _publishChannel!;

        var connection = await _connectionManager.GetConnectionAsync();

        await _sync.WaitAsync();
        try
        {
            if (_publishChannel is { IsOpen: true })
                return _publishChannel!;

            _publishChannel?.Dispose();

            var channelOption = _rabbitMqOptions.Channel;

            var opt = new CreateChannelOptions
            (
                publisherConfirmationsEnabled: channelOption.PublisherConfirmationsEnabled,
                publisherConfirmationTrackingEnabled: channelOption.PublisherConfirmationTrackingEnabled
            );

            _publishChannel = await connection.CreateChannelAsync(opt);

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

            _publishChannel.ContinuationTimeout = connectionTimeOut;

            _logger.LogInformation("RabbitMQ channel created.");
            return _publishChannel!;
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

    public async ValueTask<IChannel> GetChannelAsyncForConsume()
    {
        if (_consumeChannel is { IsOpen: true })
            return _consumeChannel!;

        var connection = await _connectionManager.GetConnectionAsync();

        await _sync.WaitAsync();
        try
        {
            if (_consumeChannel is { IsOpen: true })
                return _consumeChannel!;

            _consumeChannel?.Dispose();

            var channelOption = _rabbitMqOptions.Channel;

            var opt = new CreateChannelOptions
            (
                publisherConfirmationsEnabled: channelOption.PublisherConfirmationsEnabled,
                publisherConfirmationTrackingEnabled: channelOption.PublisherConfirmationTrackingEnabled
            );

            _consumeChannel = await connection.CreateChannelAsync(opt);

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

            _consumeChannel.ContinuationTimeout = connectionTimeOut;

            _logger.LogInformation("RabbitMQ channel created.");
            return _consumeChannel!;
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
            if (_publishChannel is not null)
                await _publishChannel.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error closing RabbitMQ resources.");
        }
        finally
        {
            _publishChannel?.Dispose();
            _sync.Dispose();
        }
    }

}