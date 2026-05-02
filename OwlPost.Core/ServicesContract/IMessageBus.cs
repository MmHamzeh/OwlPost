namespace OwlPost.Core.ServicesContract;

public interface IMessageBus
{
    Task PublishAsync<T>(IMessageBusRequest request,
        CancellationToken cancellationToken = default) where T : IMessageBusResponse;
}
