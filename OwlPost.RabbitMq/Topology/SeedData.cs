using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace OwlPost.RabbitMq.Topology;

internal static class SeedData
{
    private static List<ExchangeOption> ExchangeOptions =>
    [
        new()
        {
            Name = ConstData.ChatExchangeName,
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        },
        new()
        {
            Name = ConstData.NotificationExchangeName,
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        },
        new()
        {
            Name = ConstData.RoomExchangeName,
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

        var chatExchange = ExchangeOptions.First(e => e.Name == ConstData.ChatExchangeName);
        var notificationExchange = ExchangeOptions.First(e => e.Name == ConstData.NotificationExchangeName);
        var roomExchange = ExchangeOptions.First(e => e.Name == ConstData.RoomExchangeName);

        #region QueueOption

        var chatQueue = new QueueOption(chatExchange, routingKey: ConstData.ChatRoutingKey)
        {
            Name = ConstData.ChatQueueName,
            Durable = true,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        var notificationQueue = new QueueOption(notificationExchange, routingKey: ConstData.NotificationRoutingKey)
        {
            Name = ConstData.NotificationQueueName,
            Durable = false,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        var roomQueue = new QueueOption(roomExchange, routingKey: ConstData.RoomRoutingKey)
        {
            Name = ConstData.RoomQueueName,
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
