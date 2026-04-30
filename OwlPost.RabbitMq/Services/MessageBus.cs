using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace OwlPost.RabbitMq.Services;

internal class MessageBus : IMessageBus
{
    private readonly ILogger<RabbitMqExchangeBuilder> _logger;
    private readonly IRabbitMqConnectionBuilder _rabbitMqConnection;

    internal MessageBus(ILogger<RabbitMqExchangeBuilder> logger, IRabbitMqConnectionBuilder rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMqConnection = rabbitMqConnection;
    }

    public Task PublishAsync<T>(T message, string routingKey)
    {
        throw new NotImplementedException();


        //if (MainConfig.Channel is null)
        //    throw new InvalidOperationException("RabbitMQ channel is not initialized.");

        //var json = JsonSerializer.Serialize(message);
        //var body = Encoding.UTF8.GetBytes(json);

        //var props = new BasicProperties
        //{
        //    ContentType = "application/json",
        //    DeliveryMode = DeliveryModes.Persistent
        //};

        //await MainConfig.Channel.BasicPublishAsync(
        //    exchange: "",
        //    routingKey: routingKey,
        //    mandatory: false,
        //    basicProperties: props,
        //    body: body
        //);
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey, bool isPersistent)
    {
        var channel = await _rabbitMqConnection.GetChannelAsync();

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = isPersistent ? DeliveryModes.Persistent : DeliveryModes.Transient
        };

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }


}
