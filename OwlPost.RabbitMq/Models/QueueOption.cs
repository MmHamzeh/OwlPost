namespace OwlPost.RabbitMq.Models;

public class QueueOption
{
    #region Fields and Ctor

    public QueueOption(ExchangeOption exchange, string routingKey)
    {
        Exchange = exchange;
        Durable = exchange.Durable;
        AutoDelete = exchange.AutoDelete;

        RoutingKey = routingKey;
    }

    #endregion

    public ExchangeOption Exchange { get; private set; }

    public required string Name { get; init; }
    public bool Durable { get; init; }
    public bool Exclusive { get; init; }
    public bool AutoDelete { get; init; }
    public IDictionary<string, object?>? Arguments { get; set; }
    public string RoutingKey => string.IsNullOrWhiteSpace(field) ? Name : field;


}
