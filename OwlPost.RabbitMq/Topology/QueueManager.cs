namespace OwlPost.RabbitMq.Topology;

internal interface IQueueManager : IAsyncDisposable
{
    ValueTask DeclareQueues(IChannel channel);
}

internal class QueueManager : IQueueManager
{
    private readonly ILogger<QueueManager> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private IChannel? _channel;

    internal QueueManager(ILogger<QueueManager> logger, IOptions<RabbitMqOptions> options)
    {
        _logger = logger;
        _rabbitMqOptions = options.Value;
    }

    public async ValueTask DeclareQueues(IChannel channel)
    {
        _channel = channel;
        var queueOptionList = _rabbitMqOptions.Queue;

        await _sync.WaitAsync();
        try
        {
            foreach (var queueOption in queueOptionList!)
            {
                var queue = await _channel.QueueDeclareAsync(
                    queue: queueOption.Name,
                    durable: queueOption.Durable,
                    exclusive: queueOption.Exclusive,
                    autoDelete: queueOption.AutoDelete,
                    arguments: queueOption.Arguments
                );

                if (queue is null)
                    throw new Exception($"Failed to create queue '{queueOption.Name}'.");

                _logger.LogInformation("Queue '{QueueOptionName}' created successfully.", queueOption.Name);

                await _channel.QueueBindAsync(
                    queue: queueOption.Name,
                    exchange: queueOption.Exchange.Name,
                    routingKey: queueOption.RoutingKey
                );

                _logger.LogInformation
                ("Queue '{QueueOptionName}' bound to exchange {ExchangeName} successfully.",
                    queueOption.Name, queueOption.Exchange.Name);

            }
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
            {
                await _sync.WaitAsync();
                try
                {
                    var queueOptionList = _rabbitMqOptions.Queue;

                    foreach (var queueOption in queueOptionList)
                    {
                        await _channel.QueueDeleteAsync(queue: queueOption.Name);
                    }

                    await _channel.CloseAsync();
                }
                finally
                {
                    _sync.Release();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing RabbitMQ resources.");
        }
        finally
        {
            _channel?.Dispose();
            _sync.Dispose();
        }
    }
}
