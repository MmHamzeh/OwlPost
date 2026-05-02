namespace OwlPost.RabbitMq.Services;

internal sealed class RabbitMqConnectionManagement : IRabbitMqConnectionManagement
{
    private readonly ILogger<RabbitMqConnectionManagement> _logger;
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private readonly RabbitMqOptions _rabbitMqOptions;

    public RabbitMqConnectionManagement(ILogger<RabbitMqConnectionManagement> logger, IOptions<RabbitMqOptions> options)
    {
        _logger = logger;

        _rabbitMqOptions = options.Value;

        var connectionOption = options.Value.Connection;

        _factory = new ConnectionFactory
        {
            HostName = connectionOption.HostName,
            UserName = connectionOption.UserName,
            Password = connectionOption.Password,
            Port = connectionOption.Port,
        };
    }

    public async ValueTask<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection!;

        await _sync.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true })
                return _connection!;

            _connection?.Dispose();

            var connectionOption = _rabbitMqOptions.Connection;

            _connection = await _factory.CreateConnectionAsync();
            _logger.LogInformation("RabbitMQ connection established to {Host}:{Port}.",
                connectionOption.HostName,
                connectionOption.Port);
            return _connection!;
        }
        finally
        {
            _sync.Release();
        }
    }

    public async ValueTask<IChannel> GetChannelAsync()
    {
        if (_channel is { IsOpen: true })
            return _channel!;

        var connection = await GetConnectionAsync();

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

            _channel.ContinuationTimeout =
                channelOption.ContinuationTimeout ?? channelOption.DefaultContinuationTimeout;

            _logger.LogInformation("RabbitMQ channel created.");
            return _channel!;
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

            if (_connection is not null)
                await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error closing RabbitMQ resources.");
        }
        finally
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _sync.Dispose();
        }
    }

}
