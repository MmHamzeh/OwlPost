namespace OwlPost.RabbitMq.Contracts;

internal interface IRabbitMqExchangeManagement : IAsyncDisposable
{
    ValueTask DeclareExchanges();

}
