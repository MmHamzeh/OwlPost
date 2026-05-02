using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OwlPost.Core.Models;

namespace OwlPost.RabbitMq.Services;

internal class MessageConsumer : BackgroundService
{
    #region Fields and Ctor

    private readonly ILogger<MessageConsumer> _logger;
    private readonly IRabbitMqConnectionManagement _connection;
    private readonly RabbitMqOptions _options;
    private IChannel? _channel;
    private readonly IMessageRepository _messageRepository;

    internal MessageConsumer(
        ILogger<MessageConsumer> logger,
        IRabbitMqConnectionManagement connection,
        IOptions<RabbitMqOptions> options, IMessageRepository messageRepository)
    {
        _logger = logger;
        _connection = connection;
        _options = options.Value;
        _messageRepository = messageRepository;
    }

    #endregion


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.GetChannelAsync();

        if (_channel is null || !_channel.IsOpen)
            throw new Exception("Channel is not Accessible");

        foreach (var queue in _options.Queue)
        {
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    _logger.LogInformation(
                        "Message received from {Queue}: {Message}",
                        queue.Name,
                        json);

                    await ProcessMessageAsync(json, stoppingToken);

                    await _channel.BasicAckAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        stoppingToken);
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
                        stoppingToken);
                }
            };
        }
    }

    private async Task ProcessMessageAsync(string json, CancellationToken ct)
    {
        // TODO: deserialize message and handle it
        // save message to database

        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(json);

        if (chatMessage is null)
            throw new Exception();

        _ = await _messageRepository.Add(chatMessage);

        await Task.CompletedTask;
    }

}
