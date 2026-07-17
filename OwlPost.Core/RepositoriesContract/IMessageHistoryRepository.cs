namespace OwlPost.Core.RepositoriesContract;

public interface IMessageHistoryRepository
{
    Task Add(ChatMessageHistory chatMessageHistory, CancellationToken ct);
}