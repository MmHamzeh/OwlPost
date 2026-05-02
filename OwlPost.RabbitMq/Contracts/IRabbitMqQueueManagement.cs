namespace OwlPost.RabbitMq.Contracts;

internal interface IRabbitMqQueueManagement : IAsyncDisposable
{
    ValueTask DeclareQueues();

}
