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




   
    public async Task<IMessageBusResponse> SendMessage(IMessageBusSendMessageRequest request)
    {
        return await PublishMessageAsync(request, isPersistent: true);
    }

    public async Task<IMessageBusResponse> DeleteMessage(IMessageBusDeleteMessageRequest request)
    {
        return new EnqueueResponse();
    }

    public async Task<IMessageBusResponse> EditMessage(IMessageBusEditMessageRequest request)
    {
        return new EnqueueResponse();
    }

    public async Task<IMessageBusResponse> JoinRoom(IMessageBusJoinRoomRequest request)
    {
        return new EnqueueResponse();
    }

    public async Task<IMessageBusResponse> LeaveRoom(IMessageBusLeaveRoomRequest request)
    {
        return new EnqueueResponse();
    }

    public async Task<IMessageBusResponse> SendMessage<T>(T request) where T : IMessageBusSendMessageRequest
    {
        throw new NotImplementedException();
    }


    #region Private Methods

    private async Task<IMessageBusResponse> PublishMessageAsync(IMessageBusRequest request, bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        _channel ??= await _channelManager.GetChannelAsyncForPublish();

        var messageContent = string.Empty; // request.Content;
        var json = JsonSerializer.Serialize(messageContent);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = isPersistent
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

        return new EnqueueResponse();
    }


    #endregion
}
