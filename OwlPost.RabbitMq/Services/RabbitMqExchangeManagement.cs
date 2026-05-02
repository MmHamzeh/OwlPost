namespace OwlPost.RabbitMq.Services;

internal class RabbitMqExchangeManagement : IRabbitMqExchangeManagement
{
    private readonly ILogger<RabbitMqExchangeManagement> _logger;
    private readonly IRabbitMqConnectionManagement _rabbitMqConnection;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private IChannel? _channel;

    internal RabbitMqExchangeManagement(ILogger<RabbitMqExchangeManagement> logger, IOptions<RabbitMqOptions> options, IRabbitMqConnectionManagement rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMqOptions = options.Value;
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async ValueTask DeclareExchanges()
    {
        var exchangeOptionList = _rabbitMqOptions.Exchange;

        await _sync.WaitAsync();
        try
        {
            _channel = await _rabbitMqConnection.GetChannelAsync();

            foreach (var exchangeOption in exchangeOptionList)
            {
                string exchangeType = exchangeOption.ExchangeTypeEnm switch
                {
                    ExchangeTypeEnm.Direct => ExchangeType.Direct,
                    ExchangeTypeEnm.Fanout => ExchangeType.Fanout,
                    ExchangeTypeEnm.Headers => ExchangeType.Headers,
                    ExchangeTypeEnm.Topic => ExchangeType.Topic,
                    _ => string.Empty
                };

                await _channel.ExchangeDeclareAsync(
                exchange: exchangeOption.Name,
                type: exchangeType,
                durable: exchangeOption.Durable,
                autoDelete: exchangeOption.AutoDelete,
                arguments: exchangeOption.Arguments
                );

                _logger.LogInformation($"\"{exchangeOption.Name}\" created.");
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
