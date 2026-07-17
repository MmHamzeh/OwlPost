using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System.Text;

namespace OwlPost.RabbitMq.Services;

internal sealed class MessageConsumer : BackgroundService
{
    #region Fields and Ctor

    private readonly ILogger<MessageConsumer> _logger;
    private readonly IChannelManager _channelManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly uint _prefetchSize;
    private readonly ushort _prefetchCount;
    private readonly IDictionary<string, IChannel> _channelsPerQueue;
    private readonly ISerializer _serializer;

    internal MessageConsumer(


        ILogger<MessageConsumer> logger,
        IChannelManager channelManager,
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ISerializer serializer)
    {
        _logger = logger;
        _channelManager = channelManager;
        _scopeFactory = scopeFactory;
        _serializer = serializer;
        _options = options.Value;

        _channelsPerQueue = new Dictionary<string, IChannel>(_options.Queue.Count);
        _prefetchSize = _options.Channel.PrefetchSize;
        _prefetchCount = _options.Channel.PrefetchCount;
    }

    #endregion


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        foreach (var queue in _options.Queue)
        {
            _channelsPerQueue.Add(queue.Name, await _channelManager.GetChannelAsyncForConsume());
        }

        #region Consume Messages Action

        var tasks = _options.Queue
            .Select(queue => ConsumeQueue(queue.Name, ct));

        await Task.WhenAll(tasks);


        #endregion

        await Task.Delay(Timeout.InfiniteTimeSpan, ct);
    }


    public override async Task StopAsync(CancellationToken ct)
    {
        await Task.WhenAll(
            _channelsPerQueue.Values.Select(async channel =>
            {
                try
                {
                    if (channel.IsOpen)
                        await channel.CloseAsync(ct);

                    await channel.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error closing RabbitMQ channel");
                }
            }));

        await base.StopAsync(ct);
    }


    #region Private Methods

    private async Task ConsumeQueue(string queueName, CancellationToken ct)
    {
        var channel = _channelsPerQueue[queueName];

        if (channel is null || !channel.IsOpen)
            throw new Exception("Channel is not accessible");

        await channel.BasicQosAsync(
            prefetchSize: _prefetchSize,
            prefetchCount: _prefetchCount,
            global: false,
            ct);

        var consumer = new AsyncEventingBasicConsumer(channel);

        AddReceivingEvent(queueName, consumer, channel, ct);

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            consumerTag: $"consumer-{Environment.MachineName}-{queueName}",
            cancellationToken: ct);
    }

    private void AddReceivingEvent(string queueName, AsyncEventingBasicConsumer consumer, IChannel channel, CancellationToken ct)
    {
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var consumerChannel = ((AsyncEventingBasicConsumer)sender).Channel;

            try
            {
                if (ea.Body.IsEmpty || ea.Body.Length == 0)
                {
                    _logger.LogWarning("Received empty message from queue {Queue}", queueName);
                    return;
                }

                var serializedData = ea.Body.ToArray();
                var structuredData = DeserializeRequest(serializedData, ea.BasicProperties);

                if (structuredData is null)
                {
                    _logger.LogWarning("Failed to deserialize message from queue {Queue}", queueName);
                    return;
                }

                _logger.LogDebug("Message received from {Queue}", structuredData);

                await using var scope = _scopeFactory.CreateAsyncScope();

                var processor = scope.ServiceProvider
                    .GetRequiredService<IConsumedMessageProcessor>();

                await processor.ProcessAsync(structuredData, ct);

                await consumerChannel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing message from queue {Queue}. Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                    queueName,
                    ea.Exchange,
                    ea.RoutingKey);

                try
                {
                    await channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception nackEx)
                {
                    _logger.LogError(
                        nackEx,
                        "Failed to nack message from queue {Queue}",
                        queueName);
                }
            }
        };
    }
    
    private IMessageBusRequest? DeserializeRequest(byte[] body, IReadOnlyBasicProperties? properties)
    {
        var messageType = GetMessageType(properties);

        return messageType switch
        {
            nameof(MessageBusSendMessageRequest) =>
                _serializer.Deserialize<MessageBusSendMessageRequest>(body),

            nameof(MessageBusEditMessageRequest) =>
                _serializer.Deserialize<MessageBusEditMessageRequest>(body),

            nameof(MessageBusDeleteMessageRequest) =>
                _serializer.Deserialize<MessageBusDeleteMessageRequest>(body),

            nameof(MessageBusCreateRoomRequest) =>
                _serializer.Deserialize<MessageBusCreateRoomRequest>(body),

            nameof(MessageBusEditRoomRequest) =>
                _serializer.Deserialize<MessageBusEditRoomRequest>(body),

            nameof(MessageBusDeleteRoomRequest) =>
                _serializer.Deserialize<MessageBusDeleteRoomRequest>(body),

            nameof(MessageBusJoinRoomRequest) =>
                _serializer.Deserialize<MessageBusJoinRoomRequest>(body),

            nameof(MessageBusLeaveRoomRequest) =>
                _serializer.Deserialize<MessageBusLeaveRoomRequest>(body),

            _ => throw new NotSupportedException(
                $"Unsupported message-type header: {messageType ?? "<null>"}")
        };
    }

    private static string? GetMessageType(IReadOnlyBasicProperties? properties)
    {
        if (properties?.Headers is null)
            return null;

        if (!properties.Headers.TryGetValue(ConstData.BasicPropertiesHeaders_MessageType, out var raw))
            return null;

        return raw switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string s => s,
            _ => raw?.ToString()
        };
    }

    #endregion

}
