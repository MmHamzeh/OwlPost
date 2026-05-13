using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OwlPost.Core.Models;
using OwlPost.Core.RepositoriesContract;

namespace OwlPost.RabbitMq.Services;

internal class MessageConsumer : BackgroundService
{
    #region Fields and Ctor

    private readonly IAppLogger<MessageConsumer> _logger;
    private readonly IChannelManager _channelManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RabbitMqOptions _options;
    private readonly uint _prefetchSize;
    private readonly ushort _prefetchCount;

    private IChannel? _channel;

    internal MessageConsumer(
        IAppLogger<MessageConsumer> logger,
        IChannelManager channelManager,
        IOptions<RabbitMqOptions> options,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _channelManager = channelManager;
        _unitOfWork = unitOfWork;
        _options = options.Value;

        _prefetchSize = _options.Channel.PrefetchSize;
        _prefetchCount = _options.Channel.PrefetchCount;

    }

    #endregion


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _channel ??= await _channelManager.GetChannelAsync();

        if (_channel is null || !_channel.IsOpen)
            throw new Exception("Channel is not accessible");

        while (!ct.IsCancellationRequested)
        {
            foreach (var queue in _options.Queue)
            {
                await _channel.BasicQosAsync(
                    prefetchSize: _prefetchSize,
                    prefetchCount: _prefetchCount,
                    global: false,
                    ct);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        _logger.LogDebug(
                            "Message received from {Queue}: {Message}",
                            queue.Name,
                            json);

                        await ProcessMessageAsync(json, ct);

                        await _channel.BasicAckAsync(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error processing message from queue {Queue}",
                            queue.Name);

                        await _channel.BasicNackAsync(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            requeue: false,
                            ct);
                    }
                };
            }
        }
    }

    private async Task ProcessMessageAsync(string json, CancellationToken ct)
    {
        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(json);

        if (chatMessage is null)
            throw new Exception();

        await _unitOfWork.MessageRepository.Add(chatMessage, ct);
        _ = await _unitOfWork.SaveChanges(ct);

        await Task.CompletedTask;
    }

}
