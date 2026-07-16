namespace OwlPost.RabbitMq.Topology;

internal interface IConnectionManager : IAsyncDisposable
{
    ValueTask<IConnection> GetConnectionAsync();
}

internal sealed class ConnectionManager(ILogger<ConnectionManager> logger, IOptions<RabbitMqOptions> options)
    : IConnectionManager
{
    private IConnection? _connection;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private readonly RabbitMqOptions _rabbitMqOptions = options.Value;

    public async ValueTask<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection!;

        var connectionOption = _rabbitMqOptions.Connection;

        var factory = new ConnectionFactory
        {
            HostName = connectionOption.HostName,
            UserName = connectionOption.UserName,
            Password = connectionOption.Password,
            Port = connectionOption.Port
        };

        await _sync.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true })
                return _connection!;

            _connection?.Dispose();

            _connection = await factory.CreateConnectionAsync();

            logger.LogInformation("RabbitMQ connection established to {Host}:{Port}.",
                connectionOption.HostName,
                connectionOption.Port);

            return _connection!;
        }
        catch (BrokerUnreachableException ex)
        {
            logger.LogError(ex, "RabbitMQ broker at {Host}:{Port} is unreachable.",
                factory.HostName,
                factory.Port);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to establish RabbitMQ connection to {Host}:{Port}.",
                factory.HostName,
                factory.Port);
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
            if (_connection is not null)
                await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error closing RabbitMQ resources.");
        }
        finally
        {
            _connection?.Dispose();
            _sync.Dispose();
        }
    }

}
