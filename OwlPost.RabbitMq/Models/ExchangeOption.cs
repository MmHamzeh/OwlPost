namespace OwlPost.RabbitMq.Models;

internal sealed class ExchangeOption
{
    internal required string Name { get; init; }
    internal required ExchangeTypeEnm ExchangeTypeEnm { get; init; }
    internal required bool Durable { get; init; }
    internal required bool AutoDelete { get; init; }
    internal IDictionary<string, object?>? Arguments { get; init; }

}
