namespace OwlPost.RabbitMq.Contracts;

internal interface IRabbitMqQueueBuilder : IAsyncDisposable
{
    ValueTask DeclareQueues();

}
