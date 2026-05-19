using Microsoft.Extensions.Hosting;

namespace OwlPost.RabbitMq;

internal sealed class RabbitMqInitializer(
    IConnectionManager connectionManager,
    IChannelManager channelManager,
    IExchangeManager exchangeManager,
    IQueueManager queueManager)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connection = await connectionManager.GetConnectionAsync();

        if (connection == null)
            throw new Exception("Failed to establish RabbitMQ connectionManager.");

        var channel = await channelManager.GetChannelAsyncForPublish();

        if (channel == null)
            throw new Exception("Failed to create RabbitMQ channel.");

        await exchangeManager.DeclareExchanges(channel);

        await queueManager.DeclareQueues(channel);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await queueManager.DisposeAsync();
        await exchangeManager.DisposeAsync();
        await channelManager.DisposeAsync();
        await connectionManager.DisposeAsync();
    }
}