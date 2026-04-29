namespace OwlPost.RabbitMq.Configs;

internal class QueueExchange
{

    #region Fields and Ctor

    private readonly ConnectionFactory _connectionFactory;
    private readonly ChannelFactory _channelFactory;
    private readonly ExchangeFactory _exchangeFactory;

    internal QueueExchange()
    {
        _connectionFactory = new ConnectionFactory();
        _channelFactory = new ChannelFactory(_connectionFactory);
        _exchangeFactory = new ExchangeFactory();
    }

    #endregion

    private QueueDeclareOk? _messagingQueue;

    internal async Task CreateMessageQueue()
    {
        var channel = await _channelFactory.GetChannelAsync();

        if (_messagingQueue is not null)
            return;

        await _exchangeFactory.CreateMessagingExchange();

        var queueOptions = MainConfig.QueueOptions;

        foreach (var queueOption in queueOptions!)
        {
            _messagingQueue = await channel.QueueDeclareAsync(
            queue: queueOption.Name,
            durable: queueOption.Durable,
            exclusive: queueOption.Exclusive,
            autoDelete: queueOption.AutoDelete,
            arguments: queueOption.Arguments
            ) ?? throw new Exception($"Failed to create queue '{queueOption.Name}'.");

            Console.WriteLine($"Queue '{queueOption.Name}' created successfully.");

            await channel.QueueBindAsync(
                queue: queueOption.Name,
                exchange: queueOption.Exchange.Name,
                routingKey: queueOption.Name
            );

            Console.WriteLine
                ($"Queue '{queueOption.Name}' binded to exchange {queueOption.Exchange.Name} successfully.");

        }
    }

}
