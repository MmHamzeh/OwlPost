namespace OwlPost.RabbitMq.Contracts;

internal interface IRabbitMqExchangeBuilder : IAsyncDisposable
{
    ValueTask DeclareExchanges();

}
