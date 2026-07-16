namespace OwlPost.RabbitMq.Services;

internal class MessageBus : IMessageBus, IAsyncDisposable
{
    #region Fields and Ctor

    private readonly ILogger<MessageBus> _logger;
    private readonly IChannelManager _channelManager;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore;
    private readonly ISerializer _serializer;
    private readonly TimeProvider _timeProvider;

    internal MessageBus(ILogger<MessageBus> logger, IChannelManager channelManager, ISerializer serializer, TimeProvider timeProvider)
    {
        _logger = logger;
        _channelManager = channelManager;
        _serializer = serializer;
        _timeProvider = timeProvider;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    #endregion

    #region Message Section

    public async Task<IMessageBusResponse> SendMessage(MessageBusSendMessageRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.ChatExchangeName, ct);

    public async Task<IMessageBusResponse> DeleteMessage(MessageBusDeleteMessageRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.ChatExchangeName, ct);

    public async Task<IMessageBusResponse> EditMessage(MessageBusEditMessageRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.ChatExchangeName, ct);

    #endregion

    #region ChatRoom Serction


    public async Task<IMessageBusResponse> JoinRoom(MessageBusJoinRoomRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.RoomExchangeName, ct);

    public async Task<IMessageBusResponse> LeaveRoom(MessageBusLeaveRoomRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.RoomExchangeName, ct);

    public async Task<IMessageBusResponse> CreateRoom(MessageBusCreateRoomRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.RoomExchangeName, ct);

    public async Task<IMessageBusResponse> EditRoom(MessageBusEditRoomRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.RoomExchangeName, ct);

    public async Task<IMessageBusResponse> DeleteRoom(MessageBusDeleteRoomRequest request, CancellationToken ct)
        => await PublishMessageAsync(request, isPersistent: true, ConstData.RoomExchangeName, ct);

    #endregion


    #region Private Methods

    private async Task<IMessageBusResponse> PublishMessageAsync(IMessageBusRequest request, bool isPersistent,
        string exchangeName, CancellationToken ct, bool mandatory = false)
    {
        try
        {
            var channel = await GetChannel(ct);

            var content = _serializer.Serialize(request);

            var basicPropertiesHeaders = new Dictionary<string, object?>
            {
                { ConstData.BasicPropertiesHeaders_MessageType, request.GetType().Name }
            };

            var props = new BasicProperties
            {
                ContentType = _serializer.ContentType,
                ContentEncoding = _serializer.ContentEncoding,
                MessageId = Guid.CreateVersion7().ToString(),
                DeliveryMode = isPersistent
                    ? DeliveryModes.Persistent
                    : DeliveryModes.Transient,
                Persistent = isPersistent,
                Timestamp = new AmqpTimestamp(_timeProvider.GetUtcNow().ToUnixTimeSeconds()),
                Headers = basicPropertiesHeaders
            };

            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: request.GroupingKey,
                mandatory: mandatory,
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

    private async Task<IChannel> GetChannel(CancellationToken ct)
    {
        if (_channel is not null && _channel.IsOpen)
            return _channel;

        try
        {
            await _semaphore.WaitAsync(ct);

            if (_channel is not null && !_channel.IsOpen)
            {
                if (!_channel.IsClosed)
                    await _channel.CloseAsync(ct);

                await _channel.DisposeAsync();
            }

            _channel ??= await _channelManager.GetChannelAsyncForPublish();
        }
        finally
        {
            _semaphore.Release();
        }

        return _channel;
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        await _channelManager.DisposeAsync();
        if (_channel != null) await _channel.DisposeAsync();
        _semaphore.Dispose();
    }

}
