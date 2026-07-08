using OwlPost.RabbitMq.Exceptions;

namespace OwlPost.RabbitMq.Services;

internal class MessageBus : IMessageBus
{
    private readonly IAppLogger<ExchangeManager> _logger;
    private readonly IChannelManager _channelManager;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore;
    private readonly ISerializer _serializer;

    internal MessageBus(IAppLogger<ExchangeManager> logger, IChannelManager channelManager, ISerializer serializer)
    {
        _logger = logger;
        _channelManager = channelManager;
        _serializer = serializer;
        _semaphore = new SemaphoreSlim(1, 1);
    }


    public async Task<IMessageBusResponse> SendMessage(IMessageBusSendMessageRequest request)
    {
        if (request is not MessageBusSendMessageRequest)
            throw new NotSupportedArgumentException("type of request must be MessageBusSendMessageRequest");

        return await PublishMessageAsync(request, isPersistent: true);
    }

    public async Task<IMessageBusResponse> DeleteMessage(IMessageBusDeleteMessageRequest request)
    {

        if (request is not MessageBusDeleteMessageRequest)
            throw new NotSupportedArgumentException("type of request must be MessageBusDeleteMessageRequest");

        return await PublishMessageAsync(request, isPersistent: true);
    }

    public async Task<IMessageBusResponse> EditMessage(IMessageBusEditMessageRequest request)
    {

        if (request is not MessageBusEditMessageRequest)
            throw new NotSupportedArgumentException("type of request must be MessageBusEditMessageRequest");

        return await PublishMessageAsync(request, isPersistent: true);
    }

    public async Task<IMessageBusResponse> JoinRoom(IMessageBusJoinRoomRequest request)
    {

        if (request is not MessageBusJoinRoomRequest)
            throw new NotSupportedArgumentException("type of request must be MessageBusJoinRoomRequest");

        return await PublishMessageAsync(request, isPersistent: true);
    }

    public async Task<IMessageBusResponse> LeaveRoom(IMessageBusLeaveRoomRequest request)
    {

        if (request is not MessageBusLeaveRoomRequest)
            throw new NotSupportedArgumentException("type of request must be MessageBusLeaveRoomRequest");

        return await PublishMessageAsync(request, isPersistent: true);
    }



    #region Private Methods

    private async Task<IMessageBusResponse> PublishMessageAsync(IMessageBusRequest request, bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        if (_channel is null)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                _channel ??= await _channelManager.GetChannelAsyncForPublish();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        var body = _serializer.Serialize(request);
        //var body = await _serializer.SerializeAsync(request);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = isPersistent
                ? DeliveryModes.Persistent
                : DeliveryModes.Transient
        };

        await _channel.BasicPublishAsync(
            exchange: "chat.exchange",
            routingKey: request.GroupingKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken
        );

        return new EnqueueResponse();
    }


    #endregion
}
