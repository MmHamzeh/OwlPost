namespace OwlPost.RabbitMq.Models;

public class QueueOption
{
    public QueueOption()
    {
        
    }

    public QueueOption(ExchangeOption exchange)
    {
        Exchange = exchange;
        Durable = exchange.Durable;
        AutoDelete = exchange.AutoDelete;
    }

    public ExchangeOption Exchange { get; set; }

    public string Name { get; set; }
    public bool Durable { get; set; }
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public IDictionary<string, object?>? Arguments { get; set; }

}
