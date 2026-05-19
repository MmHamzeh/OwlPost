using System.Text;
using System.Text.Json;

namespace OwlPost.RabbitMq.Services;

internal class MessageBus : IMessageBus
{
    private readonly IAppLogger<ExchangeManager> _logger;
    private readonly IChannelManager _channelManager;
    private IChannel? _channel;

    internal MessageBus(IAppLogger<ExchangeManager> logger, IChannelManager channelManager)
    {
        _logger = logger;
        _channelManager = channelManager;
    }

    public async Task PublishMessageAsync<T>(IMessageBusRequest request,
        CancellationToken cancellationToken = default) where T : IMessageBusResponse
    {
        _channel ??= await _channelManager.GetChannelAsyncForPublish();

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

        await _channel.BasicPublishAsync(
            exchange: "chat.exchange",
            routingKey: "chat.routingKey",
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken
        );
    }


}
