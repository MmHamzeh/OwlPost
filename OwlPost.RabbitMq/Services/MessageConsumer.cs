using Microsoft.Extensions.Hosting;

namespace OwlPost.RabbitMq.Services;

internal class MessageConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Consume queues here
    }
}
