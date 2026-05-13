namespace OwlPost.Core.RepositoriesContract;

public interface IMessageRepository
{
    Task Add(ChatMessage message, bool saveChanges, CancellationToken ct);
}