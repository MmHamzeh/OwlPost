namespace OwlPost.RabbitMq.Models;

internal class RabbitMqOptions
{
    internal ConnectionOption Connection { get; set; }
    internal ChannelOption Channel { get; set; }
    internal List<ExchangeOption> Exchange { get; set; }
    internal List<QueueOption> Queue { get; set; }
}
