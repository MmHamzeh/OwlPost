namespace OwlPost.Core.RepositoriesContract;

public interface IMessageRepository
{
    Task Add(ChatMessage message, CancellationToken ct);
}