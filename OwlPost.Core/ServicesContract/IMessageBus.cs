namespace OwlPost.Core.ServicesContract;

public interface IMessageBus
{
    Task PublishMessageAsync<T>(IMessageBusRequest request,
        CancellationToken cancellationToken = default) where T : IMessageBusResponse;
}
