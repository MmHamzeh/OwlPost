namespace OwlPost.RabbitMq.Models;

internal sealed class QueueOption
{
    #region Fields and Ctor

    internal QueueOption(ExchangeOption exchange, string routingKey)
    {
        Exchange = exchange;
        Durable = exchange.Durable;
        AutoDelete = exchange.AutoDelete;

        RoutingKey = routingKey;
    }

    #endregion

    internal ExchangeOption Exchange { get; private set; }

    internal required string Name { get; init; }
    internal bool Durable { get; init; }
    internal bool Exclusive { get; init; }
    internal bool AutoDelete { get; init; }
    internal IDictionary<string, object?>? Arguments { get; set; }
    internal string RoutingKey => string.IsNullOrWhiteSpace(field) ? Name : field;


}
