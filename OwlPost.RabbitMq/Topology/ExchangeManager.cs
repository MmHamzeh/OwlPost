namespace OwlPost.RabbitMq.Topology;

internal interface IExchangeManager : IAsyncDisposable
{
    ValueTask DeclareExchanges(IChannel channel);
}

internal class ExchangeManager : IExchangeManager
{
    private readonly IAppLogger<ExchangeManager> _logger;
    private readonly IConnectionManager _connection;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private IChannel? _channel;

    internal ExchangeManager(IAppLogger<ExchangeManager> logger, IOptions<RabbitMqOptions> options, IConnectionManager connection)
    {
        _logger = logger;
        _rabbitMqOptions = options.Value;
        _connection = connection;
    }

    public async ValueTask DeclareExchanges(IChannel channel)
    {
        var exchangeOptionList = _rabbitMqOptions.Exchange;
        _channel = channel;

        await _sync.WaitAsync();
        try
        {

            foreach (var exchangeOption in exchangeOptionList)
            {
                var exchangeType = exchangeOption.ExchangeTypeEnm switch
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

                _logger.LogInformation("\"{ExchangeOptionName}\" created.", exchangeOption.Name);
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
                try
                {
                    await _sync.WaitAsync();

                    var exchangeOptionList = _rabbitMqOptions.Exchange;

                    foreach (var exchangeOption in exchangeOptionList)
                    {
                        await _channel.ExchangeDeleteAsync(exchange: exchangeOption.Name);
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
            _logger.LogWarning(ex, "Error closing RabbitMQ resources.");
        }
        finally
        {
            _channel?.Dispose();
            _sync.Dispose();
        }
    }
}
