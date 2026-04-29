namespace OwlPost.RabbitMq.Models;

internal class QueueOption
{
    public QueueOption(ExchangeOption exchange)
    {
        Exchange = exchange;
    }

    public ExchangeOption Exchange { get; set; }

    public string Name { get; set; }
    public bool Durable { get; set; }
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public IDictionary<string, object?>? Arguments { get; set; }

}
