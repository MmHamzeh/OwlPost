using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace OwlPost.RabbitMq.Topology;

internal static class SeedData
{
    #region Const data

    internal const string ChatExchangeName = "chat.exchange";
    internal const string ChatQueueName = "chat.queue";
    internal const string ChatRoutingKey = "chat.queue";

    internal const string NotificationExchangeName = "notification.exchange";
    internal const string NotificationQueueName = "notification.queue";
    internal const string NotificationRoutingKey = "notification.queue";
    
    internal const string RoomExchangeName = "room.exchange";
    internal const string RoomQueueName = "room.queue";
    internal const string RoomRoutingKey = "room.queue";    

    #endregion

    private static List<ExchangeOption> ExchangeOptions =>
    [
        new()
        {
            Name = ChatExchangeName,
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        },
        new()
        {
            Name = NotificationExchangeName,
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        },
        new()
        {
            Name = RoomExchangeName,
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        }
    ];



    internal static ConnectionOption GetConnectionOption(IConfiguration configuration)
    {
        var json = configuration.GetSection("RabbitMQ:Connection")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("");

        var connectionOption = JsonSerializer.Deserialize<ConnectionOption>(json);

        if (connectionOption == null)
            throw new Exception("");

        return connectionOption;
    }


    internal static ChannelOption GetChannelOption(IConfiguration configuration)
    {
        var json = configuration.GetSection("RabbitMQ:ChannelOption")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("");

        var channelOption = JsonSerializer.Deserialize<ChannelOption>(json);

        if (channelOption == null)
            throw new Exception("");

        return channelOption;
    }

    internal static (List<ExchangeOption>, List<QueueOption>) GetExchangeOptionQueueOption()
    {

        var chatExchange = ExchangeOptions.First(e => e.Name == ChatExchangeName);
        var notificationExchange = ExchangeOptions.First(e => e.Name == NotificationExchangeName);
        var roomExchange = ExchangeOptions.First(e => e.Name == RoomExchangeName);

        #region QueueOption

        var chatQueue = new QueueOption(chatExchange, routingKey: ChatRoutingKey)
        {
            Name = ChatQueueName,
            Durable = true,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        var notificationQueue = new QueueOption(notificationExchange, routingKey: NotificationRoutingKey)
        {
            Name = NotificationQueueName,
            Durable = false,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        var roomQueue = new QueueOption(roomExchange, routingKey: RoomRoutingKey)
        {
            Name = RoomQueueName,
            Durable = true,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        #endregion

        List<ExchangeOption> exchangeOptionList =
        [
            chatExchange,
            notificationExchange,
            roomExchange
        ];

        List<QueueOption> queueOptionList =
        [
            chatQueue,
            notificationQueue,
            roomQueue
        ];

        return (exchangeOptionList, queueOptionList);
    }



}
