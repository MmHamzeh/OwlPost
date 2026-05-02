using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace OwlPost.RabbitMq;

public static class OwlPostRabbitMqExtension
{
    public static void AddRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new Action<RabbitMqOptions>(opt =>
        {
            opt.Connection = GetConnectionOption(configuration);
            opt.Exchange = GetExchangeOption(configuration);
            opt.Queue = GetQueueOption(configuration);
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

    private static List<ExchangeOption> GetExchangeOption(IConfiguration configuration)
    {
        var json = configuration.GetSection("RabbitMQ:Exchanges")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("");

        var exchangeOptionList = JsonSerializer.Deserialize<List<ExchangeOption>>(json);

        if (exchangeOptionList == null)
            throw new Exception("");

        return exchangeOptionList;
    }

    private static List<QueueOption> GetQueueOption(IConfiguration configuration)
    {
        var json = configuration.GetSection("RabbitMQ:Queues")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("");

        var queueOptionList = JsonSerializer.Deserialize<List<QueueOption>>(json);

        if (queueOptionList == null)
            throw new Exception("");

        return queueOptionList;
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

        services.AddSingleton<IRabbitMqConnectionManagement, RabbitMqConnectionManagement>();
        services.AddSingleton<IRabbitMqExchangeManagement, RabbitMqExchangeManagement>();
        services.AddSingleton<IRabbitMqQueueManagement, RabbitMqQueueManagement>();

        services.AddSingleton<IMessageBus, MessageBus>();

        services.AddHostedService<MessageConsumer>();
    }

    #endregion

}
