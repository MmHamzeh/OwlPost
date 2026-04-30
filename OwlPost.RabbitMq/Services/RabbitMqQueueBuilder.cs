using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OwlPost.RabbitMq.Services;

internal class RabbitMqQueueBuilder : IRabbitMqQueueBuilder
{
    private readonly ILogger<RabbitMqExchangeBuilder> _logger;
    private readonly IRabbitMqConnectionBuilder _rabbitMqConnection;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private IChannel? _channel;

    internal RabbitMqQueueBuilder(ILogger<RabbitMqExchangeBuilder> logger, IOptions<RabbitMqOptions> options, IRabbitMqConnectionBuilder rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMqOptions = options.Value;
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async ValueTask DeclareQueues()
    {
        _channel = await _rabbitMqConnection.GetChannelAsync();
        var queueOptionList = _rabbitMqOptions.Queue;

        foreach (var queueOption in queueOptionList!)
        {
            var queue = await _channel!.QueueDeclareAsync(
            queue: queueOption.Name,
            durable: queueOption.Durable,
            exclusive: queueOption.Exclusive,
            autoDelete: queueOption.AutoDelete,
            arguments: queueOption.Arguments
            ) ?? throw new Exception($"Failed to create queue '{queueOption.Name}'.");

            _logger.LogInformation($"Queue '{queueOption.Name}' created successfully.");

            await _channel!.QueueBindAsync(
                queue: queueOption.Name,
                exchange: queueOption.Exchange.Name,
                routingKey: queueOption.Name
            );

            _logger.LogInformation
                ($"Queue '{queueOption.Name}' binded to exchange {queueOption.Exchange.Name} successfully.");

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
