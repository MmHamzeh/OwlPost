using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace OwlPost.RabbitMq;

public static class OwlPostRabbitMqExtension
{
    public static void AddRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        var exchangeOptionQueueOption = SeedData.GetExchangeOptionQueueOption();

        var options = new Action<RabbitMqOptions>(opt =>
        {
            opt.Connection = SeedData.GetConnectionOption(configuration);
            opt.Exchange = exchangeOptionQueueOption.Item1;
            opt.Queue = exchangeOptionQueueOption.Item2;
            opt.Channel = SeedData.GetChannelOption(configuration);
        });

        services.Configure(options);
        AddServices(services);
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


}
