using OwlPost.RabbitMq.Exceptions;

namespace OwlPost.RabbitMq.Services;

internal class MessageBus : IMessageBus
{
    private readonly IAppLogger<MessageBus> _logger;
    private readonly IChannelManager _channelManager;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore;
    private readonly ISerializer _serializer;

    internal MessageBus(IAppLogger<MessageBus> logger, IChannelManager channelManager, ISerializer serializer)
    {
        _logger = logger;
        _channelManager = channelManager;
        _serializer = serializer;
        _semaphore = new SemaphoreSlim(1, 1);

    }


    public async Task<IMessageBusResponse> SendMessage(IMessageBusSendMessageRequest request, CancellationToken ct)
    {
        if (request is not MessageBusSendMessageRequest)
            throw new NotSupportedArgumentException(nameof(request), typeof(MessageBusSendMessageRequest));


        return await PublishMessageAsync(request, isPersistent: true, SeedData.ChatExchangeName, ct);
    }

    public async Task<IMessageBusResponse> DeleteMessage(IMessageBusDeleteMessageRequest request, CancellationToken ct)
    {

        if (request is not MessageBusDeleteMessageRequest)
            throw new NotSupportedArgumentException(nameof(request), typeof(MessageBusDeleteMessageRequest));

        return await PublishMessageAsync(request, isPersistent: true, SeedData.ChatExchangeName, ct);
    }

    public async Task<IMessageBusResponse> EditMessage(IMessageBusEditMessageRequest request, CancellationToken ct)
    {

        if (request is not MessageBusEditMessageRequest)
            throw new NotSupportedArgumentException(nameof(request), typeof(MessageBusEditMessageRequest));

        return await PublishMessageAsync(request, isPersistent: true, SeedData.ChatExchangeName, ct);
    }

    public async Task<IMessageBusResponse> JoinRoom(IMessageBusJoinRoomRequest request, CancellationToken ct)
    {

        if (request is not MessageBusJoinRoomRequest)
            throw new NotSupportedArgumentException(nameof(request), typeof(MessageBusJoinRoomRequest));

        return await PublishMessageAsync(request, isPersistent: true, SeedData.RoomExchangeName, ct);
    }

    public async Task<IMessageBusResponse> LeaveRoom(IMessageBusLeaveRoomRequest request, CancellationToken ct)
    {

        if (request is not MessageBusLeaveRoomRequest)
            throw new NotSupportedArgumentException(nameof(request), typeof(MessageBusLeaveRoomRequest));

        return await PublishMessageAsync(request, isPersistent: true, SeedData.RoomExchangeName, ct);
    }



    #region Private Methods

    private async Task<IMessageBusResponse> PublishMessageAsync(IMessageBusRequest request, bool isPersistent,
        string exchangeName, CancellationToken ct)
    {
        try
        {
            if (_channel is null)
            {
                try
                {
                    await _semaphore.WaitAsync(ct);
                    _channel ??= await _channelManager.GetChannelAsyncForPublish();
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            var content = _serializer.Serialize(request);

            var props = new BasicProperties
            {
                ContentType = _serializer.ContentType,
                ContentEncoding = _serializer.ContentEncoding,
                MessageId = Guid.CreateVersion7().ToString(),
                DeliveryMode = isPersistent
                    ? DeliveryModes.Persistent
                    : DeliveryModes.Transient,
                Persistent = isPersistent
            };

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: request.GroupingKey,
                mandatory: false,
                basicProperties: props,
                body: content,
                ct
            );

            _logger.LogDebug(
                "Message published to exchange \"{ExchangeName}\" with routing key \"{RequestGroupingKey}\"",
                exchangeName, request.GroupingKey);

            return new EnqueueResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while publishing message to exchange \"{ExchangeName}\" with routing key \"{RequestGroupingKey}\"", exchangeName, request.GroupingKey);
            throw;
        }
    }


    #endregion
}
