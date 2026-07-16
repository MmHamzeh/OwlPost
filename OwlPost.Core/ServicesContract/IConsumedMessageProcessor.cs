namespace OwlPost.Core.ServicesContract;

public interface IConsumedMessageProcessor
{
    Task ProcessAsync(IMessageBusRequest message, CancellationToken ct);
}