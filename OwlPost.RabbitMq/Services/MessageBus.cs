using System.Text;
using System.Text.Json;

namespace OwlPost.RabbitMq.Services;

internal class MessageBus : IMessageBus
{
    private readonly ILogger<RabbitMqExchangeManagement> _logger;
    private readonly IRabbitMqConnectionManagement _rabbitMqConnection;

    internal MessageBus(ILogger<RabbitMqExchangeManagement> logger, IRabbitMqConnectionManagement rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async Task PublishAsync<T>(IMessageBusRequest request,
        CancellationToken cancellationToken = default) where T : IMessageBusResponse
    {
        var channel = await _rabbitMqConnection.GetChannelAsync();

        var messageContent = request.MessageContent;
        var json = JsonSerializer.Serialize(messageContent);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = request.IsPersistent
                ? DeliveryModes.Persistent
                : DeliveryModes.Transient
        };

        await channel.BasicPublishAsync(
            exchange: request.Exchange,
            routingKey: request.RoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken
        );
    }


}
