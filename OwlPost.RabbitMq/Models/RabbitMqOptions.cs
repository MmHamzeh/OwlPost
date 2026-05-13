namespace OwlPost.RabbitMq.Models;

internal sealed class RabbitMqOptions
{
    internal required ConnectionOption Connection { get; set; }
    internal required ChannelOption Channel { get; set; }
    internal required List<ExchangeOption> Exchange { get; set; }
    internal required List<QueueOption> Queue { get; set; }
}
