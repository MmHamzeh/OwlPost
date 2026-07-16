namespace OwlPost.Core.ServicesContract;

public interface IChatHubService
{
    Task NotifyMessageAdded(ChatMessage entity, CancellationToken ct);
}