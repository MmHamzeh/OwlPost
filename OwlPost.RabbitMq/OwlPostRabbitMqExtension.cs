using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace OwlPost.RabbitMq;

public static class OwlPostRabbitMqExtension
{
    public static void AddRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        var exchangeOptionQueueOption = GetExchangeOptionQueueOption();

        var options = new Action<RabbitMqOptions>(opt =>
        {
            opt.Connection = GetConnectionOption(configuration);
            opt.Exchange = exchangeOptionQueueOption.Item1;
            opt.Queue = exchangeOptionQueueOption.Item2;
            opt.Channel = GetChannelOption(configuration);
        });

        services.Configure(options);
        AddServices(services);
    }

    #region Private Methods

    private static ConnectionOption GetConnectionOption(IConfiguration configuration)
    {
        var json = configuration.GetSection("RabbitMQ:Connection")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("");

        var connectionOption = JsonSerializer.Deserialize<ConnectionOption>(json);

        if (connectionOption == null)
            throw new Exception("");

        return connectionOption;
    }

    private static (List<ExchangeOption>, List<QueueOption>) GetExchangeOptionQueueOption()
    {

        #region ExchangeOption

        var chatExchange = new ExchangeOption()
        {
            Name = "chat.exchange",
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        };

        var notificationExchange = new ExchangeOption()
        {
            Name = "notification.exchange",
            ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            Durable = true,
            AutoDelete = false,
            Arguments = null
        };

        #endregion

        #region QueueOption

        var chatQueue = new QueueOption(chatExchange, routingKey: "chat.queue")
        {
            Name = "chat.queue",
            Durable = true,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        var notificationQueue = new QueueOption(notificationExchange, routingKey: "notification.queue")
        {
            Name = "notification.queue",
            Durable = false,
            Exclusive = true,
            AutoDelete = false,
            Arguments = null,
        };

        #endregion

        List<ExchangeOption> exchangeOptionList =
        [
            chatExchange,
            notificationExchange
        ];

        List<QueueOption> queueOptionList =
        [
            chatQueue,
            notificationQueue,
        ];

        return (exchangeOptionList, queueOptionList);
    }


    private static ChannelOption GetChannelOption(IConfiguration configuration)
    {
        var json = configuration.GetSection("RabbitMQ:ChannelOption")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("");

        var channelOption = JsonSerializer.Deserialize<ChannelOption>(json);

        if (channelOption == null)
            throw new Exception("");

        return channelOption;
    }


    private static void AddServices(IServiceCollection services)
    {

        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IChannelManager, ChannelManager>();
        services.AddSingleton<IExchangeManager, ExchangeManager>();
        services.AddSingleton<IQueueManager, QueueManager>();

        services.AddSingleton<IMessageBus, MessageBus>();

        services.AddHostedService<RabbitMqInitializer>();
        services.AddHostedService<MessageConsumer>();
    }

    #endregion

}
