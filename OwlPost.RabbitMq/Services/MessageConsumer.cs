using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OwlPost.RabbitMq.Services;

internal sealed class MessageConsumer : BackgroundService
{
    #region Fields and Ctor

    private readonly IAppLogger<MessageConsumer> _logger;
    private readonly IChannelManager _channelManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly uint _prefetchSize;
    private readonly ushort _prefetchCount;
    private readonly IDictionary<string, IChannel> _channelsPerQueue;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    internal MessageConsumer(
        IAppLogger<MessageConsumer> logger,
        IChannelManager channelManager,
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _channelManager = channelManager;
        _scopeFactory = scopeFactory;
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
            _channelsPerQueue.Add(queue.Name, await _channelManager.GetChannelAsync());
        }

        #region Consume Messages Action

        var tasks = _options.Queue
            .Select(queue => ConsumeQueue(queue.Name, ct));

        await Task.WhenAll(tasks);


        #endregion

        await Task.Delay(Timeout.InfiniteTimeSpan, ct);
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _channelsPerQueue.Values.Select(async channel =>
            {
                try
                {
                    if (channel.IsOpen)
                        await channel.CloseAsync(cancellationToken);

                    await channel.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error closing RabbitMQ channel");
                }
            }));

        await base.StopAsync(cancellationToken);
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

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var consumerChannel = ((AsyncEventingBasicConsumer)sender).Channel;

            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                _logger.LogDebug(
                    "Message received from {Queue}",
                    queueName);

                await ProcessMessageAsync(json);

                await consumerChannel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing message from queue {Queue}",
                    queueName);

                await consumerChannel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    ct);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            consumerTag: $"consumer-{Environment.MachineName}-{queueName}",
            cancellationToken: ct);
    }

    private async Task ProcessMessageAsync(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("Received empty message payload");

        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(json, _jsonOptions);

        if (chatMessage is null)
            throw new InvalidOperationException("Failed to deserialize ChatMessage");

        await using var scope = _scopeFactory.CreateAsyncScope();

        var unitOfWork =
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await unitOfWork.MessageRepository.Add(chatMessage, saveChanges: true, CancellationToken.None);

    }

    #endregion

}
