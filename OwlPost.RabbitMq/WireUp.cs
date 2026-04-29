using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace OwlPost.RabbitMq;

public static class WireUp
{
    private static async Task SetupRabbit(
        ConnectionOption connectionOption,
        List<ExchangeOption> exchangeOptionList,
        List<QueueOption> queueOptionList)
    {

        var connectionBuilder = new ConnectionBuilder();
        MainConfig.Connection = await connectionBuilder.GetRabbitConnection(connectionOption);

        var channelBuilder = new ChannelBuilder();
        MainConfig.Channel = await channelBuilder.GetChannelAsync();

        var exchangeBuilder = new ExchangeBuilder();
        await exchangeBuilder.CreateMessagingExchange(exchangeOptionList);

        var queueBuilder = new QueueBuilder();
        await queueBuilder.CreateQueueList(queueOptionList);

    }


    private static async Task SetupRabbit()
    {
        ConnectionOption connectionOption = new()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        List<ExchangeOption> exchangeOptionList =
        [
            new ExchangeOption()
            {
                Name = "messaging.exchange",
                Durable = true,
                AutoDelete = true,
                ExchangeTypeEnm = ExchangeTypeEnm.Direct,
            }
        ];

        List<QueueOption> queueOptionList =
        [
            new QueueOption(exchangeOptionList.Single(e => e.Name.Equals("messaging.exchange")))
            {
                Name = "messaging.queue",
                Exclusive = false,
                Arguments = null
            }
        ];

        await SetupRabbit(connectionOption, exchangeOptionList, queueOptionList);
    }

    
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSection = configuration.GetSection("RabbitMQ");

        var connectionOption = JsonSerializer.Deserialize<ConnectionOption>
            (rabbitMqSection.GetSection("Connection").Value!);

        var exchangeOptionList = JsonSerializer.Deserialize<List<ExchangeOption>>
            (rabbitMqSection.GetSection("Exchanges").Value!);

        var queueOptionList = JsonSerializer.Deserialize<List<QueueOption>>
            (rabbitMqSection.GetSection("Queues").Value!);

        _ = SetupRabbit(connectionOption, exchangeOptionList, queueOptionList);
    }



}
