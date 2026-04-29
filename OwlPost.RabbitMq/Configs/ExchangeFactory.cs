namespace OwlPost.RabbitMq.Configs;

internal class ExchangeFactory
{
    internal readonly string MessagingExchangeName = "messaging.exchange";

    internal async Task CreateMessagingExchange()
    {
        var connectionFactory = new ConnectionFactory();

        var connection = await connectionFactory.GetRabbitConnection();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: MessagingExchangeName,
            type: ExchangeType.Direct,
            durable: false,
            autoDelete: false,
            arguments: null
        );

        Console.WriteLine($"\"{MessagingExchangeName}\" created.");
    }
}
