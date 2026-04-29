namespace OwlPost.RabbitMq.Models;

public class ExchangeOption
{
    public required string Name { get; set; }
    public required ExchangeTypeEnm ExchangeTypeEnm { get; set; }
    public required bool Durable { get; set; }
    public required bool AutoDelete { get; set; }
    public IDictionary<string, object?>? Arguments { get; set; }

}
